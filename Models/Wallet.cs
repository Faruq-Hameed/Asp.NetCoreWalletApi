namespace WalletApi.Models;

public class Wallet
{
    public int Id { get; set; }

    // Foreign key convention: EF Core recognizes "UserId" as the FK for the User navigation
    // property below, purely by naming convention - no decorator needed (though you can be
    // explicit with [ForeignKey] if you want).
    public int UserId { get; set; }
    public User? User { get; set; }

    public decimal Balance { get; set; }

    // One wallet has many transactions - the "many" side of a one-to-many relationship.
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
