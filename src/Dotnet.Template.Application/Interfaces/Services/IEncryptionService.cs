namespace Dotnet.Template.Application.Interfaces.Services;

public interface IEncryptionService
{
    /// <summary>
    /// Computes a one-way hash of the input string using a cryptographic hash function.
    /// </summary>
    /// <param name="input">The string to hash.</param>
    /// <returns>The computed hash value.</returns>
    public string Hash(string input);

    /// <summary>
    /// Verifies if the input string matches the provided hash.
    /// </summary>
    /// <param name="input">The string to verify.</param>
    /// <param name="hashedInput">The hash to verify against.</param>
    /// <returns>True if the input matches the hash, false otherwise.</returns>
    public bool VerifyHash(string input, string hashedInput);

    /// <summary>
    /// Hashes a password using a secure password hashing algorithm with salt.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The password hash with embedded salt.</returns>
    /// <remarks>
    /// Implementation requirements:
    /// - Use a strong adaptive hashing algorithm (e.g., Argon2, PBKDF2, or BCrypt)
    /// - Generate a unique cryptographic salt per password
    /// - Enforce minimum password length and complexity
    /// </remarks>
    public string HashPassword(string password);

    /// <summary>
    /// Verifies if the password matches the stored password hash.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="passwordHash">The stored password hash.</param>
    /// <returns>True if the password is valid, false otherwise.</returns>
    public bool VerifyPassword(string password, string passwordHash);

    /// <summary>
    /// Encrypts a string using a symmetric encryption algorithm.
    /// </summary>
    /// <param name="text">The text to encrypt.</param>
    /// <returns>The encrypted string.</returns>
    /// <remarks>
    /// Implementation requirements:
    /// - Use AES-256 or equivalent strength encryption
    /// - Implement proper key management and rotation
    /// - Use secure modes of operation (e.g., CBC with PKCS7 padding)
    /// </remarks>
    public string EncryptString(string text);

    /// <summary>
    /// Decrypts a previously encrypted string.
    /// </summary>
    /// <param name="cipherText">The encrypted text to decrypt.</param>
    /// <returns>The decrypted string.</returns>
    public string DecryptString(string cipherText);
}
