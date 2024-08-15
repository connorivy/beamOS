using Microsoft.AspNetCore.Authorization;

namespace BeamOs.Common.Identity.Policies;

public class RequireModelOwnerAccess : IAuthorizationRequirement
{
    public static readonly string PolicyName = nameof(RequireModelOwnerAccess);
}
