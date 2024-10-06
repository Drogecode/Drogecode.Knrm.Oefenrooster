using System.Security.Cryptography;
using System.Text;

namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

public static class SecretHelper
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
}