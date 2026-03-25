using Microsoft.EntityFrameworkCore;
using finance.debts.Domain;

namespace finance.Infrastructure;

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

                entity.Property(x => x.DebtId).HasColumnName("debt_id");
                entity.Property(x => x.ClientId).HasColumnName("client_id");
                entity.Property(x => x.AmountDue).HasColumnName("amount_due").HasColumnType("decimal(10,2)");
                entity.Property(x => x.StatusId).HasColumnName("status_id");
                entity.Property(x => x.CreatedAt).HasColumnName("created_at");
            });
        });
    }
}