namespace Shopipy.Shared;

public static class AuthorizationPolicies
{
    public const string RequireSuperAdmin = "SuperAdmin";
    public const string RequireBusinessOwnerOrSuperAdmin = "BusinessOwnerOrSuperAdmin";
    public const string RequireBusinessAccess  = "BusinessAccess";
}