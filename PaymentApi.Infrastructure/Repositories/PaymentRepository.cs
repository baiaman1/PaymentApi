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

            var user = await _db.Users
                .FromSqlInterpolated($"SELECT * FROM \"Users\" WHERE \"Id\" = {userId} FOR UPDATE")
                .FirstOrDefaultAsync();

            if (user == null)
            {
                await trx.RollbackAsync();
                throw new Exception("User not found");
            }

            if (user.Balance < amount)
            {
                await trx.RollbackAsync();
                return user.Balance; // не списываем
            }

            user.Balance -= amount;
            _db.Users.Update(user);

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

            return user.Balance;
        }

    }
}
