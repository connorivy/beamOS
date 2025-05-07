using BeamOs.CodeGen.StructuralAnalysisApiClient;
using BeamOs.Common.Api;
using BeamOs.Common.Application;
using BeamOs.Identity;
using BeamOs.Tests.Common;
using BeamOs.WebApp.Components.Features.Common;
using BeamOs.WebApp.Components.Features.Identity;
using BeamOs.WebApp.Components.Features.UndoRedo;
using Fluxor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace BeamOs.WebApp.Components;

public static class DI
{
    public static IServiceCollection RegisterSharedServices<TAssembly>(
        this IServiceCollection services
    )
    {
        services.AddObjectThatExtendsBase<IAssemblyMarkerWebAppComponents>(
            typeof(CommandHandlerBase<,>),
            ServiceLifetime.Scoped
        );
        services.AddObjectThatExtendsBase<IAssemblyMarkerWebAppComponents>(
            typeof(ClientCommandHandlerBase<,>),
            ServiceLifetime.Scoped
        );
        services.AddObjectThatExtendsBase<IAssemblyMarkerWebAppComponents>(
            typeof(SimpleCommandHandlerBase<,,>),
            ServiceLifetime.Scoped
        );

        services.AddObjectThatImplementInterface<IAssemblyMarkerWebAppComponents>(
            typeof(IClientCommandHandler<>),
            ServiceLifetime.Scoped,
            true
        );

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
        services.AddSingleton<UriProvider>();
        return services;
    }

    public static async Task InitializeBeamOsData(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var apiClient = scope.ServiceProvider.GetRequiredService<IStructuralAnalysisApiClientV1>();

        foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        {
            if (await modelBuilder.CreateOnly(apiClient))
            {
                await apiClient.RunDirectStiffnessMethodAsync(modelBuilder.Id, new());
            }
        }
    }
}

public interface IAssemblyMarkerWebAppComponents { }
