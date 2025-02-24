namespace Dotnet.Template.Application.Interfaces.Services;

public interface IEncryptionService
{
    public string Hash(string input);
    public bool VerifyHash(string input, string hashedInput);
    public string HashPassword(string password);
    public bool VerifyPassword(string password, string passwordHash);
    public string EncryptString(string text);
    public string DecryptString(string cipherText);
}
