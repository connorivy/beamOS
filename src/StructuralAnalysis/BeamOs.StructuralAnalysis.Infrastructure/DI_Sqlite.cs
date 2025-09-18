#if Sqlite
using EntityFramework.Exceptions.Sqlite;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Diagnostics;
using BeamOs.StructuralAnalysis.Api;

namespace BeamOs.StructuralAnalysis.Infrastructure;

public static class DI_Sqlite
{
    private static readonly Lock dbConnectionLock = new();

    public static IDisposable AddSqliteInMemoryAndReturnConnection(this IServiceCollection services)
    {
        var connection = new Microsoft.Data.Sqlite.SqliteConnection(
            "DataSource=:memory:;Cache=Shared"
        );
        connection.Open();
        _ = services.AddDbContext<StructuralAnalysisDbContext>(options =>
            options
                .UseSqlite(connection)
                .AddInterceptors(new SqliteCompositeKeyIncrementingInterceptor())
                .AddInterceptors(new ModelEntityIdIncrementingInterceptor(TimeProvider.System))
                .UseExceptionProcessor()
                .UseModel(StructuralAnalysisDbContextModel.Instance)
#if DEBUG
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine, LogLevel.Information)
#else
                .UseLoggerFactory(
                    LoggerFactory.Create(builder =>
                    {
                        builder.AddFilter((category, level) => level >= LogLevel.Error);
                    })
                )
#endif
                .ConfigureWarnings(warnings =>
                    warnings.Log(RelationalEventId.PendingModelChangesWarning)
                )
        );
        return connection;
    }
}

#endif
