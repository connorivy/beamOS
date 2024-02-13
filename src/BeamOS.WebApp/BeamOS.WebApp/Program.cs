using BeamOS.DirectStiffnessMethod.Client;
using BeamOS.PhysicalModel.Client;
using BeamOS.WebApp;
using BeamOS.WebApp.Components;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

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

builder
    .Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddGoogle(options =>
    {
        IConfigurationSection googleAuthNSection = builder
            .Configuration
            .GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
        options.SaveTokens = true;
    })
    .AddIdentityCookies();

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
