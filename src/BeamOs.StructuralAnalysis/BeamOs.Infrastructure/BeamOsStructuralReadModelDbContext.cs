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

    public DbSet<ModelReadModel> Models { get; set; }
    public DbSet<Element1dReadModel> Element1Ds { get; set; }
    public DbSet<NodeReadModel> Nodes { get; set; }
    public DbSet<MaterialReadModel> Materials { get; set; }
    public DbSet<SectionProfileReadModel> SectionProfiles { get; set; }
    public DbSet<PointLoadReadModel> PointLoads { get; set; }
    public DbSet<MomentLoadReadModel> MomentLoads { get; set; }

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
    }

    private static bool ReadConfigurationsFilter(Type type) =>
        type.FullName?.Contains($"{nameof(Data.Configurations)}.{nameof(Data.Configurations.Read)}")
        ?? false;
}
