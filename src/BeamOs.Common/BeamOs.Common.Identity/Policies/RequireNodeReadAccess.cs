using Microsoft.AspNetCore.Authorization;

namespace BeamOs.Common.Identity.Policies;

public class RequireNodeReadAccess : IAuthorizationRequirement
{
    public static readonly string PolicyName = nameof(RequireNodeReadAccess);
}
