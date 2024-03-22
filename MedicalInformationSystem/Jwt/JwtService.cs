using System.Security.Claims;
using MedicalInformationSystem.Data;
using MedicalInformationSystem.Jwt;

namespace MedicalInformationSystem.Jwt;

public class JwtService : IJwtService
{
    private readonly AppDbContext _context;

    public JwtService(AppDbContext context)
    {
        _context = context;
    }

    public ClaimsIdentity GetIdentity(string username, string password)
    {
        var user = _context.Doctor.FirstOrDefault(x => x.Email == username);
        
        if (user == null)
        {
            return null;
        }

        // Claims описывают набор базовых данных для авторизованного пользователя
        var claims = new List<Claim>
        {
            new (ClaimsIdentity.DefaultNameClaimType, user.Id.ToString())
        };

        //Claims identity и будет являться полезной нагрузкой в JWT токене, которая будет проверяться стандартным атрибутом Authorize
        var claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        return claimsIdentity;
    }

}