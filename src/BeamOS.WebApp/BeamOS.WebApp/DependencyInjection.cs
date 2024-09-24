using BeamOs.Api;
using BeamOs.Api.Common;
using BeamOs.ApiClient;
using BeamOs.CodeGen.Apis.StructuralAnalysisApi;
using BeamOs.Common.Api;
using BeamOs.Common.Identity;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Tests.TestRunner;
using BeamOS.WebApp.Client;
using BeamOs.WebApp.Client.Components;
using BeamOs.WebApp.Client.Components.Components.Editor;
using BeamOs.WebApp.Client.Components.Features.Scratchpad;
using BeamOS.WebApp.Components;
using BeamOS.WebApp.Components.Providers;
using BeamOS.WebApp.Hubs;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace BeamOS.WebApp;

public static class DependencyInjection
{
    public static IServiceCollection AddEventServices(this IServiceCollection services)
    {
        services.AddSingleton<BeamOs.IntegrationEvents.IEventBus, StructuralAnalysisHubEventBus>();
        services.AddMediatR(
            config =>
                config.RegisterServicesFromAssemblies(
                    typeof(IAssemblyMarkerApi).Assembly,
                    typeof(IAssemblyMarkerWebApp).Assembly
                )
        );
        return services;
    }

    public static IServiceCollection AddRequiredWebServerServices(this IServiceCollection services)
    {
        services.AddSignalR();
        services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();
        services.AddHttpContextAccessor();
        services.AddCascadingAuthenticationState();
        services.AddBlazoredLocalStorage();

        _ = services.AddScoped<IStructuralAnalysisApiAlphaClient>(InProcessApiClient.Create);

        return services;
    }

    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        services.AddScoped<
            AuthenticationStateProvider,
            Components.Providers.CustomAuthStateProvider
        >();

        return services;
    }

    public static IServiceCollection AddConfigurableWebServerServices(
        this IServiceCollection services,
        ConfigurationManager configuration
    )
    {
        configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                [Constants.ASSEMBLY_NAME] = typeof(Program).Assembly.GetName().Name
            }
        );

        services.AddSingleton(typeof(IAssemblyMarkerWebAppClient).Assembly);
        services.AddSingleton<ICodeTestScoreTracker, CodeTestScoresTrackerLocal>();

        string protocol = configuration["APPLICATION_URL_PROTOCOL"] ?? "dummy value for EF Core";

        services.AddHttpClient<IApiAlphaClient, ApiAlphaClient>(
            client => client.BaseAddress = new($"{protocol}://localhost:7111")
        );

        UriProvider uriProvider = new(protocol);
        services.AddSingleton<IUriProvider>(uriProvider);
        services.AddScoped<IRenderModeProvider, RenderModeProvider>();

        return services;
    }

    public static void RequiredWebApplicationConfig(this WebApplication app)
    {
        app.MapPost(
            "/scratchpad-entity",
            async (
                [FromServices] IHubContext<ScratchpadHub, IScratchpadHubClient> hubContext,
                BeamOsEntityContractBase entity,
                [FromQuery] string connectionId
            ) =>
            {
                await hubContext.Clients.Client(connectionId).LoadEntityInViewer(entity);
            }
        );

        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.MapHub<StructuralAnalysisHub>(IStructuralAnalysisHubClient.HubEndpointPattern);
        app.MapHub<ScratchpadHub>(IScratchpadHubClient.HubEndpointPattern);

        app.UseHttpsRedirection();

        //app.UseAuthentication();
        //app.UseAuthorization();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseCors();
    }

    public static void ConfigurableWebApplicationConfig(this WebApplication app)
    {
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(
                typeof(BeamOS.WebApp.Client._Imports).Assembly,
                typeof(BeamOs.WebApp.Client.Components._Imports).Assembly
            );

        string protocol =
            app.Configuration["APPLICATION_URL_PROTOCOL"] ?? "dummy value for EF Core";
        app.MapGet(
            "/app-settings",
            () =>
                Results.Ok(
                    new Dictionary<string, string>
                    {
                        [Constants.ASSEMBLY_NAME] = typeof(Program).Assembly.GetName().Name,
                        [Constants.PHYSICAL_MODEL_API_BASE_URI] = "https://localhost:7193",
                        [Constants.DSM_API_BASE_URI] = "https://localhost:7110",
                        [Constants.ANALYSIS_API_BASE_URI] = $"{protocol}://localhost:7111"
                    }
                )
        );
    }
}
