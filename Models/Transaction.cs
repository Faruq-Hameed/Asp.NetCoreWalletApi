namespace WalletApi.Models;

public enum TransactionType
{
    Credit,
    Debit
}

public class Transaction
{
    public int Id { get; set; }

    public int WalletId { get; set; }
    public Wallet? Wallet { get; set; }

    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
