using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletApi.Data;
using WalletApi.DTOs;
using WalletApi.Models;

namespace WalletApi.Controllers;

// [Authorize] on the whole controller means every action here requires a valid JWT -
// same effect as @UseGuards(AuthGuard('jwt')) on a NestJS controller.
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly AppDbContext _db;

    public WalletController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/wallet/me
    // This is the "join two rows" example: we query Wallet and pull in its related
    // Transactions in a single round trip via .Include() - EF Core generates a SQL JOIN
    // under the hood instead of you writing raw SQL or making two separate queries.
    [HttpGet("me")]
    public async Task<IActionResult> GetMyWallet()
    {
        var userId = GetCurrentUserId();

        var wallet = await _db.Wallets
            .Include(w => w.Transactions)          // <-- the join
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet is null)
            return NotFound(new { error = "Wallet not found." });

        var summary = new WalletSummaryDto(
            wallet.Balance,
            wallet.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new TransactionDto(t.Id, t.Amount, t.Type.ToString(), t.Description, t.CreatedAt))
                .ToList()
        );

        return Ok(summary);
    }

    // POST /api/wallet/transactions
    // Demonstrates a basic ledger write: validate funds, update the balance,
    // and record the transaction - the core pattern behind any wallet/payments backend.
    [HttpPost("transactions")]
    public async Task<IActionResult> AddTransaction(CreateTransactionDto dto)
    {
        if (!Enum.TryParse<TransactionType>(dto.Type, ignoreCase: true, out var type))
            return BadRequest(new { error = "Type must be 'Credit' or 'Debit'." });

        var userId = GetCurrentUserId();
        var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet is null)
            return NotFound(new { error = "Wallet not found." });

        if (type == TransactionType.Debit && wallet.Balance < dto.Amount)
            return BadRequest(new { error = "Insufficient funds." });

        wallet.Balance += type == TransactionType.Credit ? dto.Amount : -dto.Amount;

        var transaction = new Transaction
        {
            WalletId = wallet.Id,
            Amount = dto.Amount,
            Type = type,
            Description = dto.Description
        };

        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync();

        return Ok(new { wallet.Balance });
    }

    // Pulls the user id out of the JWT claims set by TokenService - the C# equivalent
    // of reading `req.user.id` after a NestJS AuthGuard has run.
    private int GetCurrentUserId()
    {
        Console.WriteLine("User Claims:");
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}, Sub: {JwtRegisteredClaimNames.Sub}");
        }
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
       Console.WriteLine($"Extracted Sub Claim: {sub}");
        return int.Parse(sub!);
    }
}
