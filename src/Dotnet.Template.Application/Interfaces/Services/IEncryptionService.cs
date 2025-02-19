namespace Dotnet.Template.Application.Interfaces.Services;

public interface IEncryptionService
{
    public string Hash(string input);
    public bool VerifyPassword(string password, string passwordHash);
    public string EncryptString(string text);
    public string DecryptString(string cipherText);
    public string GenerateRandomString();
}
