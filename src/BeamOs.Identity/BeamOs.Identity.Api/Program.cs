using System.Security.Claims;
using System.Text;
using BeamOS.Common.Api;
using BeamOs.Identity.Api;
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

//builder.Services.AddScoped<IdentityDbSeeder>();

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

app.UseHttpsRedirection();

app.Run();
