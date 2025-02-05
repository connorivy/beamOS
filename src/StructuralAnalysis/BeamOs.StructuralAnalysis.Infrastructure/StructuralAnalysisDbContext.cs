using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Application.PhysicalModel.Models.Mappers;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Tests.Common;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure;

/// <summary>
/// dotnet ef dbcontext optimize --output-dir ..\BeamOs.StructuralAnalysis.Infrastructure\CompiledModels
/// </summary>
public class StructuralAnalysisDbContext : DbContext
{
    public StructuralAnalysisDbContext(DbContextOptions<StructuralAnalysisDbContext> options)
        : base(options) { }

    protected StructuralAnalysisDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Model> Models { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<SectionProfile> SectionProfiles { get; set; }
    public DbSet<Element1d> Element1ds { get; set; }
    public DbSet<ResultSet> ResultSets { get; set; }
    public DbSet<NodeResult> NodeResults { get; set; }
    public DbSet<PointLoad> PointLoads { get; set; }
    public DbSet<MomentLoad> MomentLoads { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //configurationBuilder.AddCommonInfrastructure();
        configurationBuilder.AddPhysicalModelInfrastructure();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        _ = builder
            .Ignore<List<IDomainEvent>>()
            .ApplyConfigurationsFromAssembly(typeof(StructuralAnalysisDbContext).Assembly);

        //builder
        //    .Model
        //    .GetEntityTypes()
        //    .SelectMany(e => e.GetProperties())
        //    .Where(p => p.IsPrimaryKey())
        //    .ToList()
        //    .ForEach(
        //        p => p.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never
        //    );
    }

    public async Task SeedTestModels()
    {
        foreach (var modelBuilder in AllSolvedProblems.ModelFixtures())
        {
            BeamOsModelBuilderDomainMapper mapper = new(modelBuilder.Id);
            ModelId typedId = new(modelBuilder.Id);

            if (await this.Models.AnyAsync(x => x.Id == typedId))
            {
                continue;
            }

            var model = mapper.ToDomain(modelBuilder);
            this.Models.Add(model);
        }
        await this.SaveChangesAsync();
    }
}
