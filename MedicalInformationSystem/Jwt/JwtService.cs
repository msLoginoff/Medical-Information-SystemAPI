using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MedicalInformationSystem.Data;
using Microsoft.IdentityModel.Tokens;

namespace MedicalInformationSystem.Jwt;

public class JwtService : IJwtService
{
    private readonly AppDbContext _context;

    public JwtService(AppDbContext context)
    {
        _context = context;
    }

    public long? GetTokenSeriesByDoctorId(Guid doctorId)
    {
       return _context.Password.FirstOrDefault(x => x.Doctor.Id == doctorId)?.TokenSeries;
    }

    public string? GenerateToken(Guid doctorId)
    {
        var tokenSeries = GetTokenSeriesByDoctorId(doctorId);

        if (tokenSeries == null)
        {
            return null;
        }
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, doctorId.ToString()),
            new Claim(ClaimTypes.Version, tokenSeries.ToString())
        };
        
        var now = DateTime.UtcNow;
        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            issuer: JwtConfigurations.Issuer,
            audience: JwtConfigurations.Audience,
            notBefore: now,
            claims: claims,
            expires: now.Add(TimeSpan.FromSeconds(JwtConfigurations.Lifetime)),
            signingCredentials: new SigningCredentials(JwtConfigurations.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return encodedJwt;
    }

}