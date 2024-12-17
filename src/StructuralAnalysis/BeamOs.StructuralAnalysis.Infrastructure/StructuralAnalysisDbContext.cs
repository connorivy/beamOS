using BeamOs.StructuralAnalysis.Domain.Common.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.StructuralAnalysis.Infrastructure;

public class StructuralAnalysisDbContext : DbContext
{
    public StructuralAnalysisDbContext(DbContextOptions<StructuralAnalysisDbContext> options)
        : base(options) { }

    protected StructuralAnalysisDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Node> Nodes { get; set; }

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
}
