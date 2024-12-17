using System.Data.Common;
using System.Diagnostics;
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Api.Functions;
using BeamOs.StructuralAnalysis.Application;
using BeamOs.StructuralAnalysis.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public class WebAppFactory : WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApiFunctions>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            //var dbContextDescriptor = services.SingleOrDefault(d =>
            //    d.ServiceType == typeof(DbContextOptions<StructuralAnalysisDbContext>)
            //);

            //services.Remove(dbContextDescriptor);

            //var dbConnectionDescriptor = services.SingleOrDefault(d =>
            //    d.ServiceType == typeof(DbConnection)
            //);

            //services.Remove(dbConnectionDescriptor);

            //const string connectionString =
            //    "Server=localhost,1433;Database=yourDatabaseName;Integrated Security=False;Encrypt=False;TrustServerCertificate=true;MultipleActiveResultSets=true;User=SA;Password=yourStrong(!)Password;";

            //services.AddPhysicalModelInfrastructure(connectionString);
        });
        //builder.UseEnvironment("Release");
    }
}

public class TestsInitializer
{
    public TestsInitializer() { }
}
