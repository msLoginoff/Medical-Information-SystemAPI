using System.Security.Cryptography;
using System.Text;

namespace MedicalInformationSystem.Services.HashService;

public class HashPasswordService
{
    public string GenerateSalt()
    {
        using var rng = RandomNumberGenerator.Create();
        var saltBytes = new byte[16];
        rng.GetBytes(saltBytes);
        return Convert.ToBase64String(saltBytes);
    }

    public string HashPassword(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var saltedPassword = new byte[saltBytes.Length + passwordBytes.Length];

        Array.Copy(saltBytes, 0, saltedPassword, 0, saltBytes.Length);
        Array.Copy(passwordBytes, 0, saltedPassword, saltBytes.Length, passwordBytes.Length);

        var sha256 = SHA256.Create();

        var hashedPassword = sha256.ComputeHash(saltedPassword);
        sha256.Dispose();
        return Convert.ToBase64String(hashedPassword);
    }

}