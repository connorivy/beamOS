using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure;

/// <summary>
/// dotnet ef migrations add Initial --project ..\BeamOs.StructuralAnalysis.Infrastructure\
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
    public DbSet<InternalNode> InternalNodes { get; set; }
    public DbSet<NodeDefinition> NodeDefinitions { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<SectionProfile> SectionProfiles { get; set; }
    public DbSet<Element1d> Element1ds { get; set; }
    public DbSet<PointLoad> PointLoads { get; set; }
    public DbSet<MomentLoad> MomentLoads { get; set; }
    public DbSet<LoadCase> LoadCases { get; set; }
    public DbSet<LoadCombination> LoadCombinations { get; set; }
    public DbSet<ModelProposal> ModelProposals { get; set; }
    public DbSet<DeleteModelEntityProposal> DeleteModelEntityProposals { get; set; }

    // public DbSet<NodeProposal> NodeProposals { get; set; }
    // public DbSet<InternalNodeProposal> InternalNodeProposals { get; set; }
    public DbSet<Element1dProposal> Element1dProposals { get; set; }

    public DbSet<ResultSet> ResultSets { get; set; }
    public DbSet<NodeResult> NodeResults { get; set; }
    public DbSet<Element1dResult> Element1dResults { get; set; }

    public DbSet<EnvelopeResultSet> EnvelopeResultSets { get; set; }
    public DbSet<EnvelopeElement1dResult> EnvelopeElement1dResults { get; set; }

    //public DbSet<ShearForceDiagram> ShearForceDiagrams { get; set; }
    //public DbSet<MomentDiagram> MomentDiagrams { get; set; }
    //public DbSet<MomentDiagramConsistentInterval> MomentDiagramConsistentIntervals { get; set; }

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
    }
}
