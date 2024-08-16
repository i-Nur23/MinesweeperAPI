using Microsoft.AspNetCore.Mvc;
using MinesweeperAPI.Models.Dtos;
using MinesweeperAPI.Models.ViewModels;
using MinesweeperAPI.Services.Interfaces;

namespace MinesweeperAPI.Controllers
{
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGamesService _gamesService;

        public GameController(IGamesService gamesService)
        {
            _gamesService = gamesService;
        }

        [HttpPost("/new")]
        public async Task<IActionResult> StartNewGameAsync(
            [FromBody] NewGameDto newGameDto,
            CancellationToken cancellationToken)
        {
            GameDto game = await _gamesService.StartNewGameAsync(
                newGameDto.Width,
                newGameDto.Height,
                newGameDto.MinesCount,
                cancellationToken);

            return Ok(game);
        }

        [HttpPost("/turn")]
        public async Task<IActionResult> MakeTurnAsync(
            [FromBody] TurnDto turnDto,
            CancellationToken cancellationToken)
        {
            GameDto game = await _gamesService.MakeTurnAsync(
                turnDto.GameId,
                turnDto.Row,
                turnDto.Column,
                cancellationToken);

            return Ok(game);
        }
    }
}
