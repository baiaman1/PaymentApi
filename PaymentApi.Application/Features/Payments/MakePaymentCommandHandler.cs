using MediatR;
using PaymentApi.Application.Interfaces.Repositories;

namespace PaymentApi.Application.Features.Payments
{
    public class MakePaymentCommandHandler : IRequestHandler<MakePaymentCommand, MakePaymentResult>
    {
        private readonly ISessionRepository _sessions;
        private readonly IUserRepository _users;
        private readonly IPaymentRepository _payments;
        private readonly PaymentAmountSettings _settings;

        private const decimal Charge = 1.10m;

        public MakePaymentCommandHandler(ISessionRepository sessions, IUserRepository users, IPaymentRepository payments)
        {
            _sessions = sessions;
            _users = users;
            _payments = payments;
        }

        public async Task<MakePaymentResult> Handle(MakePaymentCommand request, CancellationToken cancellationToken)
        {
            var session = await _sessions.GetValidByTokenAsync(request.Token);
            if (session == null) throw new UnauthorizedAccessException("Invalid token");
                        
            var user = await _users.GetByIdAsync(session.UserId);
            if (user == null) throw new Exception("User not found");

            var newBalance = await _payments.MakePaymentAtomicAsync(user.Id, Charge);

            return new MakePaymentResult { NewBalance = newBalance };
        }
    }

    public class PaymentAmountSettings
    {
        public decimal Amount { get; set; } = 1.10m;
    }
}
