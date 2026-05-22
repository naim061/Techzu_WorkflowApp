using SponsorshipWorkflow.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace SponsorshipWorkflow.Infrastructure.Services;

public class PasswordService : IPasswordService
{
    private const int SaltSize = 32;
    private const int HashSize = 64;
    private const int Iterations = 100_000;

    public (string Hash, string Salt) HashPassword(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var salt = Convert.ToBase64String(saltBytes);

        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            saltBytes,
            Iterations,
            HashAlgorithmName.SHA512,
            HashSize);

        return (Convert.ToBase64String(hashBytes), salt);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        try
        {
            var saltBytes = Convert.FromBase64String(salt);
            var hashBytes = Convert.FromBase64String(hash);

            var testHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA512,
                HashSize);

            return CryptographicOperations.FixedTimeEquals(testHash, hashBytes);
        }
        catch
        {
            return false;
        }
    }
}