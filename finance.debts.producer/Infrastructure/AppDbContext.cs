using Microsoft.EntityFrameworkCore;
using finance.debts.domain.Entities;

namespace finance.debts.producer.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Debt> Debts => Set<Debt>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Debt>(entity =>
            {
                modelBuilder.Entity<Debt>(entity =>
                {
                    entity.ToTable("debts");

                    entity.HasKey(x => x.DebtId);

                    entity.Property(e => e.DebtId).HasColumnName("debt_id");
                    entity.Property(e => e.ClientId).HasColumnName("client_id");
                    entity.Property(e => e.AmountDue).HasColumnName("amount_due");
                    entity.Property(e => e.AmountPaid).HasColumnName("amount_paid");
                    entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
                    entity.Property(e => e.CorrelationId).HasColumnName("correlation_id");
                    entity.Property(e => e.StatusId).HasColumnName("status_id");
                    entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                });
            });
        }
    }
}