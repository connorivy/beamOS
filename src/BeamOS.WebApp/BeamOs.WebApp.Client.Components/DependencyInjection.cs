using BeamOs.Application.Common;
using BeamOs.Common.Identity;
using BeamOS.Tests.Common.Fixtures.Mappers;
using BeamOS.Tests.Common.Interfaces;
using BeamOs.WebApp.Client.Components.Caches;
using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;
using BeamOs.WebApp.Client.Components.Features.KeyBindings.UndoRedo;
using BeamOs.WebApp.Client.Components.Features.Styles;
using BeamOs.WebApp.Client.Components.Features.TestExplorer;
using BeamOs.WebApp.Client.Components.Repositories;
using BeamOs.WebApp.Client.Components.State;
using Fluxor;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudExtensions.Services;
#if DEBUG
using Fluxor.Blazor.Web.ReduxDevTools;
#endif

namespace BeamOs.WebApp.Client.Components;

public static class DependencyInjection
{
    public static IServiceCollection RegisterSharedServices<TAssembly>(
        this IServiceCollection services
    )
    {
        _ = services.AddTransient<IEditorApiProxyFactory, EditorApiProxyFactory>();
        _ = services.AddTransient<EditorEventsApi>();
        _ = services.AddMudServices().AddMudExtensions();

        _ = services.AddFluxor(o =>
        {
            o.ScanAssemblies(typeof(TAssembly).Assembly).AddMiddleware<UndoRedoMiddleware>();

            if (typeof(TAssembly) != typeof(DependencyInjection))
            {
                o.ScanAssemblies(typeof(DependencyInjection).Assembly);
            }
#if DEBUG
            o.UseReduxDevTools();
#endif
        });

        _ = services.AddSingleton<TestInfoProvider>();
        _ = services.AddSingleton<TestInfoStateProvider>();
        _ = services.AddScoped<HistoryManager>();
        _ = services.AddScoped<UndoRedoFunctionality>();

        // caches and repositories
        _ = services.AddScoped<AllStructuralAnalysisModelCaches>();
        _ = services.AddScoped<EditorApiRepository>();
        _ = services.AddScoped<ModelIdRepository>();
        _ = services.AddScoped<
            IStateRepository<EditorComponentState>,
            GenericComponentStateRepository<EditorComponentState>
        >();
        _ = services.AddScoped<ChangeComponentStateCommandHandler<EditorComponentState>>();

        _ = services.AddCommandHandlers();
        _ = services.AddScoped<GenericCommandHandler>();

        _ = services.AddScoped<TestFixtureDisplayer>();
        _ = services.AddScoped<IsDarkModeProvider>();

        _ = services.AddTestServices();

        _ = services.AddTransient<IAccountService, AccountService>();

        return services;
    }

    public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
    {
        Type[] assemblyTypes = typeof(DependencyInjection)
            .Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .ToArray();

        foreach (var assemblyType in assemblyTypes)
        {
            if (assemblyType.IsGenericType)
            {
                continue;
            }

            if (
                GetInterfaceType(assemblyType, typeof(IClientCommandHandler<>))
                is Type clientCommandHandler
            )
            {
                _ = services.AddScoped(assemblyType);
                _ = services.AddScoped(clientCommandHandler, assemblyType);
            }
        }

        return services;
    }

    public static Type? GetInterfaceType(Type concreteType, Type interfaceType)
    {
        bool isGeneric = interfaceType.IsGenericType;
        foreach (var inter in concreteType.GetInterfaces())
        {
            if (
                isGeneric
                && inter.IsGenericType
                && inter.GetGenericTypeDefinition() == interfaceType
            )
            {
                return inter;
            }
            else if (!isGeneric && inter == interfaceType)
            {
                return inter;
            }
        }
        return null;
    }

    private static Type? GetBaseType(Type concreteType, Type baseType)
    {
        if (concreteType.BaseType == typeof(object) || concreteType.BaseType is null)
        {
            return null;
        }

        bool isGeneric = baseType.IsGenericType;

        if (
            isGeneric
            && concreteType.BaseType.IsGenericType
            && concreteType.BaseType.GetGenericTypeDefinition() == baseType
        )
        {
            return baseType;
        }
        else if (!isGeneric && concreteType.BaseType == baseType)
        {
            return baseType;
        }
        return GetBaseType(concreteType.BaseType, baseType);
    }

    public static IServiceCollection AddTestServices(this IServiceCollection services)
    {
        return services.AddServicesAsType<IHasSourceInfo>(
            typeof(IFixtureMapper),
            Application.Common.ServiceLifetime.Singleton
        );
    }
}
