using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

//using Microsoft.OpenApi.Reader;

namespace BeamOs.CodeGen.ApiGenerator.ApiGenerators;

public abstract class KiotaGeneratorBase<TAssemblyMarker> : IApiGenerator
    where TAssemblyMarker : class
{
    protected virtual string Language => "csharp";
    protected abstract string DestinationPath { get; }
    public abstract string ClientName { get; }
    protected virtual string ClientNamespace { get; } = "";
    protected abstract string OpenApiDefinitionPath { get; }

    public async Task GenerateClients()
    {
        using var appFactory = new WebApplicationFactory<TAssemblyMarker>().WithWebHostBuilder(
            builder =>
            {
                builder.UseSolutionRelativeContentRoot(Environment.CurrentDirectory);

                // there are some exceptions thrown during startup of the web api.
                // they don't seem to bother the api, but they will mess up the tests, so don't validate here.
                builder.UseDefaultServiceProvider(options => options.ValidateScopes = false);
            }
        );
        HttpClient client = appFactory.CreateClient();

        var json = await client.GetStringAsync(this.OpenApiDefinitionPath);

        var tempFilePath = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFilePath, json);

            ProcessStartInfo info =
                new()
                {
                    FileName = "kiota",
                    Arguments =
                        $"generate -l {this.Language} -c {this.ClientName} -n {this.ClientNamespace} -d {tempFilePath} -o {this.DestinationPath}",
                    WorkingDirectory = Environment.CurrentDirectory,
                };

            Process generateClient = new() { StartInfo = info };
            generateClient.Start();
            await generateClient.WaitForExitAsync();
        }
        finally
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
                Console.WriteLine("Temporary file deleted.");
            }
        }
    }
}
