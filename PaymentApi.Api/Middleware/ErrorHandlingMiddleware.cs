using PaymentApi.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace PaymentApi.Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpException ex)
            {
                await WriteError(context, ex.StatusCode, ex.Errors?.FirstOrDefault());
            }
            catch (UnauthorizedAccessException)
            {
                await WriteError(context, HttpStatusCode.Unauthorized, "Invalid login or password");
            }
            catch
            {
                await WriteError(context, HttpStatusCode.InternalServerError, "Internal server error");
            }
        }

        private static async Task WriteError(HttpContext context, HttpStatusCode statusCode, string? message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                StatusCode = (int)statusCode,
                Message = message
            }));
        }
    }
}