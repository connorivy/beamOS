using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using Microsoft.EntityFrameworkCore;
using BeamOS.PhysicalModel.Infrastructure;
using BeamOS.Common.Infrastructure;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Infrastructure.Configurations;

namespace BeamOS.PhysicalModel.Api.Data;

public class PhysicalModelDbContext : DbContext
{
    public PhysicalModelDbContext(DbContextOptions<PhysicalModelDbContext> options) : base(options) { }
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
    }
}
