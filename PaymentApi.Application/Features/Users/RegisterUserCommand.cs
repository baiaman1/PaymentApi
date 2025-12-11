using MediatR;

namespace PaymentApi.Application.Features.Users
{
    public class RegisterUserCommand : IRequest<string>
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
