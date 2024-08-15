using MinesweeperAPI.Models.Exceptions.ResponseExceptions;
using Newtonsoft.Json;
using System.Net;

namespace MinesweeperAPI.Middlewares
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            HttpStatusCode statusCode;

            try
            {
                await _next(context);
            }
            catch (CustomResponseException exception)
            {
                statusCode = exception.StatusCode;

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Error = exception.Message,
                }));
            }
        }
    }
}
