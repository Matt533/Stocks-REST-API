using StocksApplication.Models;

namespace StocksApplication.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
