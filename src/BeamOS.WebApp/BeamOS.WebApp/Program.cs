using BeamOS.WebApp;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services
    .AddRequiredWebAppServices(builder.Configuration)
    .AddConfigurableWebAppServices(builder.Configuration);

var app = builder.Build();

await app.RequiredWebApplicationConfig(app.Configuration);
app.ConfigurableWebApplicationConfig();

app.Run();

namespace BeamOS.WebApp
{
    public partial class Program { }
}
