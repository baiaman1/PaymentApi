using PaymentApi.Application.Interfaces.Repositories;

namespace PaymentApi.Api.Middleware
{
    public class JwtSessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtSessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext ctx, ISessionRepository sessions)
        {
            var auth = ctx.Request.Headers["Authorization"].ToString();
            if (auth.StartsWith("Bearer "))
            {
                var token = auth.Substring("Bearer ".Length);

                var valid = await sessions.IsValidAsync(token);
                if (!valid)
                {
                    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            await _next(ctx);
        }
    }
}
