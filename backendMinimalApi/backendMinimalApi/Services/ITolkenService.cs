using backendMinimalApi.Models;

namespace backendMinimalApi.Services
{
    public interface ITokenService
    {
        string MakeToken(string key, string issuer, string audience, UserModel user);
    }
}
