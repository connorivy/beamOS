using System.Collections;
using System.Reflection;
using BeamOS.Common.Api;
using BeamOS.Common.Application;
using BeamOS.IntegrationTests.DirectStiffnessMethod.Common.Fixtures.SolvedProblems.MatrixAnalysisOfStructures2ndEd;
using BeamOS.PhysicalModel.Api;
using BeamOS.PhysicalModel.Application;
using BeamOS.PhysicalModel.Client;
using BeamOS.PhysicalModel.Contracts.Common;
using BeamOS.PhysicalModel.Infrastructure;
using FastEndpoints;
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
        var keepAliveConnection = new SqliteConnection("DataSource=:memory:");
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

                keepAliveConnection.Open();

                builder
                    .Services
                    .AddScoped<PhysicalModelDbContext>(
                        x => DbContextFactory.Create(keepAliveConnection)
                    );
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

        await Example8_4.CreatePhysicalModel(client);

        var modelResponse = await client.GetModelHydratedAsync(Example8_4.ModelId);

        ContractComparer.AssertContractsEqual(modelResponse, Example8_4.GetExpectedResponse());

        await app.StopAsync();
        keepAliveConnection.Close();
    }
}

public class DbContextFactory
{
    public static PhysicalModelDbContext Create(SqliteConnection sqliteConnection)
    {
        var builder = new DbContextOptionsBuilder<PhysicalModelDbContext>();
        _ = builder.UseSqlite(sqliteConnection);

        using (var ctx = new PhysicalModelDbContext(builder.Options))
        {
            _ = ctx.Database.EnsureCreated();
        }
        return new(builder.Options);
    }
}

public static class ContractComparer
{
    public static void AssertContractsEqual(BeamOsContractBase first, BeamOsContractBase second)
    {
        foreach (
            PropertyInfo propertyInfo in first
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        )
        {
            object? firstValue = propertyInfo.GetValue(first, null);
            object? secondValue = propertyInfo.GetValue(second, null);

            AssertContractValuesAreEqual(firstValue, secondValue);
        }
    }

    private static void AssertContractValuesAreEqual(object? first, object? second)
    {
        if (first == null && second == null)
        {
            return;
        }
        if (first == null || second == null)
        {
            throw new ArgumentNullException($"Value of first: {first}, \nValue of second {second}");
        }
        if (first.GetType() != second.GetType())
        {
            throw new ArgumentException(
                $"Value and type of first: {first} {first.GetType()}, Value and type of second {second} {second.GetType()}"
            );
        }

        if (
            first is BeamOsContractBase firstContract
            && second is BeamOsContractBase secondContract
        )
        {
            AssertContractsEqual(firstContract, secondContract);
        }

        if (first is IList firstList && second is IList secondList)
        {
            if (firstList.Count != secondList.Count)
            {
                throw new ArgumentException(
                    $"First is a list with count {firstList.Count} and second is a list with count {secondList.Count}"
                );
            }
            for (int i = 0; i < firstList.Count; i++)
            {
                AssertContractValuesAreEqual(firstList[i], secondList[i]);
            }
            return;
        }

        if (!object.Equals(first, second))
        {
            throw new ArgumentException(
                $"First info: \n{first.GetType()}\n{first}\n\nSecond Info: \n{second}"
            );
        }
    }
}
