using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using BeamOs.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Infrastructure.Data.Configurations.Write;
using BeamOs.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BeamOs.Infrastructure;

/// <summary>
/// Build migrations from folder location
/// \beamOS\src\BeamOS.WebApp\BeamOS.WebApp\
/// with the command
/// dotnet ef migrations add Initial --project ..\..\BeamOs.StructuralAnalysis\BeamOs.Infrastructure\
/// </summary>
internal class BeamOsStructuralReadModelDbContext : DbContext
{
    public BeamOsStructuralReadModelDbContext(
        DbContextOptions<BeamOsStructuralReadModelDbContext> options
    )
        : base(options) { }

    protected BeamOsStructuralReadModelDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Model> Models { get; set; }
    public DbSet<Element1dReadModel> Element1Ds { get; set; }
    public DbSet<NodeReadModel> Nodes { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<SectionProfile> SectionProfiles { get; set; }
    public DbSet<PointLoad> PointLoads { get; set; }
    public DbSet<MomentLoad> MomentLoads { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //configurationBuilder.AddCommonInfrastructure();
        configurationBuilder.AddPhysicalModelInfrastructure();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        _ = builder.ApplyConfigurationsFromAssembly(
            typeof(NodeConfiguration).Assembly,
            ReadConfigurationsFilter
        );

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

    private static bool ReadConfigurationsFilter(Type type) =>
        type.FullName?.Contains($"{nameof(Data.Configurations)}.{nameof(Data.Configurations.Read)}")
        ?? false;
}
