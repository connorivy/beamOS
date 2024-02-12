using Microsoft.AspNetCore.Identity;

namespace BeamOs.Identity.Api.Entities;

public class BeamOsUser : IdentityUser
{
    public RefreshToken RefreshToken { get; set; } = RefreshToken.Default;
}
