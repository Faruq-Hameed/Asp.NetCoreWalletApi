using Microsoft.AspNetCore.Mvc;
using WalletApi.DTOs;
using WalletApi.Models;

namespace WalletApi.Services;

public interface IAuthService
{
    Task Register(RegisterDto dto);

    Task<AuthResponseDto> Login(LoginDto dto);
}

