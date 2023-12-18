using BeamOS.PhysicalModel.Client;
using BeamOS.WebApp.Client;
using BeamOS.WebApp.Client.Pages;
using BeamOS.WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder
    .Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder
    .Services
    .AddHttpClient<IPhysicalModelAlphaClient, PhysicalModelAlphaClient>(
        client => client.BaseAddress = new("https://localhost:7193")
    );

builder.Services.AddScoped<IPhysicalModelAlphaClientWithEditor, PhysicalModelAlphaClientProxy>();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Editor).Assembly);

app.UseCors();

app.Run();
