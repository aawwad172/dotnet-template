namespace Dotnet.Template.Application.Utilities;

public class SecurityUtilities
{
    public static (string Hash, string Salt) SplitHashSalt(string combinedHashSalt)
    {
        // Uses the '-' delimiter defined in your SecurityService implementation
        string[] parts = combinedHashSalt.Split('-');
        if (parts.Length != 2)
        {
            // Handle error appropriately, e.g., throw exception or return defaults
            throw new InvalidOperationException("Password hash is not in the expected HASH-SALT format.");
        }
        return (parts[0], parts[1]);
    }
}
