using System.Text.Json.Serialization;

namespace MinesweeperAPI.Models.Dtos
{
    public class GameDto
    {
        [JsonPropertyName("game_id")]
        public Guid GameId { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        [JsonPropertyName("mines_count")]
        public int MinesCount { get; set; }

        public bool Completed { get; set; }

        public IEnumerable<IEnumerable<string>> Field { get; set; }
    }
}
