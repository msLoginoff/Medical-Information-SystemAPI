using System.Security.Claims;
using MedicalInformationSystem.Services.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace MedicalInformationSystem.Requirements;

public class TokenHandler : AuthorizationHandler<TokenRequirement>
{
    private readonly IJwtService _jwtService;

    public TokenHandler(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }
    
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TokenRequirement requirement)
    {
        var doctorId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorId == null)
        {
            return Task.CompletedTask;
        }

        var tokenSeriesFromToken = context.User.FindFirst(ClaimTypes.Version)?.Value;
        var tokenSeriesFromDatabase = _jwtService.GetTokenSeriesByDoctorId(Guid.Parse(doctorId)).ToString();

        if (tokenSeriesFromToken != null && tokenSeriesFromDatabase != null && tokenSeriesFromDatabase == tokenSeriesFromToken)
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}