using System.Reflection;
using System.Text.Json;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.MomentLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.CsSdk;
using UnitsNet.Units;

namespace BeamOs.Tests.Common.SolvedProblems.SAP2000;

public class TwistyBowlFraming : ModelFixture, IHasExpectedNodeResults
{
    public override SourceInfo SourceInfo { get; } =
        new("SAP2000", FixtureSourceType.SAP2000, nameof(TwistyBowlFraming));
    public override string Name => nameof(TwistyBowlFraming);
    public override string Description =>
        "A crazy twisting bowl type structure. Made by Bjorn Steinhagen in grasshopper and then sent to beamOS using Speckle";
    public override PhysicalModelSettings Settings { get; } = new(UnitSettingsContract.K_IN);
    public override string GuidString => "4ce66084-4ac1-40bc-99ae-3d0f334c66fa";

    private BeamOsDynamicModelBuilder modelBuilder { get; set; }

    public override async ValueTask InitializeAsync()
    {
        if (modelBuilder is not null)
        {
            // already initialized
            return;
        }

        var filePath = Path.Combine(
            Path.GetDirectoryName(typeof(TwistyBowlFraming).Assembly.Location),
            "TwistyBowlFraming.json"
        );

        if (File.Exists(filePath))
        {
            string jsonString = await File.ReadAllTextAsync(filePath);
            BeamOsModelBuilderDto dto = JsonSerializer.Deserialize<BeamOsModelBuilderDto>(
                jsonString
            );
            this.modelBuilder = new(
                this.GuidString,
                this.Settings,
                this.Name,
                this.Description,
                dto
            );
        }
        else
        {
            throw new NotImplementedException();
        }
        //this.modelBuilder = new(this.GuidString, this.Settings, this.Name, this.Description);

        //modelBuilder.AddSectionProfiles(
        //    new PutSectionProfileRequest()
        //    {
        //        Area = new AreaContract(10.6, AreaUnitContract.SquareInch),
        //        StrongAxisMomentOfInertia = new AreaMomentOfInertiaContract(
        //            448,
        //            AreaMomentOfInertiaUnitContract.InchToTheFourth
        //        ),
        //        WeakAxisMomentOfInertia = new AreaMomentOfInertiaContract(
        //            24.5,
        //            AreaMomentOfInertiaUnitContract.InchToTheFourth
        //        ),
        //        PolarMomentOfInertia = new AreaMomentOfInertiaContract(
        //            .55,
        //            AreaMomentOfInertiaUnitContract.InchToTheFourth
        //        ),
        //        StrongAxisShearArea = new AreaContract(5.0095, AreaUnitContract.SquareInch),
        //        WeakAxisShearArea = new AreaContract(4.6905, AreaUnitContract.SquareInch),
        //        Id = 1636
        //    }
        //);

        //modelBuilder.AddMaterials(
        //    new PutMaterialRequest()
        //    {
        //        ModulusOfElasticity = new PressureContract(
        //            29000,
        //            PressureUnitContract.KilopoundForcePerSquareInch
        //        ),
        //        ModulusOfRigidity = new PressureContract(
        //            11_153.85,
        //            PressureUnitContract.KilopoundForcePerSquareInch
        //        ),
        //        Id = 992
        //    }
        //);

        //var recieveOperation = new BeamOsSpeckleReceiveOperation()
        //{
        //    Element1dRequestModifier = static el =>
        //        el with
        //        {
        //            MaterialId = 992,
        //            SectionProfileId = 1636
        //        },
        //    NodeRequestModifier = static el =>
        //        el with
        //        {
        //            LocationPoint = new(
        //                el.LocationPoint.X,
        //                el.LocationPoint.Z,
        //                -el.LocationPoint.Y,
        //                el.LocationPoint.LengthUnit
        //            )
        //        }
        //};

        //await foreach (
        //    IBeamOsEntityRequest request in recieveOperation.Receive(
        //        req.ApiToken,
        //        "d14016537b",
        //        "04efd5dd3d8ea53eb7c86062e47c8e10",
        //        "https://app.speckle.systems/"
        //    )
        //)
        //{
        //    if (request is PutNodeRequest putNodeRequest)
        //    {
        //        modelBuilder.AddNodes(putNodeRequest);
        //        if (
        //            new Length(
        //                putNodeRequest.LocationPoint.Y,
        //                putNodeRequest.LocationPoint.LengthUnit.MapEnumToLengthUnit()
        //            ).Millimeters > 10000
        //        )
        //        {
        //            modelBuilder.AddPointLoads(
        //                new PutPointLoadRequest()
        //                {
        //                    Id = putNodeRequest.Id,
        //                    Direction = new()
        //                    {
        //                        X = 0,
        //                        Y = -1,
        //                        Z = 0
        //                    },
        //                    Force = new(100, ForceUnitContract.Kilonewton),
        //                    NodeId = putNodeRequest.Id
        //                }
        //            );
        //        }
        //    }
        //    else if (request is PutElement1dRequest putElement1DRequest)
        //    {
        //        modelBuilder.AddElement1ds(putElement1DRequest);
        //    }
        //    else
        //    {
        //        throw new NotImplementedException(
        //            $"Type {request.GetType()} is not implemented in speckle receive endpoint"
        //        );
        //    }
        //}
    }

    public override IEnumerable<PutElement1dRequest> Element1dRequests() =>
        this.modelBuilder.Element1ds;

    public override IEnumerable<PutMaterialRequest> MaterialRequests() =>
        this.modelBuilder.Materials;

    public override IEnumerable<PutNodeRequest> NodeRequests() => this.modelBuilder.Nodes;

    public override IEnumerable<PutPointLoadRequest> PointLoadRequests() =>
        this.modelBuilder.PointLoads;

    public override IEnumerable<PutSectionProfileRequest> SectionProfileRequests() =>
        this.modelBuilder.SectionProfiles;

    public override IEnumerable<PutMomentLoadRequest> MomentLoadRequests() =>
        this.modelBuilder.MomentLoads;

    public NodeResultFixture[] ExpectedNodeResults { get; } =

        [
            new()
            {
                ResultSetId = 1,
                NodeId = 1,
                DisplacementAlongX = new(.7591, LengthUnit.Inch),
                DisplacementAlongY = new(.0358, LengthUnit.Inch),
                DisplacementAlongZ = new(-.1152, LengthUnit.Inch),
            },
            new() {
                ResultSetId = 1,
                NodeId = 50,
                ForceAlongX = new(6.306, ForceUnit.KilopoundForce),
                ForceAlongY = new(10.784, ForceUnit.KilopoundForce),
                ForceAlongZ = new(1.566, ForceUnit.KilopoundForce)
            },
            new() {
                ResultSetId = 1,
                NodeId = 100,
                ForceAlongX = new(-.896, ForceUnit.KilopoundForce),
                ForceAlongY = new(2.552, ForceUnit.KilopoundForce),
                ForceAlongZ = new(-9.632, ForceUnit.KilopoundForce)
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 150,
                DisplacementAlongX = new(-.60719, LengthUnit.Inch),
                DisplacementAlongY = new(.02335, LengthUnit.Inch),
                DisplacementAlongZ = new(-.4936, LengthUnit.Inch),
            },
            new() {
                ResultSetId = 1,
                NodeId = 200,
                ForceAlongX = new(-1.071, ForceUnit.KilopoundForce),
                ForceAlongY = new(12.826, ForceUnit.KilopoundForce),
                ForceAlongZ = new(-4.702, ForceUnit.KilopoundForce)
            },
            new() {
                ResultSetId = 1,
                NodeId = 300,
                DisplacementAlongX = new(-.0436, LengthUnit.Inch),
                DisplacementAlongY = new(-.75386, LengthUnit.Inch),
                DisplacementAlongZ = new(.54762, LengthUnit.Inch),
            },
            new()
            {
                ResultSetId = 1,
                NodeId = 330,
                DisplacementAlongX = new(1.90641, LengthUnit.Centimeter),
                DisplacementAlongY = new(-.04499, LengthUnit.Centimeter),
                DisplacementAlongZ = new(.57138, LengthUnit.Centimeter),
                RotationAboutX = new(-3.620e-4, AngleUnit.Radian),
                RotationAboutY = new(.00158, AngleUnit.Radian),
                RotationAboutZ = new(1.173e-4, AngleUnit.Radian)
            },
            new() {
                ResultSetId = 1,
                NodeId = 400,
                DisplacementAlongX = new(-.45241, LengthUnit.Inch),
                DisplacementAlongY = new(-.01802, LengthUnit.Inch),
                DisplacementAlongZ = new(-.53931, LengthUnit.Inch),
            },
            new() {
                ResultSetId = 1,
                NodeId = 500,
                DisplacementAlongX = new(.27826, LengthUnit.Inch),
                DisplacementAlongY = new(-.58624, LengthUnit.Inch),
                DisplacementAlongZ = new(-.79309, LengthUnit.Inch),
            },
        ];
}
