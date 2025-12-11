using Microsoft.EntityFrameworkCore;
using PaymentApi.Application.Interfaces.Repositories;
using PaymentApi.Domain.Entities;
using PaymentApi.Infrastructure.Persistence;

namespace PaymentApi.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _db;
        public PaymentRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(PaymentHistory payment)
        {
            _db.PaymentHistory.Add(payment);
            await _db.SaveChangesAsync();
        }

        public async Task<decimal> MakePaymentAtomicAsync(int userId, decimal amount)
        {
            await using var trx = await _db.Database.BeginTransactionAsync();

            var lockedUser = await _db.Users
                .FromSqlInterpolated($"SELECT * FROM \"Users\" WHERE \"Id\" = {userId} FOR UPDATE")
                .FirstOrDefaultAsync();

            if (lockedUser == null)
            {
                await trx.RollbackAsync();
                throw new Exception("User not found");
            }

            if (lockedUser.Balance < amount)
            {
                await trx.RollbackAsync();
                return lockedUser.Balance;
            }

            lockedUser.Balance -= amount;
            _db.Users.Update(lockedUser);

            var payment = new PaymentHistory
            {
                UserId = userId,
                Amount = amount,
                Currency = "USD",
                CreatedAt = DateTime.UtcNow
            };
            _db.PaymentHistory.Add(payment);

            await _db.SaveChangesAsync();
            await trx.CommitAsync();

            return lockedUser.Balance;
        }
    }
}
