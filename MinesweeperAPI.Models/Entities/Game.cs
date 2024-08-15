namespace MinesweeperAPI.Models.Entities
{
    public class Game
    {
        public Guid Id { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int MinesCount { get; set; }

        public bool IsCompleted { get; set; }

        public string FieldText { get; set; }

        public string CurrentFieldText { get; set; }

        public IEnumerable<IEnumerable<string>> Field => FieldText
            .Split(':')
            .Select(row => row.Split(','));

        public IEnumerable<IEnumerable<string>> CurrentField => CurrentFieldText
            .Split(':')
            .Select(row => row.Split(','));

        public DateTime UpdatedAt { get; set; }
    }
}
