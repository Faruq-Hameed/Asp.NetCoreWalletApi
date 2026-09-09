using WalletApi.Models;

namespace WalletApi.Services;

public interface ITokenService
{
    string GenerateToken(User user);
}
