using BeamOS.Common.Api;
using BeamOS.Common.Application;
using BeamOS.PhysicalModel.Api;
using BeamOS.PhysicalModel.Application;
using BeamOS.PhysicalModel.Infrastructure;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder
    .Services
    .AddFastEndpoints()
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.DocumentName = "Alpha Release";
            s.Title = "beamOS api";
            s.Version = "v0";
        };
        //o.ExcludeNonFastEndpoints = true;
    });

builder
    .Services
    .AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
        });
    });

builder.Services.AddMappers<IAssemblyMarkerPhysicalModelApi>();
builder.Services.AddBeamOsEndpoints<IAssemblyMarkerPhysicalModelApi>();
builder.Services.AddCommandHandlers<IAssemblyMarkerPhysicalModelApplication>();
builder.Services.AddPhysicalModelInfrastructure();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder
    .Services
    .AddDbContext<PhysicalModelDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGroup("/api").AddBeamOsEndpoints<IAssemblyMarkerPhysicalModelApi>();

app.UseFastEndpoints(c =>
    {
        c.Endpoints.RoutePrefix = "api";
        c.Versioning.Prefix = "v";
    })
    .UseSwaggerGen();

//Configure the HTTP-request pipeline
if (app.Environment.IsDevelopment())
{
    //seed the DB
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<PhysicalModelDbContext>();
    await dbContext.SeedAsync();
}

app.UseCors();

//SwaggerBuilderExtensions.UseSwagger(app);
//app.UseSwaggerUI();

app.Run();
