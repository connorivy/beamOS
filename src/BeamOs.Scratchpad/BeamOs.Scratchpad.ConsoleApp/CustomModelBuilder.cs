using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using MathNet.Spatial.Euclidean;
using Speckle.Core.Credentials;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Scratchpad.ConsoleApp;

public class CustomModelBuilder : CreateModelRequestBuilder
{
    public string ScratchpadId => "Hpq0wU_9_wy-NjoGl1h3gA";

    public override Guid ModelGuid { get; } = Guid.Parse("00000000-0000-0000-0000-000000000000");
    public override PhysicalModelSettings Settings { get; } = new(UnitSettingsDtoVerbose.kN_M);

    private readonly HashSet<string> addedNodeIds = [];

    public override async Task InitializeAsync()
    {
        //this.PopulateFromJson(
        //    Path.Combine(
        //        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
        //        "TwistyBowlFraming.json"
        //    )
        //);

        this.CreateMaterialAndSectionProfile();
        await foreach (
            var builder in SpeckleConnector
                .SpeckleConnector
                .ReceiveData(
                    AccountManager.GetAccounts().First(),
                    "e6b1988124",
                    "f404f297534f6bd4502a42cb1dd08f21"
                )
        )
        {
            if (builder is CreateNodeRequestBuilder nodeRequestBuilder)
            {
                if (
                    nodeRequestBuilder.LocationPoint.YCoordinate.Value > 10000
                    && this.addedNodeIds.Add(nodeRequestBuilder.Id.ToString())
                )
                {
                    this.AddPointLoad(
                        new()
                        {
                            NodeId = nodeRequestBuilder.Id,
                            Direction = UnitVector3D.Create(0, -1, 0),
                            Force = new(100, ForceUnit.Kilonewton)
                        }
                    );
                }
                this.AddNode(nodeRequestBuilder);
            }
            else if (builder is CreateElement1dRequestBuilder element1DRequestBuilder)
            {
                var comparisonResult = string.Compare(
                    element1DRequestBuilder.StartNodeId.ToString(),
                    element1DRequestBuilder.EndNodeId.ToString(),
                    StringComparison.Ordinal
                );

                string elementId;
                if (comparisonResult == 0)
                {
                    continue;
                }
                else if (comparisonResult > 0)
                {
                    elementId =
                        $"n{element1DRequestBuilder.StartNodeId}n{element1DRequestBuilder.EndNodeId}";
                }
                else
                {
                    elementId =
                        $"n{element1DRequestBuilder.EndNodeId}n{element1DRequestBuilder.StartNodeId}";
                }

                this.AddElement1d(
                    element1DRequestBuilder with
                    {
                        Id = elementId,
                        MaterialId = "A992Steel",
                        SectionProfileId = "W16x36"
                    }
                );
            }
        }
    }

    private void CreateMaterialAndSectionProfile()
    {
        this.AddMaterial(
            new()
            {
                ModulusOfElasticity = new Pressure(29000, PressureUnit.KilopoundForcePerSquareInch),
                ModulusOfRigidity = new Pressure(
                    11_153.85,
                    PressureUnit.KilopoundForcePerSquareInch
                ),
                Id = "A992Steel"
            }
        );

        this.AddSectionProfile(
            new()
            {
                Area = new Area(10.6, AreaUnit.SquareInch),
                StrongAxisMomentOfInertia = new AreaMomentOfInertia(
                    448,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                WeakAxisMomentOfInertia = new AreaMomentOfInertia(
                    24.5,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                PolarMomentOfInertia = new AreaMomentOfInertia(
                    .55,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                StrongAxisShearArea = new Area(5.0095, AreaUnit.SquareInch),
                WeakAxisShearArea = new Area(4.6905, AreaUnit.SquareInch),
                Id = "W16x36"
            }
        );
    }
}
