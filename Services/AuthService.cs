using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WalletApi.Data;
using WalletApi.DTOs;
using WalletApi.Exceptions;
using WalletApi.Models;
using WalletApi.Services;

public class AuthService : IAuthService
{
    readonly AppDbContext _db;
    readonly ITokenService _tokenService;

    public AuthService(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

        public async Task Register(RegisterDto dto)
    {
        var isUserNameExisting = await _db.Users.AnyAsync(user => user.Username.Equals(dto.Username.ToLower()));
        if (isUserNameExisting)
        {
            throw new ConflictException("Username already taken.");
        }
        // create a new with hashed password and default wallet
        var user = new User
        {
            Username = dto.Username.ToLower(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Wallet = new Wallet { Balance = 0 }
        };
        _db.Users.Add(user);
       await _db.SaveChangesAsync();
       return;
    }

    public async Task<AuthResponseDto> Login(LoginDto dto)
    {
       var user = await _db.Users.FirstOrDefaultAsync(user => user.Username.Equals(dto.Username.ToLower()));
       bool isPasswordCorrect = user is null? false :
       BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
       if(user is null || !isPasswordCorrect)
        {
            throw new UnauthorizedException("Invalid login credentials");
        }

        var token = _tokenService.GenerateToken(user);
        return new AuthResponseDto(token, user.Username);
    }


}