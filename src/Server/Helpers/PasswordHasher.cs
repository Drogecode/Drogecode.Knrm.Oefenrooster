using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Globalization;
using System.Security.Cryptography;

namespace Drogecode.Knrm.Oefenrooster.Server.Helpers;

public static class PasswordHasher
{
  public static bool ComparePassword(string? inputPassword, string? correctHashedPassword)
  {
    if (correctHashedPassword is null || inputPassword is null)
      return false;
    var passParts = correctHashedPassword.Split(".");
    var salt = Convert.FromBase64String(passParts[0]);
    var prf = StringToKeyDerivationPrf(passParts[1]);
    var iterationCount = 100000;
    int numBytesRequested = 512 / 8;
    if (int.TryParse(passParts[2], out int x))
      iterationCount = x;
    if (int.TryParse(passParts[3], out x))
      numBytesRequested = x;
    var checkPassword = HashPassword(inputPassword, salt, prf, iterationCount, numBytesRequested);
    if (string.Compare(correctHashedPassword, checkPassword, false, CultureInfo.InvariantCulture) == 0)
      return true;
    return false;
  }

  public static string HashNewPassword(string password)
  {
    byte[] salt = GenerateSalt();
    string hash = HashPassword(password, salt);
    return hash;
  }

  private static string HashPassword(string password, byte[] salt, KeyDerivationPrf prf = KeyDerivationPrf.HMACSHA512, int iterationCount = 100000, int numBytesRequested = 512 / 8)
  {
    var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
      password: password,
      salt: salt,
      prf: prf,
      iterationCount: iterationCount,
      numBytesRequested: numBytesRequested));
    var saltString = Convert.ToBase64String(salt);
    var result = string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}.{4}", saltString, prf, iterationCount, numBytesRequested, hashed);
    return result;
  }

  private static byte[] GenerateSalt()
  {
    byte[] salt = new byte[128 / 8];
    using (var rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(salt);
    }
    return salt;
  }

  private static KeyDerivationPrf StringToKeyDerivationPrf(string prf)
  {
    KeyDerivationPrf ret;
    switch (prf)
    {
      case "HMACSHA1":
        ret = KeyDerivationPrf.HMACSHA1;
        break;
      case "HMACSHA256":
        ret = KeyDerivationPrf.HMACSHA256;
        break;
      case "HMACSHA512":
      default:
        ret = KeyDerivationPrf.HMACSHA512;
        break;
    }
    return ret;
  }
}