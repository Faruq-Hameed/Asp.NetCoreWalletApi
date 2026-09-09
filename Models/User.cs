namespace WalletApi.Models;

// A plain C# class mapped to a table by EF Core - this is your TypeORM @Entity() equivalent,
// just without decorators. EF Core infers the table/column mapping by convention,
// and we fine-tune relationships in AppDbContext.OnModelCreating.
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation property: lets EF Core (and our code) traverse User -> Wallet.
    // This is what makes `.Include(u => u.Wallet)` possible later.
    public Wallet? Wallet { get; set; }
}
