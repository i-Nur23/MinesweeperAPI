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
                IsStarted = true,
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
            if (row < 2 || row > 30)
            {
                throw new BadRequestException("Ряд должен быть не менее 2 и не более 30");
            }

            if (col < 2 || col > 30)
            {
                throw new BadRequestException("Столбец должен быть не менее 2 и не более 30");
            }

            Game? game = await _gamesRepository.GetAsync(gameId, cancellationToken)
                ?? throw new BadRequestException($"Игра с идентификатором {gameId} не обнаружена");

            if (game.IsCompleted)
            {
                throw new BadRequestException("Игра уже окончена");
            }

            int[][] field = game.Field;
            string[][] currentField = game.CurrentField;

            if (!game.IsStarted)
            {
                int[][] coords = GetMineCoords(
                    game.Width * game.Height,
                    game.MinesCount,
                    game.Height,
                    game.Width * row + col);

                game.IsStarted = true;

                foreach (int[] coord in coords)
                {
                    PutMineOnField(field, coord[0], coord[1]);
                }

                game.FieldText = JsonConvert.SerializeObject(field);
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
            int max, 
            int minesCount, 
            int rowsCount, 
            int firstStep)
        {
            Random random = new Random();

            int[] positions = Enumerable.Range(0, max)
                .OrderBy(x => random.Next())
                .Take(minesCount)
                .OrderBy(x => Math.Abs(x - firstStep))
                .ToArray();

            positions[0] = firstStep;

            return positions.Select(x => new[] { x / rowsCount, x % rowsCount }).ToArray();

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
                col >= 0 && col < field.GetLength(1) && field[row][col] != -1)
            {
                field[row][col]++;
            }
        }

        private void EndGame(string[][] currentField, int[][] field, bool isLost)
        {
            int rowsCount = field.GetLength(0);
            int colsCount = field.GetLength(1);

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
               col >= 0 && col < field.GetLength(1) && field[row][col] != -1)
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
