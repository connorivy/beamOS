using System.Diagnostics;
using System.Text;
using BeamOs.Api;
using BeamOs.Api.Common;
using BeamOs.ApiClient;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Infrastructure;
using BeamOs.Tests.TestRunner;
using BeamOS.WebApp;
using BeamOS.WebApp.Client;
using BeamOS.WebApp.Client.Components.Editor;
using BeamOS.WebApp.Client.Features.Scratchpad;
using BeamOS.WebApp.Components;
using BeamOS.WebApp.Components.Providers;
using BeamOS.WebApp.Hubs;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

if (builder.Configuration["APPLICATION_URL_PROTOCOL"] is null)
{
    // ef core design run. define dummy variables
    builder.Configuration["APPLICATION_URL_PROTOCOL"] = "dummy";
}

builder
    .Configuration
    .AddInMemoryCollection(
        new Dictionary<string, string?>
        {
            [Constants.ASSEMBLY_NAME] = typeof(Program).Assembly.GetName().Name
        }
    );

builder.Services.AddSignalR();

// Add services to the container.
builder
    .Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder
    .Services
    .AddAnalysisEndpoints()
    .AddAnalysisEndpointServices()
    .AddAnalysisEndpointOptions()
    .AddAnalysisInfrastructure(builder.Configuration);

string protocol =
    builder.Configuration["APPLICATION_URL_PROTOCOL"]
    ?? throw new Exception("Unable to find protocol in application configuration");

builder
    .Services
    .AddHttpClient<IApiAlphaClient, ApiAlphaClient>(
        client => client.BaseAddress = new($"{protocol}://localhost:7111")
    );

UriProvider uriProvider = new(protocol);
builder.Services.AddSingleton<IUriProvider>(uriProvider);
builder.Services.AddSingleton<ICodeTestScoreTracker, CodeTestScoresTracker>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<BeamOs.IntegrationEvents.IEventBus, StructuralAnalysisHubEventBus>();
builder.Services.AddScoped<IRenderModeProvider, RenderModeProvider>();

builder
    .Services
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
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"])
            ),
            ValidateAudience = true,
            ValidAudiences = builder.Configuration["JwtSettings:Audiences"].Split(','),
            ValidateLifetime = true,
        };
    });

// workaround to make link redirection work in .net 8 with JWT auth
// see this issue and comment https://github.com/dotnet/aspnetcore/issues/52063#issuecomment-1817420640
builder
    .Services
    .AddSingleton<
        IAuthorizationMiddlewareResultHandler,
        BlazorAuthorizationMiddlewareResultHandler
    >();

builder.Services.RegisterSharedServices<Program>();

var app = builder.Build();

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
        Trace.WriteLine($"message with connectionId {connectionId}");
        await hubContext.Clients.Client(connectionId).LoadEntityInViewer(entity);
    }
);

app.AddAnalysisEndpoints();

//seed the DB
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BeamOsStructuralDbContext>();
    await dbContext.SeedAsync();
}

//app.Use(
//    async (context, next) =>
//    {
//        // hard code user bearer token for auth
//        context.Request.Headers.Authorization =
//            "Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ1c2VyQGVtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJVc2VyIiwiYXVkIjpbImh0dHBzOi8vbG9jYWxob3N0OjcxOTMiLCJodHRwczovL2xvY2FsaG9zdDo3MTk0Il0sImV4cCI6NDg3MDA5MDM2MCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE5NCJ9.CTW9SNJl4kSvAlJGZ7dDpFsCc-6hVeOu6OhJFllzGwkE2FZwBd34i7q8nIaKQDQf3T8-O-GqyF7Jbey2ULDPOA";

//        await next(context);
//    }
//);

// Configure the HTTP request pipeline.
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

app.UseStatusCodePagesWithRedirects("/404");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BeamOS.WebApp.Client._Imports).Assembly);

app.UseCors();

app.Run();

namespace BeamOS.WebApp
{
    public partial class Program { }
}
