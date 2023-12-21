using BeamOS.PhysicalModel.Client;

namespace BeamOS.WebApp.Client;

public class RuntimeConfig
{
    static RuntimeConfig() { }

    private RuntimeConfig() { }

    public static RuntimeConfig Instance { get; } = new RuntimeConfig();

    public List<IConfigurationSource> ConfigurationSources { get; } = [];
}

public sealed class RuntimeConfig2
{
    static RuntimeConfig2() { }

    private RuntimeConfig2() { }

    public static IServiceProvider? ServiceProvider { get; set; }
}

public static class IServiceCollectionExtension
{
    public static void RegisterServerClientServices(this IServiceCollection serviceCollection)
    {
        if (RuntimeConfig2.ServiceProvider is null)
        {
            _ = serviceCollection.AddHttpClient<
                IPhysicalModelAlphaClient,
                PhysicalModelAlphaClient
            >(client => client.BaseAddress = new("https://localhost:7193"));
        }
        else
        {
            _ = serviceCollection.AddScoped(
                client =>
                    RuntimeConfig2.ServiceProvider.GetRequiredService<IPhysicalModelAlphaClient>()
            );
        }
        _ = serviceCollection.AddScoped<
            IPhysicalModelAlphaClientWithEditor,
            PhysicalModelAlphaClientProxy
        >();
    }
}

public class config : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        throw new NotImplementedException();
}

// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;

public class PhysicalModelUriStringConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new PhysicalModelUriStringConfigurationProvider();
    }
}

// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Configuration;

public class PhysicalModelUriStringConfigurationProvider : ConfigurationProvider
{
    public override void Load()
    {
        this.Data.Add("PhysicalModelApiBaseUriString", "https://localhost:7193");
    }
}
