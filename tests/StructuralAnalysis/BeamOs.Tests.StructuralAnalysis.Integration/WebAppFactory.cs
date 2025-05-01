#if !RUNTIME
using BeamOs.StructuralAnalysis.Api;
using BeamOs.StructuralAnalysis.Infrastructure;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace BeamOs.Tests.StructuralAnalysis.Integration;

public class WebAppFactory(string connectionString, TimeProvider? timeProvider = null)
    : WebApplicationFactory<IAssemblyMarkerStructuralAnalysisApi>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.Single(d =>
                d.ServiceType == typeof(DbContextOptions<StructuralAnalysisDbContext>)
            );

            services.Remove(dbContextDescriptor);

            // remove existing interceptor because it doesn't get the connection string from the container,
            // so it will continue to to go to the old db if we don't remove it
            var existingInterceptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(IDbContextOptionsConfiguration<StructuralAnalysisDbContext>)
            );

            services.Remove(existingInterceptor);

            //services.AddDbContext<StructuralAnalysisDbContext>(options =>
            //{
            //    var optionsBuilderNoInterceptor =
            //        options.UseSqlServer(connectionString).Options
            //        as DbContextOptions<StructuralAnalysisDbContext>;

            //    options.AddInterceptors(new IdentityInsertInterceptor(optionsBuilderNoInterceptor));
            //});

            services.AddDbContext<StructuralAnalysisDbContext>(options =>
                options
                    .UseNpgsql(connectionString)
                    .AddInterceptors(
                        new ModelEntityIdIncrementingInterceptor(),
                        new ModelLastModifiedUpdater(timeProvider ?? TimeProvider.System)
                    )
            //.UseModel(StructuralAnalysisDbContextModel.Instance)
            );

            using IServiceScope scope = services.BuildServiceProvider().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();
            dbContext.Database.EnsureCreated();
        });
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        await AssemblySetup.TearDown();
    }
}

public class TestsInitializer
{
    public TestsInitializer() { }
}

#endif
