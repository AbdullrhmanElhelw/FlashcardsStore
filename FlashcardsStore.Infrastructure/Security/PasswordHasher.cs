using System.Security.Cryptography;
using System.Text;
using FlashcardsStore.Application.Interfaces.Services;

namespace FlashcardsStore.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128-bit salt
    private const int HashSize = 32; // 256-bit hash
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            Algorithm,
            HashSize);

        // Standard OWASP-recommended format: $pbkdf2-sha256$iterations$salt$hash
        return $"$pbkdf2-sha256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string enteredPassword, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(enteredPassword) || string.IsNullOrWhiteSpace(storedHash))
        {
            return false;
        }

        // Modern PBKDF2 verification with constant-time equality check
        if (storedHash.StartsWith("$pbkdf2-sha256$"))
        {
            var parts = storedHash.Split('$');
            if (parts.Length != 5) return false;

            if (!int.TryParse(parts[2], out int iterations)) return false;
            byte[] salt = Convert.FromBase64String(parts[3]);
            byte[] expectedHash = Convert.FromBase64String(parts[4]);

            byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(enteredPassword),
                salt,
                iterations,
                Algorithm,
                expectedHash.Length);

            return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
        }

        // Legacy plain SHA-256 fallback for backwards compatibility
        var legacyHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(enteredPassword)));
        return string.Equals(legacyHash, storedHash, StringComparison.OrdinalIgnoreCase);
    }
}
