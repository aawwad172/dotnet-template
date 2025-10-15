namespace Dotnet.Template.Application.Utilities;

public static class MapsterConfiguration
{
    public static void RegisterMappings()
    {
        // Example: Mapping a User entity to a UserDto

        // TypeAdapterConfig<Entity, EntityDTO>.NewConfig()
        //     .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}")
        //     .Ignore(dest => dest.PasswordHash); // Ignore sensitive data

        //  Add additional mappings as needed
    }
}
