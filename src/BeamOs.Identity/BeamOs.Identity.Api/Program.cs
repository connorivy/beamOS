using System.Security.Claims;
using System.Text;
using BeamOs.Identity.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
#pragma warning disable CA5404 // Do not disable token validation checks
        x.TokenValidationParameters = new()
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey")),
            ValidateIssuerSigningKey = true,
            ValidateAudience = false,
            ValidateIssuer = false,
        };
#pragma warning restore CA5404 // Do not disable token validation checks
    });

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

builder.Services.AddScoped<IdentityDbSeeder>();

var app = builder.Build();

app.MapIdentityApi<BeamOsUser>();

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

    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IdentityDbSeeder>();
    await seeder.SeedAsync();
}

app.UseHttpsRedirection();

app.Run();
