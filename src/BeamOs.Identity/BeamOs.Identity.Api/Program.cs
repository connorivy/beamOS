using System.Security.Claims;
using System.Text;
using BeamOS.Common.Api;
using BeamOs.Identity.Api;
using BeamOs.Identity.Api.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder
    .Services
    .AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition(
            "oauth2",
            new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey
            }
        );
        options.OperationFilter<SecurityRequirementsOperationFilter>();
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddBeamOsEndpoints<IAssemblyMarkerIdentityApi>();

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
    })
    .AddGoogle(options =>
    {
        options.SignInScheme = IdentityConstants.ExternalScheme;
        IConfigurationSection googleAuthNSection = builder
            .Configuration
            .GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
        //options.SaveTokens = true;
    })
    .AddIdentityCookies();

builder
    .Services
    .AddAuthorizationBuilder()
    .AddPolicy("AdminAccess", policy => policy.RequireRole("Admin"))
    .AddPolicy(
        "ManagerAccess",
        policy =>
            policy.RequireAssertion(
                context => context.User.IsInRole("Admin") || context.User.IsInRole("Manager")
            )
    );

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<BeamOsIdentityDbContext>(x => x.UseSqlServer(connectionString));

//builder.Services.AddScoped<IdentityDbSeeder>();

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

app.UseAuthentication();
app.UseAuthorization();

app.AddBeamOsEndpoints<IAssemblyMarkerIdentityApi>();

//app.MapIdentityApi<BeamOsUser>();

app.MapGet("/user", (ClaimsPrincipal user) => $"Hello user {user.Identity.Name}")
    .RequireAuthorization();

app.MapGet("/admin", (ClaimsPrincipal user) => $"Hello admin {user.Identity.Name}")
    .RequireAuthorization("AdminAccess");

app.MapGet("/manager", (ClaimsPrincipal user) => $"Hello manager {user.Identity.Name}")
    .RequireAuthorization("ManagerAccess");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //using var scope = app.Services.CreateScope();
    //var seeder = scope.ServiceProvider.GetRequiredService<IdentityDbSeeder>();
    //await seeder.SeedAsync();
}

app.MapPost(
    "/PerformExternalLogin",
    async (HttpContext context) =>
    {
        //if (!await antiforgery.IsRequestValidAsync(context))
        //{
        //    return Results.Redirect("/");
        //}

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
            new() { RedirectUri = "/PerformExternalLogin2" },
            [GoogleDefaults.AuthenticationScheme]
        );
    }
);

app.MapGet(
    "/PerformExternalLogin2",
    () =>
    {
        return "hello api";
    }
);

app.UseHttpsRedirection();

app.UseCors();

app.Run();
