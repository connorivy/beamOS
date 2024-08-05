using System.Text;
using BeamOs.Api;
using BeamOs.Api.Common;
using BeamOs.ApiClient;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Infrastructure;
using BeamOs.Tests.TestRunner;
using BeamOS.WebApp.Client;
using BeamOS.WebApp.Client.Components.Editor;
using BeamOS.WebApp.Client.Features.Scratchpad;
using BeamOS.WebApp.Components;
using BeamOS.WebApp.Components.Providers;
using BeamOS.WebApp.Hubs;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

namespace BeamOS.WebApp;

public static class DependencyInjection
{
    public static IServiceCollection AddRequiredWebAppServices(
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
        services.AddSignalR();
        services
            .AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        services.AddRequiredAnalysisServices();

        services.AddSingleton<ICodeTestScoreTracker, CodeTestScoresTracker>();
        services.AddHttpContextAccessor();
        services.AddCascadingAuthenticationState();
        services.AddBlazoredLocalStorage();
        services.AddSingleton<BeamOs.IntegrationEvents.IEventBus, StructuralAnalysisHubEventBus>();

        services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])
                    ),
                    ValidateAudience = true,
                    ValidAudiences = configuration["JwtSettings:Audiences"].Split(','),
                    ValidateLifetime = true,
                };
            });

        // workaround to make link redirection work in .net 8 with JWT auth
        // see this issue and comment https://github.com/dotnet/aspnetcore/issues/52063#issuecomment-1817420640
        //builder
        //    .Services
        //    .AddSingleton<
        //        IAuthorizationMiddlewareResultHandler,
        //        BlazorAuthorizationMiddlewareResultHandler
        //    >();

        services.RegisterSharedServices<Program>();

        return services;
    }

    public static IServiceCollection AddConfigurableWebAppServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddAnalysisEndpointOptions().AddAnalysisDb(configuration);

        string protocol = configuration["APPLICATION_URL_PROTOCOL"] ?? "dummy value for EF Core";

        services.AddHttpClient<IApiAlphaClient, ApiAlphaClient>(
            client => client.BaseAddress = new($"{protocol}://localhost:7111")
        );

        UriProvider uriProvider = new(protocol);
        services.AddSingleton<IUriProvider>(uriProvider);
        services.AddScoped<IRenderModeProvider, RenderModeProvider>();

        return services;
    }

    public static async Task RequiredWebApplicationConfig(
        this WebApplication app,
        IConfiguration configuration
    )
    {
        string protocol = configuration["APPLICATION_URL_PROTOCOL"] ?? "dummy value for EF Core";
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

        app.AddAnalysisEndpoints();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<BeamOsStructuralDbContext>();
            _ = dbContext.Database.EnsureCreated();

            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                await dbContext.SeedAsync();
            }
            else
            {
                app.UseExceptionHandler("/Error", createScopeForErrors: true);
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
        }

        app.MapHub<StructuralAnalysisHub>(IStructuralAnalysisHubClient.HubEndpointPattern);
        app.MapHub<ScratchpadHub>(IScratchpadHubClient.HubEndpointPattern);

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseAntiforgery();

        app.UseCors();
    }

    public static void ConfigurableWebApplicationConfig(this WebApplication app)
    {
        app.Use(
            async (context, next) =>
            {
                // hard code user bearer token for auth
                if (
                    !context.Request.Headers.ContainsKey("Authorization")
                    || !context.Request.Headers["Authorization"][0].StartsWith("Bearer ")
                )
                {
                    context.Request.Headers.Authorization =
                        "Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ1c2VyQGVtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJVc2VyIiwiYXVkIjpbImh0dHBzOi8vbG9jYWxob3N0OjcxOTMiLCJodHRwczovL2xvY2FsaG9zdDo3MTk0Il0sImV4cCI6NDg3MDA5MDM2MCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE5NCJ9.CTW9SNJl4kSvAlJGZ7dDpFsCc-6hVeOu6OhJFllzGwkE2FZwBd34i7q8nIaKQDQf3T8-O-GqyF7Jbey2ULDPOA";
                }

                await next(context);
            }
        );

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(BeamOS.WebApp.Client._Imports).Assembly);
    }
}
