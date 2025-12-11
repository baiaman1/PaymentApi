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

        public async Task<bool> IsValidAsync(string token)
        {
            var session = await _db.Sessions.FirstOrDefaultAsync(x => x.Token == token);
            return session != null && !session.IsRevoked;
        }

        public async Task<bool> InvalidateAsync(string token)
        {
            var session = await _db.Sessions.FirstOrDefaultAsync(x => x.Token == token);
            if (session == null) return false;

            session.IsRevoked = true;
            await _db.SaveChangesAsync();
            return true;
        }
        public async Task<Session?> GetValidByTokenAsync(string token)
        {
            return await _db.Sessions.FirstOrDefaultAsync(s => s.Token == token && !s.IsRevoked);
        }
    }
}