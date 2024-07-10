using BeamOS.WebApp.Client.Caches;
using BeamOS.WebApp.Client.Components.Editor;
using BeamOS.WebApp.Client.Components.Editor.CommandHandlers;
using BeamOS.WebApp.Client.Features.KeyBindings.UndoRedo;
using BeamOS.WebApp.Client.Features.TestExplorer;
using BeamOS.WebApp.Client.Repositories;
using BeamOS.WebApp.Client.State;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using MudBlazor.Services;
using MudExtensions.Services;

namespace BeamOS.WebApp.Client;

public static class DependencyInjection
{
    public static void RegisterSharedServices<TAssembly>(this IServiceCollection services)
    {
        //_ = services.AddScoped(x => EditorApiProxy.Create(x.GetRequiredService<IJSRuntime>()));
        _ = services.AddTransient<EditorApiProxyFactory>();
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
        _ = services.AddScoped<GenericCommandHandlerWithoutHistory>();
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

    private static Type? GetInterfaceType(Type concreteType, Type interfaceType)
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
}
