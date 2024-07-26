using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.SpeckleConnector;
using MathNet.Spatial.Euclidean;
using UnitsNet.Units;

namespace BeamOS.Tests.Common.SolvedProblems.ETABS_Models.TwistyBowlFraming;

public class TwistyBowlFraming : CreateModelRequestBuilder
{
    public override Guid ModelGuid { get; } = Guid.Parse("f30e580d-9cb0-46d2-ade1-a4140c454632");
    public override PhysicalModelSettings ModelSettings { get; } = new(UnitSettingsDtoVerbose.kN_M);

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
            if (
                builder is CreateNodeRequestBuilder nodeRequestBuilder
                && nodeRequestBuilder.LocationPoint.YCoordinate.Value > 10000
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
            this.AddElement(builder);
        }
    }

    private void CreateMaterialAndSectionProfile()
    {
        this.AddMaterial(
            new()
            {
                ModulusOfElasticity = new UnitsNet.Pressure(
                    29000 * .8,
                    PressureUnit.KilopoundForcePerSquareInch
                ),
                ModulusOfRigidity = new UnitsNet.Pressure(
                    11_460,
                    PressureUnit.KilopoundForcePerSquareInch
                )
            }
        );

        this.AddSectionProfile(
            new()
            {
                Area = new UnitsNet.Area(10.6, AreaUnit.SquareInch),
                StrongAxisMomentOfInertia = new UnitsNet.AreaMomentOfInertia(
                    448,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                WeakAxisMomentOfInertia = new UnitsNet.AreaMomentOfInertia(
                    24.5,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                ),
                PolarMomentOfInertia = new UnitsNet.AreaMomentOfInertia(
                    .55,
                    AreaMomentOfInertiaUnit.InchToTheFourth
                )
            }
        );
    }

    public static TwistyBowlFraming Instance { get; } = new();
}
