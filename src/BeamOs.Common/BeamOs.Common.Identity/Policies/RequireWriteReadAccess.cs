using Microsoft.AspNetCore.Authorization;

namespace BeamOs.Common.Identity.Policies;

public class RequireNodeWriteAccess : IAuthorizationRequirement
{
    public static readonly string PolicyName = nameof(RequireNodeWriteAccess);
}
