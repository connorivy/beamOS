using BeamOs.Identity.Api.Entities;

namespace BeamOs.Identity.Api;

public static class DependencyInjection
{
    public static void AddIdentityApi(this IServiceCollection services)
    {
        services.AddTransient<AccessTokenFactory>();
        services.AddTransient<AuthenticationResponseFactory>();
    }
}
