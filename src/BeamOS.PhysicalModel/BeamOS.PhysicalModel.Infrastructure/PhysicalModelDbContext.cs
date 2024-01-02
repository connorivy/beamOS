using BeamOS.Common.Domain.Utils;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.Common.Infrastructure;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.PointLoadAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using BeamOS.PhysicalModel.Infrastructure.Common.Configurations;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.EntityFrameworkCore;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Infrastructure;

/// <summary>
/// Build migrations from folder location
/// \beamOS\src\BeamOS.PhysicalModel\BeamOS.PhysicalModel.Api\
/// with the command
/// dotnet ef migrations add Initial --project ..\BeamOS.PhysicalModel.Infrastructure\
/// </summary>
public class PhysicalModelDbContext : DbContext
{
    public PhysicalModelDbContext(DbContextOptions<PhysicalModelDbContext> options)
        : base(options) { }

    protected PhysicalModelDbContext(DbContextOptions options)
        : base(options) { }

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
        _ = builder.ApplyConfigurationsFromAssembly(typeof(NodeConfiguration).Assembly);

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
        this.Database.EnsureCreated();
        ModelId zeroId = new(TypedGuids.G0);
        if (await this.Models.AnyAsync(m => m.Id == zeroId))
        {
            return;
        }

        _ = this.Models.Add(
            new(
                "Big Ol' Building",
                "Building on the corner of 6th and Main",
                new(UnitSettings.K_IN),
                new(TypedGuids.G0)
            )
        );

        //_ = this.Nodes.Add(
        //    new(
        //        new(TypedGuids.G0),
        //        0,
        //        0,
        //        0,
        //        UnitSettings.K_FT.LengthUnit,
        //        restraint: Restraint.Free,
        //        id: new(TypedGuids.G1)
        //    )
        //);

        //_ = this.Nodes.Add(
        //    new(
        //        new(TypedGuids.G0),
        //        -20,
        //        0,
        //        0,
        //        UnitSettings.K_FT.LengthUnit,
        //        restraint: Restraint.Fixed,
        //        id: new(TypedGuids.G2)
        //    )
        //);

        //_ = this.Nodes.Add(
        //    new(
        //        new(TypedGuids.G0),
        //        0,
        //        -20,
        //        0,
        //        UnitSettings.K_FT.LengthUnit,
        //        restraint: Restraint.Fixed,
        //        id: new(TypedGuids.G3)
        //    )
        //);

        //_ = this.Nodes.Add(
        //    new(
        //        new(TypedGuids.G0),
        //        0,
        //        0,
        //        -20,
        //        UnitSettings.K_FT.LengthUnit,
        //        restraint: Restraint.Fixed,
        //        id: new(TypedGuids.G4)
        //    )
        //);

        //Material steel29000 =
        //    new(
        //        new(TypedGuids.G0),
        //        new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
        //        new Pressure(11500, PressureUnit.KilopoundForcePerSquareInch),
        //        new MaterialId(TypedGuids.G0)
        //    );

        //SectionProfile profile33in2 =
        //    new(
        //        new(TypedGuids.G0),
        //        new Area(32.9, AreaUnit.SquareInch),
        //        strongAxisMomentOfInertia: new AreaMomentOfInertia(
        //            716,
        //            AreaMomentOfInertiaUnit.InchToTheFourth
        //        ),
        //        weakAxisMomentOfInertia: new AreaMomentOfInertia(
        //            236,
        //            AreaMomentOfInertiaUnit.InchToTheFourth
        //        ),
        //        polarMomentOfInertia: new AreaMomentOfInertia(
        //            15.1,
        //            AreaMomentOfInertiaUnit.InchToTheFourth
        //        )
        //    );

        ////_ = this.PointLoads.Add(
        ////    new(
        ////        new(TypedGuids.G0),
        ////        new(5, ForceUnit.KilopoundForce),
        ////        DenseVector.OfArray([0.0, -1, 0]),
        ////        new(TypedGuids.G0)
        ////    )
        ////);

        //_ = this.Element1Ds.Add(
        //    new(
        //        new ModelId(TypedGuids.G0),
        //        new NodeId(TypedGuids.G2),
        //        new NodeId(TypedGuids.G1),
        //        steel29000.Id,
        //        profile33in2.Id,
        //        new Element1DId(TypedGuids.G1)
        //    )
        //    {
        //        SectionProfileRotation = Angle.Zero
        //    }
        //);

        //_ = this.Element1Ds.Add(
        //    new(
        //        new ModelId(TypedGuids.G0),
        //        new NodeId(TypedGuids.G3),
        //        new NodeId(TypedGuids.G1),
        //        steel29000.Id,
        //        profile33in2.Id,
        //        new Element1DId(TypedGuids.G2)
        //    )
        //    {
        //        SectionProfileRotation = new Angle(Math.PI / 2, AngleUnit.Radian)
        //    }
        //);

        //_ = this.Element1Ds.Add(
        //    new(
        //        new ModelId(TypedGuids.G0),
        //        new NodeId(TypedGuids.G4),
        //        new NodeId(TypedGuids.G1),
        //        steel29000.Id,
        //        profile33in2.Id,
        //        new Element1DId(TypedGuids.G3)
        //    )
        //    {
        //        SectionProfileRotation = new Angle(30, AngleUnit.Degree),
        //    }
        //);

        //_ = this.Materials.Add(
        //    new(
        //        new(TypedGuids.G0),
        //        new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
        //        new Pressure(11500, PressureUnit.KilopoundForcePerSquareInch),
        //        new MaterialId(TypedGuids.G0)
        //    )
        //);

        //_ = this.SectionProfiles.Add(
        //    new(
        //        new(TypedGuids.G0),
        //        new Area(32.9, AreaUnit.SquareInch),
        //        strongAxisMomentOfInertia: new AreaMomentOfInertia(
        //            716,
        //            AreaMomentOfInertiaUnit.InchToTheFourth
        //        ),
        //        weakAxisMomentOfInertia: new AreaMomentOfInertia(
        //            236,
        //            AreaMomentOfInertiaUnit.InchToTheFourth
        //        ),
        //        polarMomentOfInertia: new AreaMomentOfInertia(
        //            15.1,
        //            AreaMomentOfInertiaUnit.InchToTheFourth
        //        ),
        //        new(TypedGuids.G0)
        //    )
        //);

        Material steel29000 =
            new(
                new(TypedGuids.G0),
                new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
                new Pressure(1, PressureUnit.KilopoundForcePerSquareInch)
            );
        SectionProfile area8In =
            new(
                new(TypedGuids.G0),
                new Area(8, AreaUnit.SquareInch),
                new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
                new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
                new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth)
            );
        SectionProfile area6In =
            new(
                new(TypedGuids.G0),
                new Area(6, AreaUnit.SquareInch),
                new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
                new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth),
                new AreaMomentOfInertia(1, AreaMomentOfInertiaUnit.InchToTheFourth)
            );
        _ = this.Materials.Add(steel29000);
        _ = this.SectionProfiles.Add(area8In);
        _ = this.SectionProfiles.Add(area6In);

        Restraint free2D = new(true, true, false, false, false, true);
        Restraint pinned2d = new(false, false, false, false, false, true);
        var node1 = new Node(
            new(TypedGuids.G0),
            12,
            16,
            0,
            LengthUnit.Foot,
            restraint: free2D,
            id: new(TypedGuids.G1)
        );

        PointLoad pl1 = new PointLoad(
            node1.Id,
            new Force(150, ForceUnit.KilopoundForce),
            DenseVector.OfArray([1, 0, 0])
        );

        PointLoad pl2 = new PointLoad(
            node1.Id,
            new Force(300, ForceUnit.KilopoundForce),
            DenseVector.OfArray([0, -1, 0])
        );

        _ = this.Nodes.Add(node1);
        _ = this.PointLoads.Add(pl1);
        _ = this.PointLoads.Add(pl2);

        var node2 = new Node(
            new(TypedGuids.G0),
            0,
            0,
            0,
            LengthUnit.Foot,
            restraint: pinned2d,
            id: new(TypedGuids.G2)
        );

        _ = this.Nodes.Add(node2);

        var node3 = new Node(
            new(TypedGuids.G0),
            12,
            0,
            0,
            LengthUnit.Foot,
            pinned2d,
            id: new(TypedGuids.G3)
        );
        _ = this.Nodes.Add(node3);

        var node4 = new Node(
            new(TypedGuids.G0),
            24,
            0,
            0,
            LengthUnit.Foot,
            pinned2d,
            id: new(TypedGuids.G4)
        );
        _ = this.Nodes.Add(node4);

        Element1D element1 =
            new(
                new(TypedGuids.G0),
                node2.Id,
                node1.Id,
                steel29000.Id,
                area8In.Id,
                id: new(TypedGuids.G1)
            );
        _ = this.Element1Ds.Add(element1);

        Element1D element2 =
            new(
                new(TypedGuids.G0),
                node3.Id,
                node1.Id,
                steel29000.Id,
                area6In.Id,
                id: new(TypedGuids.G2)
            );
        _ = this.Element1Ds.Add(element2);

        Element1D element3 =
            new(
                new(TypedGuids.G0),
                node4.Id,
                node1.Id,
                steel29000.Id,
                area8In.Id,
                id: new(TypedGuids.G3)
            );
        _ = this.Element1Ds.Add(element3);

        _ = await this.SaveChangesAsync();
    }
}
