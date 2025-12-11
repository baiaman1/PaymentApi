using MediatR;
using PaymentApi.Application.Interfaces.Repositories;

namespace PaymentApi.Application.Features.Payments
{
    public class MakePaymentCommandHandler
    : IRequestHandler<MakePaymentCommand, MakePaymentResult>
    {
        private readonly ISessionRepository _sessions;
        private readonly IUserRepository _users;
        private readonly IPaymentRepository _payments;

        private const decimal Charge = 1.10m;

        public MakePaymentCommandHandler(
            ISessionRepository sessions,
            IUserRepository users,
            IPaymentRepository payments)
        {
            _sessions = sessions;
            _users = users;
            _payments = payments;
        }

        public async Task<MakePaymentResult> Handle(
            MakePaymentCommand request,
            CancellationToken cancellationToken)
        {
            // проверяем, что сессия по токену валидна
            var session = await _sessions.GetValidByTokenAsync(request.Token);
            if (session == null)
                throw new UnauthorizedAccessException("Session invalid or token revoked.");

            // получаем пользователя из сессии
            var user = await _users.GetByIdAsync(session.UserId);
            if (user == null)
                throw new UnauthorizedAccessException();

            // проверка достаточности средств
            if (user.Balance < Charge)
            {
                return new MakePaymentResult
                {
                    Success = false,
                    NewBalance = user.Balance
                };
            }

            // атомарная транзакция списания
            var newBalance = await _payments.MakePaymentAtomicAsync(user.Id, Charge);

            return new MakePaymentResult
            {
                Success = true,
                NewBalance = newBalance
            };
        }
    }
}
