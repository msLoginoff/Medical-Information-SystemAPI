using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MedicalInformationSystem.Services.Jwt;

public abstract class JwtConfigurations
{
    public const string Issuer = "JwtTestIssuer"; // издатель токена
    public const string Audience = "JwtTestClient"; // потребитель токена
    private const string Key = "SuperSecretKeyBazingaLolKek!*228322";   // ключ для шифрации
    public const int Lifetime = 600; // время жизни токена - 60 минут
    public static SymmetricSecurityKey GetSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
    }
}
