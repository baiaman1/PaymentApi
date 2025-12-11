using PaymentApi.Domain.Entities;

namespace PaymentApi.Application.Interfaces.Repositories
{
    public interface ISessionRepository
    {
        Task AddAsync(Session session);
        Task<bool> IsValidAsync(string token);
        Task<bool> InvalidateAsync(string token);
        Task<Session?> GetValidByTokenAsync(string token);
    }
}
