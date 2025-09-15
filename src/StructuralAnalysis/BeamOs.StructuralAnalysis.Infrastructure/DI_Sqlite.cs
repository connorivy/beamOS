#if Sqlite
using EntityFramework.Exceptions.Sqlite;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Diagnostics;

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

        //         var options = new DbContextOptionsBuilder<StructuralAnalysisDbContext>()
        //             // .UseSqlite(connection)
        //             .UseSqlite(connection)
        //             // .AddInterceptors(new ModelLastModifiedUpdater(TimeProvider.System))
        //             .AddInterceptors(new SqliteConnectionLockInterceptor())
        //             .AddInterceptors(new ModelEntityIdIncrementingInterceptor(TimeProvider.System))
        //             .UseExceptionProcessor()
        // #if DEBUG
        //             .EnableSensitiveDataLogging()
        //             .EnableDetailedErrors()
        //             .LogTo(Console.WriteLine, LogLevel.Information)
        // #else
        //             .UseLoggerFactory(
        //                 LoggerFactory.Create(builder =>
        //                 {
        //                     builder.AddFilter((category, level) => level >= LogLevel.Error);
        //                 })
        //             )
        // #endif
        //             .ConfigureWarnings(warnings =>
        //                 warnings.Log(RelationalEventId.PendingModelChangesWarning)
        //             )
        //             .Options;

        //         services.AddScoped(sp =>
        //         {
        //             sp.GetRequiredService<ConnectionLock>();
        //             lock (dbConnectionLock)
        //             {
        //                 return new StructuralAnalysisDbContext(options);
        //             }
        //         });
        _ = services.AddDbContext<StructuralAnalysisDbContext>(options =>
            options
                .UseSqlite(connection)
                // .UseSqlite("DataSource=::;Cache=Shared")
                // .AddInterceptors(new ModelLastModifiedUpdater(TimeProvider.System))
                // .AddInterceptors(new SqliteConnectionLockInterceptor())
                .AddInterceptors(new SqliteCompositeKeyIncrementingInterceptor())
                .AddInterceptors(new ModelEntityIdIncrementingInterceptor(TimeProvider.System))
                // .AddInterceptors(new SqliteSavingChangesConnectionLockInterceptor())
                .UseExceptionProcessor()
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

internal sealed class ConnectionLock : IDisposable
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public ConnectionLock()
    {
        _semaphore.Wait();
    }

    public void Dispose()
    {
        _semaphore.Release();
    }
}

#endif
