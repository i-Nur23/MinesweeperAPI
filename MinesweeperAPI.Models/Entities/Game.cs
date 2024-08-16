using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinesweeperAPI.Models.Entities
{
    public class Game
    {
        public Guid Id { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int MinesCount { get; set; }

        public bool IsCompleted { get; set; }

        public bool IsStarted { get; set; }

        public string FieldText { get; set; }

        public int ClosedCellsCount { get; set; }

        public string CurrentFieldText { get; set; }

        [NotMapped]
        public int[][] Field => 
            JsonConvert.DeserializeObject<int[][]>(FieldText);

        [NotMapped]
        public string[][] CurrentField =>
            JsonConvert.DeserializeObject<string[][]>(CurrentFieldText);

        public DateTime UpdatedAt { get; set; }
    }
}