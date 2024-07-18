using BeamOs.Common.Events;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Common.Interfaces;
using BeamOs.Domain.Common.Utils;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Infrastructure.Data.Configurations.Write;
using BeamOs.Infrastructure.Interceptors;
using BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToDomain;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using MathNet.Spatial.Euclidean;
using Microsoft.EntityFrameworkCore;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Infrastructure;

/// <summary>
/// Build migrations from folder location
/// \beamOS\src\BeamOs.StructuralAnalysis\BeamOs.Api\
/// with the command
/// dotnet ef migrations add Initial --project ..\..\BeamOs.StructuralAnalysis\BeamOs.Infrastructure\ --context BeamOsStructuralDbContext
/// </summary>
public class BeamOsStructuralDbContext : DbContext
{
    private readonly PublishIntegrationEventsInterceptor publishIntegrationEventsInterceptor;

    public BeamOsStructuralDbContext(
        DbContextOptions<BeamOsStructuralDbContext> options,
        PublishIntegrationEventsInterceptor publishIntegrationEventsInterceptor
    )
        : base(options)
    {
        this.publishIntegrationEventsInterceptor = publishIntegrationEventsInterceptor;
    }

    protected BeamOsStructuralDbContext(
        DbContextOptions options,
        PublishIntegrationEventsInterceptor publishIntegrationEventsInterceptor
    )
        : base(options)
    {
        this.publishIntegrationEventsInterceptor = publishIntegrationEventsInterceptor;
    }

    public DbSet<Model> Models { get; set; }
    public DbSet<Element1D> Element1Ds { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<SectionProfile> SectionProfiles { get; set; }
    public DbSet<PointLoad> PointLoads { get; set; }
    public DbSet<MomentLoad> MomentLoads { get; set; }
    public DbSet<NodeResult> NodeResults { get; set; }
    public DbSet<ShearForceDiagram> ShearForceDiagrams { get; set; }
    public DbSet<DiagramConsistantInterval> DiagramConsistantIntervals { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //configurationBuilder.AddCommonInfrastructure();
        configurationBuilder.AddPhysicalModelInfrastructure();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(this.publishIntegrationEventsInterceptor);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        _ = builder
            .Ignore<List<IIntegrationEvent>>()
            .ApplyConfigurationsFromAssembly(
                typeof(NodeConfiguration).Assembly,
                WriteConfigurationsFilter
            );

        builder
            .Model
            .GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.IsPrimaryKey())
            .ToList()
            .ForEach(
                p => p.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never
            );
    }

    private static bool WriteConfigurationsFilter(Type type) =>
        type.FullName?.Contains(
            $"{nameof(Data.Configurations)}.{nameof(Data.Configurations.Write)}"
        ) ?? false;

    public async Task SeedAsync()
    {
        _ = this.Database.EnsureCreated();
        ModelId zeroId = new(TypedGuids.G0);
        if (await this.Models.AnyAsync(m => m.Id == zeroId))
        {
            return;
        }

        var kass_3_8 = Kassimali_Example3_8.Instance.ToDomain();
        this.InsertIntoEfCore(kass_3_8);

        _ = await this.SaveChangesAsync();
    }

    private void InsertIntoEfCore(Model model)
    {
        this.Models.Add(new Model(model.Name, model.Description, model.Settings, model.Id));
        this.AddEntities(
            model
                .SectionProfiles
                .Select(
                    el =>
                        new SectionProfile(
                            model.Id,
                            el.Area,
                            el.StrongAxisMomentOfInertia,
                            el.WeakAxisMomentOfInertia,
                            el.PolarMomentOfInertia,
                            el.Id
                        )
                )
        );

        this.AddEntities(
            model
                .Materials
                .Select(
                    el =>
                        new Material(model.Id, el.ModulusOfElasticity, el.ModulusOfRigidity, el.Id)
                )
        );

        this.AddEntities(model.Nodes); // keep point loads and moment loads loaded here

        this.AddEntities(
            model
                .Element1ds
                .Select(
                    el =>
                        new Element1D(
                            model.Id,
                            el.StartNodeId,
                            el.EndNodeId,
                            el.MaterialId,
                            el.SectionProfileId,
                            el.Id
                        )
                )
        );
    }

    private void AddEntities<TEntity>(IEnumerable<TEntity> entities)
        where TEntity : class
    {
        foreach (var entity in entities)
        {
            this.Set<TEntity>().Add(entity);
        }
    }
}
