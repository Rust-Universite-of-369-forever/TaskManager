using System.Security.Cryptography;
using System.Text;

namespace TaskManager.Services;

public class PasswordService
{
    public string Hash(string password)
    {
        using var sha = SHA256.Create();
        return Convert.ToHexString(
            sha.ComputeHash(Encoding.UTF8.GetBytes(password)));
    }

    public bool Verify(string password, string hash)
        => Hash(password) == hash;
}
