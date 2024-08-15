using System.Net;

namespace MinesweeperAPI.Exceptions
{
    public class CustomResponseException : Exception
    {
        public virtual HttpStatusCode StatusCode { get; set; }

        public CustomResponseException(string message = null) : base(message) { }
    }
}
