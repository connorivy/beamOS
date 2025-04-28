using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Ai;

public static class DI
{
    public static void AddAi(this IServiceCollection services)
    {
        services.AddScoped<GithubModelsChatCommandHandler>();
        services.AddScoped<AiApiPlugin>();
    }
}
