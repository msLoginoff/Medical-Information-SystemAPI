using System.Security.Claims;

namespace MedicalInformationSystem.Jwt;

public interface IJwtService
{
    ClaimsIdentity GetIdentity(string username, string password);
}