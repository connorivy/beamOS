using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.SpeckleConnector;
using BeamOS.Tests.Common.Fixtures;
using MathNet.Spatial.Euclidean;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.ETABS_Models.TwistyBowlFraming;

public class TwistyBowlFraming : CreateModelRequestBuilder, IHasExpectedNodeDisplacementResults
{
    public override Guid ModelGuid { get; } = Guid.Parse("f30e580d-9cb0-46d2-ade1-a4140c454632");
    public override PhysicalModelSettings Settings { get; } = new(UnitSettingsDtoVerbose.kN_M);

    private readonly HashSet<string> addedNodeIds = [];

    public override async Task InitializeAsync()
    {
        this.CreateMaterialAndSectionProfile();
        await foreach (
            var builder in SpeckleConnector.ReceiveData(
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
                this.AddElement1d(
                    element1DRequestBuilder with
                    {
                        MaterialId = "A992Steel",
                        SectionProfileId = "W16x36"
                    }
                );
            }

            //this.AddElement(builder);
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

    public static TwistyBowlFraming Instance { get; } = new();
    public NodeResultFixture[] ExpectedNodeDisplacementResults { get; }
}
