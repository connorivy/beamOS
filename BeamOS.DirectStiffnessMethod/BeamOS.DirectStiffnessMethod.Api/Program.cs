using BeamOS.Common.Api;
using BeamOS.DirectStiffnessMethod.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBeamOsEndpoints<PhysicalModelApiClient>();
builder.Services.AddHttpClient<PhysicalModelApiClient>(client => client.BaseAddress = new("https://localhost:7193"));

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddBeamOsEndpoints<PhysicalModelApiClient>();

app.Run();
