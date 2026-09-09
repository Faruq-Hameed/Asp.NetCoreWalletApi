using Microsoft.EntityFrameworkCore;
using WalletApi.Models;

namespace WalletApi.Data;

// AppDbContext is your "connection + schema" object - roughly TypeORM's DataSource,
// but also doubles as your set of repositories via the DbSet<T> properties below.
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    // This is where relationships and constraints that EF Core can't infer by convention
    // get configured explicitly - the C# equivalent of TypeORM's @OneToOne/@OneToMany decorators.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // One-to-one: User <-> Wallet
        modelBuilder.Entity<User>()
            .HasOne(u => u.Wallet)
            .WithOne(w => w.User)
            .HasForeignKey<Wallet>(w => w.UserId);

        // One-to-many: Wallet -> Transactions
        modelBuilder.Entity<Wallet>()
            .HasMany(w => w.Transactions)
            .WithOne(t => t.Wallet)
            .HasForeignKey(t => t.WalletId);

        // Store the enum as a string in the DB instead of an int - much easier to read
        // when you're eyeballing rows directly in Postgres.
        modelBuilder.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>();
    }
}
