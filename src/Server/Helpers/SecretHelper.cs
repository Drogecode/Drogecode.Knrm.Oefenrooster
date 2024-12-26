using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

public static partial class SecretHelper
{
    internal static string CreateSecret(int length)
    {
        var res = new StringBuilder();
        var random = new byte[1];
        using (var cryptoProvider = new RNGCryptoServiceProvider())
        {
            while (0 < length--)
            {
                char rndChar;
                do
                {
                    cryptoProvider.GetBytes(random);
                    rndChar = (char)((random[0] % 92) + 33);
                } while (!char.IsLetterOrDigit(rndChar));

                res.Append(rndChar);
            }
        }

        return res.ToString();
    }

    internal static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        var b64Hash = Convert.ToBase64String(hash);
        var code = MyRegex().Replace(b64Hash, "-");
        code = MyRegex1().Replace(code, "_");
        code = MyRegex2().Replace(code, "");
        return code;
    }

    [GeneratedRegex("\\+")]
    private static partial Regex MyRegex();

    [GeneratedRegex("\\/")]
    private static partial Regex MyRegex1();

    [GeneratedRegex("=+$")]
    private static partial Regex MyRegex2();
}