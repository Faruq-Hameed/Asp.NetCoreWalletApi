using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletApi.Data;
using WalletApi.DTOs;
using WalletApi.Models;
using WalletApi.Services;

namespace WalletApi.Controllers;

// [ApiController] enables automatic model validation (400 responses on invalid DTOs)
// and a few other conveniences - the closest thing to NestJS's ValidationPipe being
// applied automatically. [Route] sets the base path, same as @Controller('auth').
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IAuthService _authService;

    // Constructor injection - identical concept to NestJS constructor injection,
    // resolved automatically by the DI container configured in Program.cs.
    public AuthController(AppDbContext db, ITokenService tokenService, IAuthService authService)
    {
        _db = db;
        _tokenService = tokenService;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var usernameTaken = await _db.Users.AnyAsync(u => u.Username == dto.Username.ToLower());
        if (usernameTaken)
            return Conflict(new { error = "Username already taken." });
        var user = new User
        {
            Username = dto.Username.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            // Every new user gets a wallet created in the same transaction -
            // EF Core saves both User and Wallet together because of the navigation property.
            Wallet = new Wallet { Balance = 0 }
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        // var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == dto.Username.ToLower());

        // if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        //     return Unauthorized(new { error = "Invalid username or password." }); //CHECK THEM
        //     // return Unauthorized(new { error = "Invalid username or password." });


        // var token = _tokenService.GenerateToken(user);
        return Ok(await _authService.Login(dto));
    }
}
