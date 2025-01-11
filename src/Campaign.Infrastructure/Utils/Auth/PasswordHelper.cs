using System.Security.Cryptography;
using System.Text;

namespace Campaign.Infrastructure.Utils.Auth;

public static class PasswordHelper
{
    public static string ComputeSha1Hash(string input)
    {
        using var sha1 = SHA1.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = sha1.ComputeHash(bytes);
        var hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        return hash;
    }
}
