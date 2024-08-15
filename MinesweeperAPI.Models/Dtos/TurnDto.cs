using System.Text.Json.Serialization;

namespace MinesweeperAPI.Models.ViewModels
{
    public class TurnDto
    {
        [JsonPropertyName("game_id")]
        public Guid GameId { get; set; }

        [JsonPropertyName("col")]
        public int Column { get; set; }

        public int Row { get; set; }
    }
}
