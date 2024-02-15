using System.Text;
using BeamOS.DirectStiffnessMethod.Client;
using BeamOS.PhysicalModel.Client;
using BeamOS.WebApp;
using BeamOS.WebApp.Components;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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

builder
    .Services
    .AddHttpClient<IPhysicalModelAlphaClient, PhysicalModelAlphaClient>(
        client => client.BaseAddress = new("https://localhost:7193")
    );

builder
    .Services
    .AddHttpClient<IDirectStiffnessMethodAlphaClient, DirectStiffnessMethodAlphaClient>(
        client => client.BaseAddress = new("https://localhost:7110")
    );

builder.Services.AddCascadingAuthenticationState();
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
                [Constants.DSM_API_BASE_URI] = "https://localhost:7110"
            }
        )
);

app.Use(
    async (context, next) =>
    {
        var token = context.Request.Cookies["Authorization"];
        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Add("Authorization", "Bearer " + token);
        }
        await next();
    }
);

//var accountGroup = app.MapGroup("/Account");

//accountGroup.MapPost(
//    "/PerformExternalLogin",
//    () =>
//    {
//        //IEnumerable<KeyValuePair<string, StringValues>> query = [
//        //    new("ReturnUrl", returnUrl),
//        //                new("Action", "LoginCallbackAction")];

//        //var redirectUrl = UriHelper.BuildRelative(
//        //    context.Request.PathBase,
//        //    "/Account/ExternalLogin",
//        //    QueryString.Create(query));

//        //var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
//        //return TypedResults.Challenge(properties, [provider]);
//        return Results.Challenge(new() { RedirectUri = "" }, [GoogleDefaults.AuthenticationScheme]);
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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode();

//.AddAdditionalAssemblies(typeof(BeamOS.WebApp.Client.Pages.Editor).Assembly);

var accountGroup = app.MapGroup("/Account");
accountGroup.MapPost(
    "/PerformExternalLogin",
    (HttpContext context) =>
    {
        //IEnumerable<KeyValuePair<string, StringValues>> query = [
        //    new("ReturnUrl", returnUrl),
        //                new("Action", ExternalLogin.LoginCallbackAction)];

        //var redirectUrl = UriHelper.BuildRelative(
        //    context.Request.PathBase,
        //    "/Account/ExternalLogin",
        //    QueryString.Create(query));

        //var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //return TypedResults.Challenge(properties, [provider]);
        return Results.Challenge(
            new() { RedirectUri = "https://localhost:7194/PerformExternalLogin" },
            [GoogleDefaults.AuthenticationScheme]
        );
    }
);

app.MapGet(
    "/signin-google",
    (string? state, string? code, string? scope, string? authuser, string? prompt) =>
    {
        ;
    }
);

app.UseCors();

app.Run();
