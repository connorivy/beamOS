using BeamOs.Identity;
using BeamOs.StructuralAnalysis;
using BeamOs.StructuralAnalysis.Sdk;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Identity;
using BeamOs.WebApp.Components.Features.UndoRedo;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using ServiceScan.SourceGenerator;

namespace BeamOs.WebApp.Components;

public static partial class DI
{
    public static IServiceCollection RegisterSharedServices<TAssembly>(
        this IServiceCollection services
    )
    {
        services.AddCommandHandlers().AddClientCommandHandlers().AddSimpleCommandHandlers();

        services.AddFluxor(options =>
            options.ScanAssemblies(typeof(DI).Assembly).AddMiddleware<HistoryMiddleware>()
        );
        services.AddMudServices();
        services.AddScoped<UndoRedoFunctionality>();
        services.AddScoped<HistoryManager>();

#pragma warning disable EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        services.AddHybridCache(options =>
            options.DefaultEntryOptions = new()
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(10),
            }
        );
#pragma warning restore EXTEXP0018 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        return services;
    }

    public static IServiceCollection RegisterSharedServicesConfigurable<TAssembly>(
        this IServiceCollection services
    )
    {
        services.AddSingleton<AuthenticationStateProvider, CustomAuthStateProvider>();
        services.AddScoped<IUserApiTokenService, ExampleUserApiTokenService>();
        services.AddScoped<IUserApiUsageService, ExampleUserApiUsageService>();
        // services.AddSingleton<UriProvider>();
        return services;
    }

    public static async Task InitializeBeamOsData(this IServiceProvider services)
    {
        // using var scope = services.CreateScope();
        // var apiClient = scope.ServiceProvider.GetRequiredService<BeamOsResultApiClient>();

        // foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        // {
        //     if (await modelBuilder.CreateOnly(apiClient))
        //     {
        //         await apiClient
        //             .Models[modelBuilder.Id]
        //             .Analyze.Opensees.RunOpenSeesAnalysisAsync(new());
        //     }
        // }
    }

    [GenerateServiceRegistrations(
        AssignableTo = typeof(CommandHandlerBase<,>),
        Lifetime = ServiceLifetime.Scoped,
        AsSelf = true
    )]
    private static partial IServiceCollection AddCommandHandlers(this IServiceCollection services);

    [GenerateServiceRegistrations(
        AssignableTo = typeof(ClientCommandHandlerBase<,>),
        Lifetime = ServiceLifetime.Scoped,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    private static partial IServiceCollection AddClientCommandHandlers(
        this IServiceCollection services
    );

    [GenerateServiceRegistrations(
        AssignableTo = typeof(SimpleCommandHandlerBase<,,>),
        Lifetime = ServiceLifetime.Scoped,
        AsSelf = true
    )]
    private static partial IServiceCollection AddSimpleCommandHandlers(
        this IServiceCollection services
    );
}

public interface IAssemblyMarkerWebAppComponents { }
