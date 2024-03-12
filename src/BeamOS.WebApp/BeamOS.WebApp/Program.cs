using System.Text;
using BeamOs.Api;
using BeamOs.ApiClient;
using BeamOS.Common.Api;
using BeamOs.Identity.Client;
using BeamOs.Infrastructure;
using BeamOS.WebApp;
using BeamOS.WebApp.Client;
using BeamOS.WebApp.Components;
using Blazored.LocalStorage;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder
    .Configuration
    .AddInMemoryCollection(
        new Dictionary<string, string?>
        {
            [Constants.ASSEMBLY_NAME] = typeof(Program).Assembly.GetName().Name
        }
    );

// Add services to the container.
builder
    .Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

const string alphaRelease = "Alpha Release";
builder
    .Services
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.DocumentName = alphaRelease;
            s.Title = "beamOS api";
            s.Version = "v0";
            s.SchemaSettings.SchemaProcessors.Add(new MarkAsRequiredIfNonNullableSchemaProcessor());
        };
        o.ShortSchemaNames = true;
        o.ExcludeNonFastEndpoints = true;
        o.EndpointFilter = ed => ed.EndpointType.Assembly == typeof(IAssemblyMarkerApi).Assembly;
    });

//builder
//    .Services
//    .AddHttpClient<IPhysicalModelAlphaClient, PhysicalModelAlphaClient>(
//        client => client.BaseAddress = new("https://localhost:7193")
//    );

//builder
//    .Services
//    .AddHttpClient<IDirectStiffnessMethodAlphaClient, DirectStiffnessMethodAlphaClient>(
//        client => client.BaseAddress = new("https://localhost:7110")
//    );

builder
    .Services
    .AddHttpClient<IIdentityAlphaClient, IdentityAlphaClient>(
        client => client.BaseAddress = new("https://localhost:7194")
    );

builder
    .Services
    .AddHttpClient<IApiAlphaClient, ApiAlphaClient>(
        client => client.BaseAddress = new("https://localhost:7111")
    );

builder.Services.AddAnalysisApiServices();
var connectionString =
    builder.Configuration.GetConnectionString("AnalysisDbConnection")
    ?? throw new InvalidOperationException("Connection string 'AnalysisDbConnection' not found.");
builder
    .Services
    .AddDbContext<BeamOsStructuralDbContext>(options => options.UseSqlServer(connectionString));

UriProvider uriProvider = new("https");
builder.Services.AddSingleton<IUriProvider>(uriProvider);
builder.Services.AddHttpContextAccessor();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddBlazoredLocalStorage();

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

builder.Services.RegisterSharedServices();

builder
    .Services
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });

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
                [Constants.ANALYSIS_API_BASE_URI] = "https://localhost:7111"
            }
        )
);

app.AddBeamOsEndpointsForAnalysis();

//seed the DB
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<BeamOsStructuralDbContext>();
await dbContext.SeedAsync();

app.Use(
    async (context, next) =>
    {
        var token = context.Request.Cookies[CommonApiConstants.ACCESS_TOKEN_GUID];
        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Authorization = $"Bearer {token}";
        }
        await next(context);
    }
);

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

app.UseStatusCodePagesWithRedirects("/404");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BeamOS.WebApp.Client._Imports).Assembly);

app.UseCors();

app.Run();
