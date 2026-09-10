using Microsoft.AspNetCore.Mvc;
using WalletApi.Data;
using WalletApi.DTOs;
using WalletApi.Services;

namespace WalletApi.Controllers;

// [ApiController] enables automatic model validation (400 responses on invalid DTOs)
// and a few other conveniences - the closest thing to NestJS's ValidationPipe being
// applied automatically. [Route] sets the base path, same as @Controller('auth').
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    // Constructor injection - identical concept to NestJS constructor injection,
    // resolved automatically by the DI container configured in Program.cs.
    public AuthController(AppDbContext db, ITokenService tokenService, IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        await _authService.Register(dto);

        return Ok(new { message = "Registered successfully." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        return Ok(await _authService.Login(dto));
    }
}
