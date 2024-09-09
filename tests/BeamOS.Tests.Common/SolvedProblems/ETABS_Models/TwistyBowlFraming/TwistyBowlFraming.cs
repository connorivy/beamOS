using System.Reflection;
using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.SpeckleConnector;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Fixtures.Mappers.ToDomain;
using BeamOS.Tests.Common.Interfaces;
using MathNet.Spatial.Euclidean;
using Speckle.Core.Credentials;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.ETABS_Models.TwistyBowlFraming;

public class TwistyBowlFraming
    : CreateModelRequestBuilder,
        IHasSourceInfo,
        IHasExpectedNodeDisplacementResults
{
    public override Guid ModelGuid { get; } = Guid.Parse("f30e580d-9cb0-46d2-ade1-a4140c454632");
    public override PhysicalModelSettings Settings { get; } = new(UnitSettingsDtoVerbose.kN_M);

    public SourceInfo SourceInfo { get; } =
        new("SAP2000", FixtureSourceType.SAP2000, nameof(TwistyBowlFraming));

    //private readonly HashSet<string> addedNodeIds = [];

    public override async Task InitializeAsync()
    {
        this.PopulateFromJson(
            Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "TwistyBowlFraming.json"
            )
        );

        //this.CreateMaterialAndSectionProfile();
        //await foreach (
        //    var builder in SpeckleConnector.ReceiveData(
        //        AccountManager.GetAccounts().First(),
        //        "e6b1988124",
        //        "f404f297534f6bd4502a42cb1dd08f21"
        //    )
        //)
        //{
        //    if (builder is CreateNodeRequestBuilder nodeRequestBuilder)
        //    {
        //        if (
        //            nodeRequestBuilder.LocationPoint.YCoordinate.Value > 10000
        //            && this.addedNodeIds.Add(nodeRequestBuilder.Id.ToString())
        //        )
        //        {
        //            this.AddPointLoad(
        //                new()
        //                {
        //                    NodeId = nodeRequestBuilder.Id,
        //                    Direction = UnitVector3D.Create(0, -1, 0),
        //                    Force = new(100, ForceUnit.Kilonewton)
        //                }
        //            );
        //        }
        //        this.AddNode(nodeRequestBuilder);
        //    }
        //    else if (builder is CreateElement1dRequestBuilder element1DRequestBuilder)
        //    {
        //        var comparisonResult = string.Compare(
        //            element1DRequestBuilder.StartNodeId.ToString(),
        //            element1DRequestBuilder.EndNodeId.ToString(),
        //            StringComparison.Ordinal
        //        );

        //        string elementId;
        //        if (comparisonResult == 0)
        //        {
        //            continue;
        //        }
        //        else if (comparisonResult > 0)
        //        {
        //            elementId =
        //                $"n{element1DRequestBuilder.StartNodeId}n{element1DRequestBuilder.EndNodeId}";
        //        }
        //        else
        //        {
        //            elementId =
        //                $"n{element1DRequestBuilder.EndNodeId}n{element1DRequestBuilder.StartNodeId}";
        //        }

        //        this.AddElement1d(
        //            element1DRequestBuilder with
        //            {
        //                Id = elementId,
        //                MaterialId = "A992Steel",
        //                SectionProfileId = "W16x36"
        //            }
        //        );
        //    }
        //}
    }

    //private void CreateMaterialAndSectionProfile()
    //{
    //    this.AddMaterial(
    //        new()
    //        {
    //            ModulusOfElasticity = new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
    //            ModulusOfRigidity = new Pressure(
    //                11_153.85,
    //                PressureUnit.KilopoundForcePerSquareInch
    //            ),
    //            Id = "A992Steel"
    //        }
    //    );

    //    this.AddSectionProfile(
    //        new()
    //        {
    //            Area = new Area(10.6, AreaUnit.SquareInch),
    //            StrongAxisMomentOfInertia = new AreaMomentOfInertia(
    //                448,
    //                AreaMomentOfInertiaUnit.InchToTheFourth
    //            ),
    //            WeakAxisMomentOfInertia = new AreaMomentOfInertia(
    //                24.5,
    //                AreaMomentOfInertiaUnit.InchToTheFourth
    //            ),
    //            PolarMomentOfInertia = new AreaMomentOfInertia(
    //                .55,
    //                AreaMomentOfInertiaUnit.InchToTheFourth
    //            ),
    //            StrongAxisShearArea = new Area(5.0095, AreaUnit.SquareInch),
    //            WeakAxisShearArea = new Area(4.6905, AreaUnit.SquareInch),
    //            Id = "W16x36"
    //        }
    //    );
    //}

    public static TwistyBowlFraming Instance { get; } = new();
    public NodeResultFixture[] ExpectedNodeDisplacementResults { get; } =

        [
            new()
            {
                NodeId = "330",
                DisplacementAlongX = new(1.90641, LengthUnit.Centimeter),
                DisplacementAlongY = new(-.04499, LengthUnit.Centimeter),
                DisplacementAlongZ = new(.57138, LengthUnit.Centimeter),
                RotationAboutX = new(-3.620e-4, AngleUnit.Radian),
                RotationAboutY = new(.00158, AngleUnit.Radian),
                RotationAboutZ = new(1.173e-4, AngleUnit.Radian)
            }
        ];
}
