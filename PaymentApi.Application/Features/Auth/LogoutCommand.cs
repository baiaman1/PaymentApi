using MediatR;

namespace PaymentApi.Application.Features.Auth
{
    public class LogoutCommand : IRequest<bool>
    {
        public string Token { get; set; } = null!;
    }
}
