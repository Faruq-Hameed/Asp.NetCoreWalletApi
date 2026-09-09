using System.ComponentModel.DataAnnotations;

namespace WalletApi.DTOs;

// `record` gives you a concise, immutable data class with built-in equality -
// perfect for DTOs. This is the same instinct as using class-validator DTOs in NestJS:
// never bind requests directly to your entity classes.
public record RegisterDto(
    [Required, MinLength(3), MaxLength(100)] //I need them to be normalized to lowerase

    string Username,

    [Required, MinLength(6)] 
    string Password
);

public record LoginDto(
    [Required] //CHECK WITH AND WITHOUT [Required]
    string Username,

    [Required] 
    string Password
);

public record AuthResponseDto(string Token, string Username);
