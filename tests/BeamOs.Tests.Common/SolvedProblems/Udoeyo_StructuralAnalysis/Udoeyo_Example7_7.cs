using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;

namespace BeamOs.Tests.Common.SolvedProblems.Udoeyo_StructuralAnalysis;

public sealed class Udoeyo_Example7_7 : ModelFixture
{
    public override SourceInfo SourceInfo { get; } =
        new(
            "Structural Analysis by Felix Udoeyo",
            FixtureSourceType.Textbook,
            "Example 7.7",
            null,
            "https://temple.manifoldapp.org/read/structural-analysis/section/072c774b-392a-4b99-a65d-f2e9d1fddfbd#sec7-5"
        );
    public override string Name { get; } = nameof(Udoeyo_Example7_7);
    public override string Description { get; } =
        "Example 7.7 from Udoeyo's Structural Analysis book";
    public override PhysicalModelSettings Settings { get; } = new(UnitSettingsContract.kN_M);
    public override string GuidString => "e63cca48-5585-4117-8dd9-0d0bf56f3d50";

    public override IEnumerable<PutElement1dRequest> Element1dRequests()
    {
        yield return new()
        {
            Id = 1,
            StartNodeId = 1,
            EndNodeId = 2,
            MaterialId = 1,
            SectionProfileId = 1
        };
    }

    public override IEnumerable<PutMaterialRequest> MaterialRequests()
    {
        yield return new()
        {
            Id = 1,
            ModulusOfElasticity = new(1, PressureUnitContract.KilonewtonPerSquareMeter),
            ModulusOfRigidity = new(1, PressureUnitContract.KilonewtonPerSquareMeter),
        };
    }

    public override IEnumerable<PutNodeRequest> NodeRequests()
    {
        yield return new()
        {
            Id = 1,
            LocationPoint = new(0, 0, 0, LengthUnitContract.Meter),
            Restraint = Restraint.FreeXyPlane,
        };

        yield return new()
        {
            Id = 2,
            LocationPoint = new(6, 0, 0, LengthUnitContract.Meter),
            Restraint = Restraint.Fixed,
        };
    }

    public override IEnumerable<PutPointLoadRequest> PointLoadRequests() => [];

    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests()
    {
        yield return new()
        {
            Id = 1,
            Area = new(1, AreaUnitContract.SquareMeter),
            StrongAxisMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.MeterToTheFourth),
            WeakAxisMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.MeterToTheFourth),
            PolarMomentOfInertia = new(1, AreaMomentOfInertiaUnitContract.MeterToTheFourth),
            StrongAxisShearArea = new(1, AreaUnitContract.SquareMeter),
            WeakAxisShearArea = new(1, AreaUnitContract.SquareMeter),
        };
    }

    public override IEnumerable<PutMomentLoadRequest> MomentLoadRequests()
    {
        yield return new()
        {
            Id = 1,
            NodeId = 1,
            Torque = new(-20, TorqueUnitContract.KilonewtonMeter),
            AxisDirection = new(0, 0, 1),
        };
    }
}
