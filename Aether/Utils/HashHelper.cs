using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Aether.Utils;

public class HashHelper
{
    public static string ComputeFileHash(string filePath)
    {
        using (var sha256 = SHA256.Create())
        using (var stream = File.OpenRead(filePath))
        {
            byte[] hashBytes = sha256.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (var b in hashBytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }
}
