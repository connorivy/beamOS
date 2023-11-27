using BeamOS.Common.Api;
using BeamOS.DirectStiffnessMethod.Api;
using BeamOS.DirectStiffnessMethod.Application;
using BeamOS.Common.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMappers<IAssemblyMarkerDirectStiffnessMethodApi>();
builder.Services.AddBeamOsEndpoints<IAssemblyMarkerDirectStiffnessMethodApi>();
builder.Services.AddCommandHandlers<IAssemblyMarkerDirectStiffnessMethodApplication>();
builder.Services.AddHttpClient<PhysicalModelApiClient>(client => client.BaseAddress = new("https://localhost:7193"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroup("/api").AddBeamOsEndpoints<IAssemblyMarkerDirectStiffnessMethodApi>();

app.Run();
