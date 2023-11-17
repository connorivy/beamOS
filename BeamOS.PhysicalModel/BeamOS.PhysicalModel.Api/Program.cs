using FastEndpoints;
using BeamOS.PhysicalModel.Application;
using BeamOS.PhysicalModel.Infrastructure;
using BeamOS.PhysicalModel.Api;
using Microsoft.EntityFrameworkCore;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.DocumentName = "Alpha Release";
            s.Title = "beamOS api";
            s.Version = "v0";
        };
        o.ExcludeNonFastEndpoints = true;
    });

builder.Services.AddMappers();
builder.Services.AddPhysicalModelApplication();
builder.Services.AddPhysicalModelInfrastructure();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<PhysicalModelDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly("BeamOS.PhysicalModel.Api")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Versioning.Prefix = "v";
})
.UseSwaggerGen();

app.Run();
