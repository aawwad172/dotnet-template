using System.Security.Cryptography;
using System.Text;

using Dotnet.Template.Application.Interfaces.Services;
using Dotnet.Template.Application.Utilities;

using Microsoft.Extensions.Configuration;

namespace Dotnet.Template.Application.HelperServices;

public class EncryptionService(IConfiguration configuration) : IEncryptionService
{
    private readonly byte[] _encryptionKey = Encoding.UTF8.GetBytes(configuration.GetRequiredSetting("EncryptionKey"));

    // Todo: Consider adding the Salt Method to the Hashed Password
    public string Hash(string input)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public string EncryptString(string text)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        byte[] plainBytes = Encoding.UTF8.GetBytes(text);
        byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        byte[] result = new byte[aes.IV.Length + encryptedBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string DecryptString(string cipherText)
    {
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using var aes = Aes.Create();
        aes.Key = _encryptionKey;

        byte[] iv = new byte[aes.BlockSize / 8];
        byte[] encryptedData = new byte[cipherBytes.Length - iv.Length];

        Buffer.BlockCopy(cipherBytes, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(cipherBytes, iv.Length, encryptedData, 0, encryptedData.Length);

        aes.IV = iv;
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public string GenerateRandomString()
    {
        byte[] randomBytes = new byte[32]; // 256-bit secure random token
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes); // Base64 encoded string
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var result = Hash(password);
        return result == passwordHash;
    }
}
