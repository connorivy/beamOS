using BeamOs.Identity.Api.Entities;
using BeamOs.Identity.Api.Features.Common;

namespace BeamOs.Identity.Api;

public static class DependencyInjection
{
    public static void AddIdentityApi(this IServiceCollection services)
    {
        services.AddTransient<AccessTokenFactory>();
        services.AddTransient<AuthenticationResponseFactory>();
        services.AddTransient<LoginVerifiedUser>();
    }
}
