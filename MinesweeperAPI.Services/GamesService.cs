using MinesweeperAPI.Database;
using MinesweeperAPI.Database.Repositories.Interfaces;
using MinesweeperAPI.Models.Dtos;
using MinesweeperAPI.Models.Entities;
using MinesweeperAPI.Models.Exceptions.ResponseExceptions;
using MinesweeperAPI.Services.Interfaces;
using Newtonsoft.Json;

namespace MinesweeperAPI.Services
{
    internal class GamesService : IGamesService
    {
        private readonly IGamesRepository _gamesRepository;
        private readonly IDatabaseRepository _databaseRepository;

        public GamesService(
            IGamesRepository gamesRepository, 
            IDatabaseRepository databaseRepository)
        {
            _gamesRepository = gamesRepository;
            _databaseRepository = databaseRepository;
        }

        public async Task<GameDto> StartNewGameAsync(
            int width,
            int height,
            int minesCount,
            CancellationToken cancellationToken)
        {
            if (width < 2 || width > 30)
            {
                throw new BadRequestException("Ширина поля должна быть не менее 2 и не более 30");
            }

            if (height < 2 || height > 30)
            {
                throw new BadRequestException("Высота поля должна быть не менее 2 и не более 30");
            }

            int maxMinesCount = height * width - 1;

            if (minesCount < 1 || minesCount > maxMinesCount)
            {
                throw new BadRequestException($"Количесто мин должно быть не менее 1 и не более {maxMinesCount}");
            }

            Guid gameId = Guid.NewGuid();

            string[][] currentField = new string[height][];
            int[][] field = new int[height][];  

            for (int i = 0; i < height; i++)
            {
                currentField[i] = Enumerable.Repeat(" ", width).ToArray();
                field[i] = Enumerable.Repeat(0, width).ToArray();
            }

            Game game = new Game
            {
                Id = gameId,
                Height = height,
                Width = width,
                IsCompleted = false,
                MinesCount = minesCount,
                FieldText = JsonConvert.SerializeObject(field),
                CurrentFieldText = JsonConvert.SerializeObject(currentField),
                ClosedCellsCount = width * height - minesCount,
                IsStarted = false,
                UpdatedAt = DateTime.UtcNow,
            };

            await _gamesRepository.AddAsync(game, cancellationToken);
            await _databaseRepository.SaveChangesAsync(cancellationToken);

            return new GameDto 
            { 
                Height = height,
                Width = width,
                GameId = gameId,
                Completed = false,
                Field = currentField,
                MinesCount = minesCount
            };
        }

        public async Task<GameDto> MakeTurnAsync(
            Guid gameId, 
            int row, 
            int col, 
            CancellationToken cancellationToken)
        {
            Game? game = await _gamesRepository.GetAsync(gameId, cancellationToken)
                ?? throw new BadRequestException($"Игра с идентификатором {gameId} не обнаружена");

            if (game.IsCompleted)
            {
                throw new BadRequestException("Игра уже окончена");
            }

            if (row < 0 || row > game.Height - 1)
            {
                throw new BadRequestException($"Ряд должен быть не менее 0 и не более {game.Height - 1}");
            }

            if (col < 0 || col > game.Width - 1)
            {
                throw new BadRequestException($"Столбец должен быть не менее 2 и не более {game.Width - 1}");
            }

            int[][] field = game.Field;
            string[][] currentField = game.CurrentField;

            if (!currentField[row][col].Equals(" "))
            {
                throw new BadRequestException($"Данная ячейка уже открыта");
            }

            if (!game.IsStarted)
            {
                int[][] coords = GetMineCoords(
                    game.MinesCount,
                    game.Height,
                    game.Width,
                    row,
                    col);

                foreach (int[] coord in coords)
                {
                    PutMineOnField(field, coord[0], coord[1]);
                }

                game.FieldText = JsonConvert.SerializeObject(field);
                game.IsStarted = true;
            }

            if (field[row][col] == -1)
            {
                game.IsCompleted = true;
                EndGame(currentField, field, true);
            }
            else
            {
                OpenCell(game, currentField, field, row, col);
            }

            game.CurrentFieldText = JsonConvert.SerializeObject(currentField);
            game.UpdatedAt = DateTime.UtcNow;
            await _databaseRepository.SaveChangesAsync(cancellationToken);

            return new GameDto
            {
                Height = game.Height,
                Width = game.Width,
                GameId = gameId,
                Completed = game.IsCompleted,
                Field = currentField,
                MinesCount = game.MinesCount
            };
        }

        private int[][] GetMineCoords( 
            int minesCount, 
            int rowsCount, 
            int colsCount,
            int row,
            int col)
        {
            int max = rowsCount * colsCount;
            int firstStep = row * rowsCount + col;
            Random random = new Random();

            return Enumerable.Range(0, max)
                .Where(x => !x.Equals(firstStep))
                .OrderBy(x => random.Next())
                .Take(minesCount)
                .Select(x => new[] { x / colsCount, x % colsCount }).ToArray()
                .ToArray();
        }

        private void PutMineOnField(int[][] field, int row, int col)
        {
            field[row][col] = -1;
            AddCellValue(field, row - 1, col - 1);
            AddCellValue(field, row - 1, col);
            AddCellValue(field, row - 1, col + 1);
            AddCellValue(field, row, col + 1);
            AddCellValue(field, row + 1, col + 1);
            AddCellValue(field, row + 1, col);
            AddCellValue(field, row + 1, col - 1);
            AddCellValue(field, row, col - 1);
        }

        private void AddCellValue(int[][] field, int row, int col)
        {
            if (row >= 0 && row < field.GetLength(0) &&
                col >= 0 && col < field[0].Length && field[row][col] != -1)
            {
                field[row][col]++;
            }
        }

        private void EndGame(string[][] currentField, int[][] field, bool isLost)
        {
            int rowsCount = field.GetLength(0);
            int colsCount = field[0].Length;

            for (int i = 0; i < rowsCount; i++)
            {
                for (int j = 0; j < colsCount; j++)
                {
                    if (field[i][j] > -1)
                    {
                        currentField[i][j] = field[i][j].ToString();
                    }
                    else
                    {
                        currentField[i][j] = isLost ? "X" : "M";
                    }
                }
            }
        }

        private void OpenCell(
            Game game, 
            string[][] currentField, 
            int[][] field, 
            int row, 
            int col)
        {
            currentField[row][col] = field[row][col].ToString();
            game.ClosedCellsCount--;
            if (game.ClosedCellsCount <= 0)
            {
                game.IsCompleted = true;
                EndGame(currentField, field, false);
            }

            if (field[row][col] == 0)
            {
                Spread(game, currentField, field, row - 1, col - 1);
                Spread(game, currentField, field, row - 1, col);
                Spread(game, currentField, field, row - 1, col + 1);
                Spread(game, currentField, field, row, col + 1);
                Spread(game, currentField, field, row + 1, col + 1);
                Spread(game, currentField, field, row + 1, col);
                Spread(game, currentField, field, row + 1, col - 1);
                Spread(game, currentField, field, row, col - 1);
            }
        }

        private void Spread(
            Game game,
            string[][] currentField,
            int[][] field,
            int row,
            int col)
        {
            if (row >= 0 && row < field.GetLength(0) &&
               col >= 0 && col < field[0].Length && field[row][col] != -1 && currentField[row][col].Equals(" "))
            {
                OpenCell(
                    game,
                    currentField,
                    field,
                    row,
                    col);
            }
        }
    }
}
