using System.Text.Json.Serialization;

namespace MinesweeperAPI.Models.ViewModels
{
    public class NewGameDto
    {
        public int Width { get; set; }

        public int Height { get; set; }

        [JsonPropertyName("mines_count")]
        public int MinesCount { get; set; }
    }
}
