using System.Security.Cryptography;
using System.Text;

using Dotnet.Template.Application.Interfaces.Services;
using Dotnet.Template.Application.Utilities;

using Microsoft.Extensions.Configuration;

namespace Dotnet.Template.Application.HelperServices;

public class EncryptionService(IConfiguration configuration) : IEncryptionService
{
    private readonly byte[] _encryptionKey = Encoding.UTF8.GetBytes(configuration.GetRequiredSetting("EncryptionKey"));
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;

    public string Hash(string input)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    public string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: Iterations,
            hashAlgorithm: Algorithm,
            outputLength: HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public string EncryptString(string text)
    {
        using AesGcm aesGcm = new AesGcm(_encryptionKey, AesGcm.TagByteSizes.MaxSize);
        byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        RandomNumberGenerator.Fill(nonce);

        byte[] plainBytes = Encoding.UTF8.GetBytes(text);
        byte[] cipherBytes = new byte[plainBytes.Length];
        byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

        aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

        // Combine nonce, tag, and ciphertext into one array for storage or transmission.
        byte[] result = new byte[nonce.Length + tag.Length + cipherBytes.Length];
        Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length + tag.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public string DecryptString(string encryptedText)
    {
        // Convert the Base64-encoded input back into bytes
        byte[] encryptedData = Convert.FromBase64String(encryptedText);

        // Define the sizes of the nonce and tag based on AES-GCM specifications
        int nonceLength = AesGcm.NonceByteSizes.MaxSize; // Typically 12 bytes
        int tagLength = AesGcm.TagByteSizes.MaxSize;       // Typically 16 bytes

        // Allocate buffers for nonce, tag, and ciphertext
        byte[] nonce = new byte[nonceLength];
        byte[] tag = new byte[tagLength];
        byte[] ciphertext = new byte[encryptedData.Length - nonceLength - tagLength];

        // Extract the nonce (first part)
        Buffer.BlockCopy(encryptedData, 0, nonce, 0, nonceLength);
        // Extract the tag (next part)
        Buffer.BlockCopy(encryptedData, nonceLength, tag, 0, tagLength);
        // Extract the ciphertext (remaining bytes)
        Buffer.BlockCopy(encryptedData, nonceLength + tagLength, ciphertext, 0, ciphertext.Length);

        byte[] decryptedBytes = new byte[ciphertext.Length];

        // Use AesGcm to decrypt; it will verify the tag automatically
        using AesGcm aesGcm = new AesGcm(_encryptionKey, AesGcm.TagByteSizes.MaxSize);
        aesGcm.Decrypt(nonce, ciphertext, tag, decryptedBytes);

        // Convert the decrypted bytes to a UTF8 string and return it
        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        string[] parts = passwordHash.Split('-');
        byte[] hash = Convert.FromHexString(parts[0]);
        byte[] salt = Convert.FromHexString(parts[1]);
        byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, inputHash);
    }

    public bool VerifyHash(string input, string hashedInput)
    {
        return Hash(input) == hashedInput;
    }
}
