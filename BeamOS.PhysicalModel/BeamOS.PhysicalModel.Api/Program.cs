using FastEndpoints;
using BeamOS.PhysicalModel.Application;
using BeamOS.PhysicalModel.Infrastructure;
using BeamOS.PhysicalModel.Api;
using BeamOS.PhysicalModel.Api.Data;
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

//builder.Services.AddDbContext<PhysicalModelDbContext>(options =>
//    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BeamOS;Trusted_Connection=True;TrustServerCertificate=True"));
builder.Services.AddDbContext<PhysicalModelDbContext>(options => options.UseInMemoryDatabase("PhysicalModelDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Versioning.Prefix = "v";
})
.UseSwaggerGen();

app.Run();
