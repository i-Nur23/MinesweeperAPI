namespace MinesweeperAPI.Models.Entities
{
    public class Game
    {
        public Guid Id { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int MinesCount { get; set; }

        public bool IsCompleted { get; set; }
    }
}
