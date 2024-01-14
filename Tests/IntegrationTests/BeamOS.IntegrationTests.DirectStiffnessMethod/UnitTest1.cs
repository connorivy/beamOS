using BeamOS.Common.Api;
using BeamOS.Common.Application;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;
using BeamOS.PhysicalModel.Api;
using BeamOS.PhysicalModel.Application;
using BeamOS.PhysicalModel.Client;
using BeamOS.PhysicalModel.Infrastructure;
using FastEndpoints;
using FastEndpoints.Swagger;
using MathNet.Numerics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        WebApplication app = WebApplicationFactory.Create(
            [],
            builder =>
            {
                builder.Services.AddFastEndpoints();

                builder
                    .Services
                    .AddCors(options =>
                    {
                        options.AddDefaultPolicy(policy =>
                        {
                            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                        });
                    });

                builder.Services.AddMappers<IAssemblyMarkerPhysicalModelApi>();
                builder.Services.AddBeamOsEndpoints<IAssemblyMarkerPhysicalModelApi>();
                builder.Services.AddCommandHandlers<IAssemblyMarkerPhysicalModelApplication>();
                builder.Services.AddPhysicalModelInfrastructure();

                var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
                keepAliveConnection.Open();

                builder
                    .Services
                    .AddDbContext<PhysicalModelDbContext>(options =>
                    {
                        options.UseSqlite(keepAliveConnection);
                    });
            },
            app =>
            {
                app.MapGroup("/api").AddBeamOsEndpoints<IAssemblyMarkerPhysicalModelApi>();

                app.UseFastEndpoints(c =>
                {
                    c.Endpoints.RoutePrefix = "api";
                    c.Versioning.Prefix = "v";
                    c.Endpoints.ShortNames = true;
                });

                app.UseCors();
            }
        );

        await app.StartAsync();

        var example8_4 = new Example8_4();

        var httpClient = new HttpClient() { BaseAddress = new(app.Urls.First()) };
        var client = new PhysicalModelAlphaClient(httpClient);

        foreach (var node in example8_4.Nodes)
        {
            await client.CreateNodeAsync()
        }


        var x = await client.GetModelHydratedAsync("");

        await app.StopAsync();
    }
}

public class TestDataContextFactory
{
    public TestDataContextFactory()
    {
        var builder = new DbContextOptionsBuilder<PhysicalModelDbContext>();
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        _ = builder.UseSqlite(connection);

        using (var ctx = new PhysicalModelDbContext(builder.Options))
        {
            _ = ctx.Database.EnsureCreated();
        }

        this.options = builder.Options;
    }

    private readonly DbContextOptions<PhysicalModelDbContext> options;

    public PhysicalModelDbContext Create() => new(this.options);
}
