using PaymentApi.Domain.Entities;

namespace PaymentApi.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task AddAsync(PaymentHistory payment);
        Task<decimal> MakePaymentAtomicAsync(int userId, decimal amount);
    }
}
