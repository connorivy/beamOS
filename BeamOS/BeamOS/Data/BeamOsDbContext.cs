using BeamOS.Common.Infrastructure;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using BeamOS.PhysicalModel.Infrastructure;
using BeamOS.PhysicalModel.Infrastructure.Common.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BeamOS.Data;

public class BeamOsDbContext : IdentityDbContext<BeamOsUser>
{
    public BeamOsDbContext() : base() { }
    public BeamOsDbContext(DbContextOptions<BeamOsDbContext> options) : base(options) { }
    // Physical Model Entities
    //public DbSet<Model> Models { get; set; }
    public DbSet<Element1D> Element1Ds { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<SectionProfile> SectionProfiles { get; set; }

    // Direct Stiffness Method Entities
    //public DbSet<AnalyticalModel> AnalyticalModels { get; set; }
    //public DbSet<AnalyticalElement1D> AnalyticalElements1Ds { get; set; }
    //public DbSet<AnalyticalNode> AnalyticalNodes { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.AddCommonInfrastructure();
        configurationBuilder.AddPhysicalModelInfrastructure();
        //_ = configurationBuilder
        //    .Properties<ModelId>()
        //    .HaveConversion<ModelIdConverter>();
        //_ = configurationBuilder
        //    .Properties<Element1DId>()
        //    .HaveConversion<Element1DIdConverter>();
        //_ = configurationBuilder
        //    .Properties<NodeId>()
        //    .HaveConversion<NodeIdConverter>();
        //_ = configurationBuilder
        //    .Properties<SectionProfileId>()
        //    .HaveConversion<SectionProfileIdConverter>();
        //_ = configurationBuilder
        //    .Properties<MaterialId>()
        //    .HaveConversion<MaterialIdConverter>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\\mssqllocaldb;Database=BeamOS;Trusted_Connection=True;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .ApplyConfigurationsFromAssembly(typeof(NodeConfiguration).Assembly);
    }
}
