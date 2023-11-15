using System.Reflection;
using BeamOS.Client.Pages;
using BeamOS.Components;
using BeamOS.Data;
using BeamOS.PhysicalModel.Api;

//using BeamOS.DirectStiffnessMethod.Api;
using BeamOS.PhysicalModel.Api.Models.Endpoints;
using BeamOS.PhysicalModel.Application;
using BeamOS.PhysicalModel.Application.Models.Commands;
using BeamOS.PhysicalModel.Infrastructure;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddFastEndpoints(o => o.Assemblies = new List<Assembly>
{
    typeof(Program).Assembly,
    //typeof(MyEndpoint).Assembly,
    typeof(CreateModelEndpoint).Assembly,
    typeof(CreateModelCommand).Assembly,
});
//builder.Services
//    .SwaggerDocument(o =>
//    {
//        o.DocumentSettings = s =>
//        {
//            s.DocumentName = "Alpha Release";
//            s.Title = "beamOS api";
//            s.Version = "v0";
//        };
//        o.ExcludeNonFastEndpoints = true;
//    });

builder.Services.AddPhysicalModelApi();
builder.Services.AddPhysicalModelApplication();
builder.Services.AddPhysicalModelInfrastructure();

builder.Services.AddDbContext<BeamOsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<BeamOsUser>()
    .AddEntityFrameworkStores<BeamOsDbContext>();

var app = builder.Build();

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

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "api";
    c.Versioning.Prefix = "v";
});

SwaggerBuilderExtensions.UseSwagger(app);
app.UseSwaggerUI();
//app.UseSwaggerGen();

app.MapGroup("/auth").MapIdentityApi<BeamOsUser>();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
