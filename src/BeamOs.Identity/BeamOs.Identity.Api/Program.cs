using System.Security.Claims;
using System.Text;
using BeamOS.Common.Api;
using BeamOs.Identity.Api;
using BeamOs.Identity.Api.Entities;
using BeamOs.Identity.Api.Infrastructure;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder
//    .Services
//    .AddSwaggerGen(options =>
//    {
//        options.AddSecurityDefinition(
//            "oauth2",
//            new OpenApiSecurityScheme()
//            {
//                In = ParameterLocation.Header,
//                Name = "Authorization",
//                Type = SecuritySchemeType.ApiKey
//            }
//        );
//        options.OperationFilter<SecurityRequirementsOperationFilter>();
//    });

const string alphaRelease = "Alpha Release";
builder.Services.AddHttpContextAccessor();
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
        //o.ExcludeNonFastEndpoints = true;
    });
builder.Services.AddBeamOsEndpoints<IAssemblyMarkerIdentityApi>();
builder.Services.AddIdentityApi();

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
        //options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        IConfigurationSection googleAuthNSection = builder
            .Configuration
            .GetSection("Authentication:Google");
        options.ClientId = googleAuthNSection["ClientId"];
        options.ClientSecret = googleAuthNSection["ClientSecret"];
        options.SaveTokens = true;
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

builder
    .Services
    .AddIdentity<BeamOsUser, IdentityRole>()
    .AddEntityFrameworkStores<BeamOsIdentityDbContext>();

builder.Services.Configure<IdentityOptions>(options => options.User.RequireUniqueEmail = true);

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
app.UseFastEndpoints().UseSwaggerGen();

const string clientNs = "BeamOs.Identity.Client";
const string clientName = "IdentityAlphaClient";
const string contractsBaseNs = $"BeamOs.Identity.Api.Features";
await app.GenerateClient(
    alphaRelease,
    clientNs,
    clientName,
    [$"{contractsBaseNs}.Common", $"{contractsBaseNs}.Login", $"{contractsBaseNs}.LoginWithGoogle"]
);

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

app.UseCors();

app.Run();
