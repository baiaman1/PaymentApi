using Microsoft.EntityFrameworkCore;
using PaymentApi.Domain.Entities;

namespace PaymentApi.Application.Interfaces
{
    public interface IAppDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Session> Sessions { get; }
        DbSet<PaymentHistory> PaymentHistory { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
