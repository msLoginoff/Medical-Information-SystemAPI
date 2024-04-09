namespace MedicalInformationSystem.Services.Jwt;

public interface IJwtService
{
    public long? GetTokenSeriesByDoctorId(Guid doctorId);
    public string? GenerateToken(Guid doctorId);
    
}