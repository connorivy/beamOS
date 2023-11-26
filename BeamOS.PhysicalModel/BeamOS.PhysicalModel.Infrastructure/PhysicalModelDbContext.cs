using BeamOS.Common.Infrastructure;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using BeamOS.PhysicalModel.Infrastructure.Common.Configurations;
using Microsoft.EntityFrameworkCore;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Infrastructure;

/// <summary>
/// Build migrations from folder location
/// \beamOS\BeamOS.PhysicalModel\BeamOS.PhysicalModel.Api\
/// with the command
/// dotnet ef migrations add Initial --project ..\BeamOS.PhysicalModel.Infrastructure\
/// </summary>
public class PhysicalModelDbContext : DbContext
{
    public PhysicalModelDbContext(DbContextOptions<PhysicalModelDbContext> options) : base(options) { }
    protected PhysicalModelDbContext(DbContextOptions options) : base(options) { }

    public DbSet<Model> Models { get; set; }
    public DbSet<Element1D> Element1Ds { get; set; }
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Material> Materials { get; set; }
    public DbSet<SectionProfile> SectionProfiles { get; set; }
    public DbSet<PointLoad> PointLoads { get; set; }

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

    public async Task SeedAsync()
    {
        ModelId zeroId = new(Guid.Parse("00000000-0000-0000-0000-000000000000"));
        if (await this.Models
            .AnyAsync(m => m.Id == zeroId))
        {
            return;
        }

        this.Models.Add(new(
            "Big Ol' Building",
            "Building on the corner of 6th and Main",
            new(UnitSettings.K_IN),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));

        this.Nodes.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            0,
            0,
            0,
            UnitSettings.K_IN.LengthUnit,
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));

        this.Nodes.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            0,
            0,
            10,
            UnitSettings.K_IN.LengthUnit,
            new(Guid.Parse("00000000-0000-0000-0000-000000000001"))
        ));

        this.Nodes.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            0,
            0,
            -10,
            UnitSettings.K_IN.LengthUnit,
            new(Guid.Parse("00000000-0000-0000-0000-000000000002"))
        ));

        this.Element1Ds.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000001")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));

        this.Element1Ds.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000001")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000002")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));


        this.Materials.Add(new Material(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(29000, PressureUnit.KilopoundForcePerSquareInch),
            new(11460, PressureUnit.KilopoundForcePerSquareInch),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));

        //W12x19
        this.SectionProfiles.Add(new(
            new(Guid.Parse("00000000-0000-0000-0000-000000000000")),
            new(5.57, AreaUnit.SquareInch),
            new(130, AreaMomentOfInertiaUnit.InchToTheFourth),
            new(3.76, AreaMomentOfInertiaUnit.InchToTheFourth),
            new(.18, AreaMomentOfInertiaUnit.InchToTheFourth),
            new(Guid.Parse("00000000-0000-0000-0000-000000000000"))
        ));

        await this.SaveChangesAsync();
    }
}
