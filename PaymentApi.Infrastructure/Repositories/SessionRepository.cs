using Microsoft.EntityFrameworkCore;
using PaymentApi.Application.Interfaces.Repositories;
using PaymentApi.Domain.Entities;
using PaymentApi.Infrastructure.Persistence;

namespace PaymentApi.Infrastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        private readonly AppDbContext _db;
        public SessionRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(Session session)
        {
            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();
        }

        public async Task<Session?> GetValidByTokenAsync(string token)
        {
            return await _db.Sessions.FirstOrDefaultAsync(s => s.Token == token && !s.IsRevoked);
        }

        public async Task RevokeAsync(Session session)
        {
            session.IsRevoked = true;
            _db.Sessions.Update(session);
            await _db.SaveChangesAsync();
        }
    }
}
