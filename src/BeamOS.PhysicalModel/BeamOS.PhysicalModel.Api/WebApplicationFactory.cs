namespace BeamOS.PhysicalModel.Api;

public static class WebApplicationFactory
{
    public static WebApplication Create(
        string[] args,
        Action<WebApplicationBuilder> builderConfig,
        Action<WebApplication> appConfig
    )
    {
        var builder = WebApplication.CreateBuilder(args);

        builderConfig(builder);

        var app = builder.Build();

        appConfig(app);

        return app;
    }
}
