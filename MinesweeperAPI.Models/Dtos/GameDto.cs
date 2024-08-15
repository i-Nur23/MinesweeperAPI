namespace MinesweeperAPI.Models.Dtos
{
    public class GameDto
    {
        public Guid GameId { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public int MinesCount { get; set; }

        public bool Completed { get; set; }

        public IEnumerable<IEnumerable<string>> Field { get; set; }
    }
}
