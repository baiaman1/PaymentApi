using MediatR;

namespace PaymentApi.Application.Features.Auth
{
    public class LoginCommand : IRequest<LoginResult>
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginResult
    {
        public string Token { get; set; } = null!;
    }
}
