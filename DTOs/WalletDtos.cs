using System.ComponentModel.DataAnnotations;

namespace WalletApi.DTOs;

public record CreateTransactionDto(
    [Required, Range(1, 100000000)] // 1 to 100 million
    decimal Amount,

    [Required] 
    string Type, // "Credit" or "Debit"

    string? Description
);

public record TransactionDto(
    int Id,
    decimal Amount,
    string Type,
    string? Description,
    DateTime CreatedAt
);

// This is the shape returned by the joined query in WalletController -
// wallet + its transactions, flattened into one clean response object.
public record WalletSummaryDto(
    decimal Balance,
    List<TransactionDto> Transactions
);
