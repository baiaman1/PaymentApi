using PaymentApi.Domain.Entities;

namespace PaymentApi.Application.Interfaces.Repositories
{
    public interface ISessionRepository
    {
        Task AddAsync(Session session);
        Task<Session?> GetValidByTokenAsync(string token);
        Task RevokeAsync(Session session);
    }
}
