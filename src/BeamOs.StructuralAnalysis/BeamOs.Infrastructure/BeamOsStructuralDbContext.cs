using BeamOs.ApiClient.Builders;
using BeamOs.Common.Events;
using BeamOs.Domain.AnalyticalResults.ModelResultAggregate;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Diagrams.Common;
using BeamOs.Domain.Diagrams.MomentDiagramAggregate;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Infrastructure.Data.Configurations;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Fixtures.Mappers.ToDomain;
using BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToDomain;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example3_8;
using BeamOS.Tests.Common.SolvedProblems.Kassimali_MatrixAnalysisOfStructures2ndEd.Example8_4;
using BeamOS.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis.Example10_7;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure;

/// <summary>
/// Build migrations from folder location
/// \beamOS\src\BeamOs.StructuralAnalysis\BeamOs.Api\
/// with the command
/// dotnet ef migrations add Initial --project ..\..\BeamOs.StructuralAnalysis\BeamOs.Infrastructure\ --context BeamOsStructuralDbContext
/// </summary>
public class BeamOsStructuralDbContext : DbContext
{
    //private readonly PublishDomainEventsInterceptor publishDomainEventsInterceptor;

    public BeamOsStructuralDbContext(
        DbContextOptions<BeamOsStructuralDbContext> options
    //PublishDomainEventsInterceptor publishDomainEventsInterceptor
    )
        : base(options)
    {
        //this.publishDomainEventsInterceptor = this.GetService<PublishDomainEventsInterceptor>();
        //this.publishDomainEventsInterceptor = publishDomainEventsInterceptor;
    }

    protected BeamOsStructuralDbContext(
        DbContextOptions options
    //PublishDomainEventsInterceptor publishDomainEventsInterceptor
    )
        : base(options)
    {
        //this.publishDomainEventsInterceptor =
        //    this.GetService<PublishDomainEventsInterceptor>();
        //this.publishDomainEventsInterceptor = publishDomainEventsInterceptor;
    }

    public DbSet<Model> Models { get; set; }
    public DbSet<Element1D> Element1Ds { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<SectionProfile> SectionProfiles { get; set; }
    public DbSet<PointLoad> PointLoads { get; set; }
    public DbSet<MomentLoad> MomentLoads { get; set; }
    public DbSet<ModelResult> ModelResults { get; set; }
    public DbSet<NodeResult> NodeResults { get; set; }
    public DbSet<ShearForceDiagram> ShearForceDiagrams { get; set; }
    public DbSet<MomentDiagram> MomentDiagrams { get; set; }
    public DbSet<DiagramConsistantInterval> DiagramConsistantIntervals { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //configurationBuilder.AddCommonInfrastructure();
        configurationBuilder.AddPhysicalModelInfrastructure();
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.AddInterceptors(this.publishDomainEventsInterceptor);
    //    base.OnConfiguring(optionsBuilder);
    //}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        _ = builder
            .Ignore<List<IDomainEvent>>()
            .ApplyConfigurationsFromAssembly(typeof(NodeConfiguration).Assembly);

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

    public async Task SeedAsync()
    {
        //var isCreated = this.Database.EnsureCreated();

        //await this.InsertIntoEfCore(TwistyBowlFraming.Instance);
        await this.InsertIntoEfCore(Kassimali_Example3_8.Instance);
        await this.InsertIntoEfCore(Kassimali_Example8_4.Instance);
        await this.InsertIntoEfCore(Udoeyo_StructuralAnalysis_Example10_7.Instance);
        //await this.InsertIntoEfCore(Simple_3_Story_Rectangular.Instance.to)

        _ = await this.SaveChangesAsync();
    }

    private async Task InsertIntoEfCore(ModelFixture2 modelFixture)
    {
        ModelId modelId = new(modelFixture.ModelGuid);
        if (await this.Models.AnyAsync(m => m.Id == modelId))
        {
            return;
        }

        this.InsertIntoEfCore(modelFixture.ToDomain());
    }

    private async Task InsertIntoEfCore(CreateModelRequestBuilder modelBuilder)
    {
        ModelId modelId = new(modelBuilder.ModelGuid);
        if (await this.Models.AnyAsync(m => m.Id == modelId))
        {
            return;
        }

        await modelBuilder.InitializeAsync();
        CreateModelRequestBuilderToDomainMapper builderMapper = new();
        Model model = builderMapper.ToDomain(modelBuilder);

        this.InsertIntoEfCore(model);
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
                            el.StrongAxisShearArea,
                            el.WeakAxisShearArea,
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
