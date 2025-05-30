using System.Security.Cryptography;
using System.Text;

public static class PasswordHasher
{
    public static string HashPassword(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var combinedBytes = Encoding.UTF8.GetBytes(password + salt);
        var hashBytes = sha256.ComputeHash(combinedBytes);
        return Convert.ToBase64String(hashBytes);
    }

    public static string GenerateSalt()
    {
        var bytes = new byte[16];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
