using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Application;
using BeamOs.Common.Contracts;
using BeamOs.Identity;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Application.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Application.Common;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.NodeResults;
using BeamOs.StructuralAnalysis.Infrastructure.AnalyticalResults.ResultSets;
using BeamOs.StructuralAnalysis.Infrastructure.Common;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.MomentLoads;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Infrastructure.PhysicalModel.SectionProfiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceScan.SourceGenerator;
#if Postgres
using EntityFramework.Exceptions.PostgreSQL;
#elif Sqlite
using EntityFramework.Exceptions.Sqlite;
#endif

namespace BeamOs.StructuralAnalysis.Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddStructuralAnalysisInfrastructureRequired(
        this IServiceCollection services
    )
    {
        _ = services.AddScoped<INodeRepository, NodeRepository>();
        _ = services.AddScoped<IInternalNodeRepository, InternalNodeRepository>();
        _ = services.AddScoped<INodeDefinitionRepository, NodeDefinitionRepository>();
        _ = services.AddScoped<IModelRepository, ModelRepository>();
        _ = services.AddScoped<IMaterialRepository, MaterialRepository>();
        _ = services.AddScoped<ISectionProfileRepository, SectionProfileRepository>();
        _ = services.AddScoped<
            ISectionProfileFromLibraryRepository,
            SectionProfileFromLibraryRepository
        >();
        _ = services.AddScoped<IElement1dRepository, Element1dRepository>();
        _ = services.AddScoped<IPointLoadRepository, PointLoadRepository>();
        _ = services.AddScoped<IMomentLoadRepository, MomentLoadRepository>();
        _ = services.AddScoped<ILoadCaseRepository, LoadCaseRepository>();
        _ = services.AddScoped<ILoadCombinationRepository, LoadCombinationRepository>();
        _ = services.AddScoped<INodeResultRepository, NodeResultRepository>();
        _ = services.AddScoped<IResultSetRepository, ResultSetRepository>();
        _ = services.AddScoped<IEnvelopeResultSetRepository, EnvelopeResultSetRepository>();
        _ = services.AddScoped<IModelProposalRepository, ModelProposalRepository>();
        _ = services.AddScoped<IProposalIssueRepository, ProposalIssueRepository>();

#if Postgres
        _ = services.AddScoped<IStructuralAnalysisUnitOfWork, UnitOfWork>();
#elif Sqlite
        _ = services.AddScoped<IStructuralAnalysisUnitOfWork, SqliteUnitOfWork>();
#endif
        // _ = services.AddScoped<IStructuralAnalysisUnitOfWork, UnitOfWork>();

        services.AddQueryHandlers();
        // services.AddObjectThatImplementInterface<IAssemblyMarkerInfrastructure>(
        //     typeof(IQueryHandler<,>),
        //     ServiceLifetime.Scoped,
        //     false
        // );

        return services;
    }

    public static IServiceCollection AddStructuralAnalysisInfrastructureConfigurable(
        this IServiceCollection services,
        string connectionString
    )
    {
        services.AddSingleton(TimeProvider.System);

#if Postgres
        services.AddDb(connectionString);
#elif Sqlite
        DI_Sqlite.AddSqliteInMemoryAndReturnConnection(services);
#endif

        services.AddScoped<IUserIdProvider, UserIdProvider>();

        services.AddScoped<
            IQueryHandler<EmptyRequest, ICollection<ModelInfoResponse>>,
            GetModelsQueryHandler
        >();

        services.AddScoped<IQueryHandler<Guid, ModelInfoResponse>, GetModelInfoQueryHandler>();

        return services;
    }

#if Postgres
    private static void AddDb(this IServiceCollection services, string connectionString) =>
        _ = services.AddDbContext<StructuralAnalysisDbContext>(
            (sp, options) =>
                options
                    .UseNpgsql(
                        connectionString,
                        o => o.MigrationsAssembly(typeof(IAssemblyMarkerInfrastructure).Assembly)
                    )
                    .AddInterceptors(
                        new ModelEntityIdIncrementingInterceptor(TimeProvider.System),
                        new ModelProposalEntityIdIncrementingInterceptor(TimeProvider.System),
                        new PublishDomainEventsInterceptor(sp)
                    )
                    .UseExceptionProcessor()
                    // .UseModel(StructuralAnalysisDbContextModel.Instance)
#if DEBUG
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
                    .LogTo(Console.WriteLine, LogLevel.Error)
#endif
#if !DEBUG
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
#endif

    [RequiresDynamicCode("Calls MigrateAsync which uses reflection to create tables.")]
    public static async Task MigrateDb(
        this IServiceScope scope,
        CancellationToken cancellationToken = default
    )
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();

        // await dbContext.Database.EnsureDeletedAsync();
        // var appliedMigrations = (await dbContext.Database.GetAppliedMigrationsAsync()).ToList();
        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
        if (pendingMigrations.Count > 0)
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }

    public static void EnsureDbCreated(this IServiceScope scope)
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<StructuralAnalysisDbContext>();
        var sql = File.ReadAllText("createSqliteDb.sql");
        _ = dbContext.Database.ExecuteSqlRaw(sql);
    }

    public static void AddPhysicalModelInfrastructure(
        this ModelConfigurationBuilder configurationBuilder
    )
    {
        configurationBuilder.AddValueConverters();
    }

    // {
    //     var valueConverters = typeof(TAssemblyMarker)
    //         .Assembly.GetTypes()
    //         .Where(t =>
    //             t.IsClass
    //             && t.BaseType is not null
    //             && t.BaseType.IsGenericType
    //             && t.BaseType.GetGenericTypeDefinition() == typeof(ValueConverter<,>)
    //         );

    //     foreach (var valueConverterType in valueConverters)
    //     {
    //         var genericArgs = valueConverterType.BaseType?.GetGenericArguments();
    //         if (genericArgs?.Length != 2)
    //         {
    //             throw new ArgumentException();
    //         }

    //         _ = configurationBuilder.Properties(genericArgs[0]).HaveConversion(valueConverterType);
    //     }
    // }

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IEntityTypeConfiguration<>),
        CustomHandler = nameof(ApplyConfiguration)
    )]
    public static partial ModelBuilder AddEntityConfigurations(this ModelBuilder builder);

    private static void ApplyConfiguration<
        TConfig,
        [DynamicallyAccessedMembers(
            DynamicallyAccessedMemberTypes.PublicConstructors
                | DynamicallyAccessedMemberTypes.NonPublicConstructors
                | DynamicallyAccessedMemberTypes.PublicFields
                | DynamicallyAccessedMemberTypes.NonPublicFields
                | DynamicallyAccessedMemberTypes.PublicProperties
                | DynamicallyAccessedMemberTypes.NonPublicProperties
                | DynamicallyAccessedMemberTypes.Interfaces
        )]
            TEntity
    >(ModelBuilder builder)
        where TConfig : IEntityTypeConfiguration<TEntity>, new()
        where TEntity : class
    {
        _ = builder.ApplyConfiguration(new TConfig());
    }

    [GenerateServiceRegistrations(
        AssignableTo = typeof(ValueConverter<,>),
        CustomHandler = nameof(AddValueConverter)
    )]
    public static partial ModelConfigurationBuilder AddValueConverters(
        this ModelConfigurationBuilder services
    );

    private static void AddValueConverter<T, TModel, TProvider>(
        this ModelConfigurationBuilder configurationBuilder
    )
        where T : ValueConverter<TModel, TProvider>
    {
        configurationBuilder.Properties<TModel>().HaveConversion<T>();
    }

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IQueryHandler<,>),
        Lifetime = ServiceLifetime.Scoped,
        AsSelf = true
    )]
    public static partial IServiceCollection AddQueryHandlers(this IServiceCollection services);
}
