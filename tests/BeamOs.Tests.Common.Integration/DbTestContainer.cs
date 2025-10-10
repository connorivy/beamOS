using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;

namespace BeamOs.Tests.Common.Integration;

public static class DbTestContainer
{
    public static INetwork DockerNetwork { get; }
    public static PostgreSqlContainer DbContainer { get; }

    static DbTestContainer()
    {
        DockerNetwork = new NetworkBuilder().WithName(Guid.NewGuid().ToString("D")).Build();

        DbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithPortBinding(5432, true)
            // .WithExposedPort(5432)
            .WithNetwork(DockerNetwork)
            .WithNetworkAliases("BeamOsTestNetwork")
            .Build();
    }

    private static readonly SemaphoreSlim semaphore = new(1, 1);

    public static async Task InitializeAsync()
    {
        if (DbContainer.State == TestcontainersStates.Exited)
        {
            throw new InvalidOperationException("Database has already exited unexpectedly.");
        }

        if (DbContainer.State == TestcontainersStates.Running)
        {
            return;
        }

        await semaphore.WaitAsync();
        try
        {
            if (DbContainer.State != TestcontainersStates.Running)
            {
                await DockerNetwork.CreateAsync();
                await DbContainer.StartAsync();
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    public static string GetConnectionString()
    {
        if (DbContainer.State != TestcontainersStates.Running)
        {
            throw new InvalidOperationException(
                $"Database has unexpected state, {DbContainer.State}."
            );
        }

        return $"{DbContainer.GetConnectionString()};Include Error Detail=True";
    }
}
