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
    public override SourceInfo SourceInfo { get; }
    public override string Name { get; } = nameof(Udoeyo_Example7_7);
    public override string Description { get; } =
        "Example 7.7 from Udoeyo's Structural Analysis book";
    public override PhysicalModelSettings Settings { get; } = new(UnitSettingsContract.kN_M);
    public override string GuidString => "e63cca48-5585-4117-8dd9-0d0bf56f3d50";

    public override IEnumerable<CreateElement1dRequest> Element1dRequests()
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

    public override IEnumerable<CreateMaterialRequest> MaterialRequests()
    {
        yield return new()
        {
            Id = 1,
            ModulusOfElasticity = new(1, PressureUnitContract.KilonewtonPerSquareMeter),
            ModulusOfRigidity = new(1, PressureUnitContract.KilonewtonPerSquareMeter),
        };
    }

    public override IEnumerable<CreateNodeRequest> NodeRequests()
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

    public override IEnumerable<CreatePointLoadRequest> PointLoadRequests() => [];

    public override IEnumerable<CreateSectionProfileRequest> SectionProfileRequests()
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

    public override IEnumerable<CreateMomentLoadRequest> MomentLoadRequests()
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
