namespace Dotnet.Template.Infrastructure.Configurations.Seed;

public static class AuthSeedConstants
{
    // WARNING: These GUIDs are placeholder examples. 
    // They are unique, static, and fixed, which is what EF Core needs.

    // --- System & Admin User IDs ---
    // User ID: System/Automation User
    public static readonly Guid SystemUserId = new("A0000000-0000-7000-8000-000000000000");

    // User ID: Initial Super Admin User
    public static readonly Guid InitialAdminUserId = new("11111111-1111-7111-8111-111111111111");

    // --- Role IDs ---
    // Role ID: SuperAdmin
    public static readonly Guid RoleIdSuperAdmin = new("22222222-2222-7222-8222-222222222222");

    // Role ID: Admin
    public static readonly Guid RoleIdAdmin = new("33333333-3333-7333-8333-333333333333");

    // Role ID: Standard User
    public static readonly Guid RoleIdUser = new("44444444-4444-7444-8444-444444444444");

    // --- Initial Permissions (Examples) ---
    // Permission ID: User.Read
    public static readonly Guid PermissionIdUserRead = new("55555555-5555-7555-8555-555555555555");

    // Permission ID: Post.Approve
    public static readonly Guid PermissionIdPostApprove = new("66666666-6666-7666-8666-666666666666");

    public static readonly DateTime SeedDateUtc = new(2025, 10, 15, 0, 0, 0, DateTimeKind.Utc);

}