using Microsoft.AspNetCore.Authorization;

namespace BeamOs.Common.Identity.Policies;

public class RequireModelReadAccess : IAuthorizationRequirement
{
    public static readonly string PolicyName = nameof(RequireModelReadAccess);
}
