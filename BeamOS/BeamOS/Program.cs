using System.Reflection;
using BeamOS.Client.Pages;
using BeamOS.Components;
using BeamOS.DirectStiffnessMethod.Api;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddFastEndpoints(o => o.Assemblies = new List<Assembly>
{
    typeof(Program).Assembly,
    typeof(MyEndpoint).Assembly,
});
//builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o => o.ExcludeNonFastEndpoints = true);

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

app.UseFastEndpoints();
app.UseSwaggerGen();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Counter).Assembly);

app.Run();
