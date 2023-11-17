using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using Microsoft.EntityFrameworkCore;
using BeamOS.Common.Infrastructure;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Infrastructure.Common.Configurations;
using BeamOS.PhysicalModel.Domain.ModelAggregate;

namespace BeamOS.PhysicalModel.Infrastructure;

public class PhysicalModelDbContext : DbContext
{
    public PhysicalModelDbContext(DbContextOptions<PhysicalModelDbContext> options) : base(options) { }

    public DbSet<Model> Models { get; set; }
    public DbSet<Element1D> Element1Ds { get; set; }
    public DbSet<Node> Nodes { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddCommonInfrastructure();
        configurationBuilder.AddPhysicalModelInfrastructure();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .ApplyConfigurationsFromAssembly(typeof(NodeConfiguration).Assembly);

        builder.Model.GetEntityTypes()
            .SelectMany(e => e.GetProperties())
            .Where(p => p.IsPrimaryKey())
            .ToList()
            .ForEach(p => p.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never);
    }
}
