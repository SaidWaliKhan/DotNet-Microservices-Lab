namespace UserService.Services;

public interface IPasswordHasherService
{
    string Hash(string plainTextPassword);
    bool Verify(string plainTextPassword, string hash);
}

// BCrypt is deliberately SLOW (by design, via its "work factor") - that's
// a feature, not a bug. It makes brute-forcing stolen password hashes
// computationally expensive. Never use a fast hash like MD5/SHA256 for
// passwords - those are built for speed, which is exactly wrong here.
public class PasswordHasherService : IPasswordHasherService
{
    // Work factor 12 is a reasonable default in 2026 - high enough to be
    // slow for attackers, low enough not to noticeably slow down a real login.
    private const int WorkFactor = 12;

    public string Hash(string plainTextPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainTextPassword, WorkFactor);
    }

    public bool Verify(string plainTextPassword, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(plainTextPassword, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // A malformed/empty hash should behave like "wrong password",
            // not crash the login endpoint with a 500. This can happen with
            // corrupted data or an account that was never properly seeded.
            return false;
        }
    }
}