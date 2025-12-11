using MediatR;
using PaymentApi.Application.Interfaces.Repositories;

namespace PaymentApi.Application.Features.Auth
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
    {
        private readonly ISessionRepository _sessions;

        public LogoutCommandHandler(ISessionRepository sessions)
        {
            _sessions = sessions;
        }

        public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _sessions.InvalidateAsync(request.Token);
        }
    }
}
