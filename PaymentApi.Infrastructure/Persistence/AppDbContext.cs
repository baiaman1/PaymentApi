using Microsoft.EntityFrameworkCore;
using PaymentApi.Domain.Entities;

namespace PaymentApi.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<PaymentHistory> PaymentHistory => Set<PaymentHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasIndex(u => u.Login).IsUnique();
                b.Property(u => u.Login).HasMaxLength(200).IsRequired();
                b.Property(u => u.PasswordHash).HasMaxLength(200).IsRequired();
                b.Property(u => u.Balance).HasColumnType("decimal(18,2)").HasDefaultValue(0.00m);
                b.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<Session>(b =>
            {
                b.HasKey(s => s.Id);
                b.Property(s => s.Token).HasMaxLength(500).IsRequired();
                b.HasIndex(s => s.Token);
                b.HasOne(s => s.User).WithMany(u => u.Sessions).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<PaymentHistory>(b =>
            {
                b.HasKey(p => p.Id);
                b.Property(p => p.Amount).HasColumnType("decimal(18,2)").IsRequired();
                b.Property(p => p.Currency).HasMaxLength(10).HasDefaultValue("USD");
                b.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
                b.HasOne(p => p.User).WithMany(u => u.Payments).HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
