using System.Net;

namespace MinesweeperAPI.Models.Exceptions.ResponseExceptions
{
    public class BadRequestException : CustomResponseException
    {
        public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.BadRequest;

        public BadRequestException(string message) : base(message) { }
    }
}
