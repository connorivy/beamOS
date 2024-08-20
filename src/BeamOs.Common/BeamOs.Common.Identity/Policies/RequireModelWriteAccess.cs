using Microsoft.AspNetCore.Authorization;

namespace BeamOs.Common.Identity.Policies;

public class RequireModelWriteAccess : IAuthorizationRequirement
{
    public static readonly string PolicyName = nameof(RequireModelWriteAccess);
}
