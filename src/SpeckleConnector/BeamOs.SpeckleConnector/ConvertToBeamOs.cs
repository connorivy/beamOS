using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.CsSdk;
using BeamOs.StructuralAnalysis.CsSdk.Mappers;
using UnitsNet;

namespace BeamOs.SpeckleConnector;

[BeamOsRoute("speckle-receive")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
public class ConvertToBeamOs
    : BeamOsFromBodyBaseEndpoint<SpeckleReceiveParameters, BeamOsModelBuilderDto>
{
    public override async Task<Result<BeamOsModelBuilderDto>> ExecuteRequestAsync(
        SpeckleReceiveParameters req,
        CancellationToken ct = default
    )
    {
        BeamOsDynamicModelBuilder modelBuilder =
            new(
                "doesn't matter",
                new(UnitSettingsContract.kN_M),
                "doesn't matter",
                "doesn't matter"
            );

        modelBuilder.AddSectionProfiles(
            new PutSectionProfileRequest()
            {
                Area = new AreaContract(10.6, AreaUnitContract.SquareInch),
                StrongAxisMomentOfInertia = new AreaMomentOfInertiaContract(
                    448,
                    AreaMomentOfInertiaUnitContract.InchToTheFourth
                ),
                WeakAxisMomentOfInertia = new AreaMomentOfInertiaContract(
                    24.5,
                    AreaMomentOfInertiaUnitContract.InchToTheFourth
                ),
                PolarMomentOfInertia = new AreaMomentOfInertiaContract(
                    .55,
                    AreaMomentOfInertiaUnitContract.InchToTheFourth
                ),
                StrongAxisShearArea = new AreaContract(5.0095, AreaUnitContract.SquareInch),
                WeakAxisShearArea = new AreaContract(4.6905, AreaUnitContract.SquareInch),
                Id = 1636
            }
        );

        modelBuilder.AddMaterials(
            new PutMaterialRequest()
            {
                ModulusOfElasticity = new PressureContract(
                    29000,
                    PressureUnitContract.KilopoundForcePerSquareInch
                ),
                ModulusOfRigidity = new PressureContract(
                    11_153.85,
                    PressureUnitContract.KilopoundForcePerSquareInch
                ),
                Id = 992
            }
        );

        var recieveOperation = new BeamOsSpeckleReceiveOperation()
        {
            Element1dRequestModifier = static el =>
                el with
                {
                    MaterialId = 992,
                    SectionProfileId = 1636
                },
            NodeRequestModifier = static el =>
                el with
                {
                    LocationPoint = new(
                        el.LocationPoint.X,
                        el.LocationPoint.Z,
                        -el.LocationPoint.Y,
                        el.LocationPoint.LengthUnit
                    )
                }
        };

        await foreach (
            IBeamOsEntityRequest request in recieveOperation.Receive(
                req.ApiToken,
                req.ProjectId,
                req.ObjectId,
                req.ServerUrl,
                ct
            )
        )
        {
            if (request is PutNodeRequest putNodeRequest)
            {
                modelBuilder.AddNodes(putNodeRequest);
                if (
                    new Length(
                        putNodeRequest.LocationPoint.Y,
                        putNodeRequest.LocationPoint.LengthUnit.MapEnumToLengthUnit()
                    ).Millimeters > 10000
                )
                {
                    modelBuilder.AddPointLoads(
                        new PutPointLoadRequest()
                        {
                            Id = putNodeRequest.Id,
                            Direction = new()
                            {
                                X = 0,
                                Y = -1,
                                Z = 0
                            },
                            Force = new(100, ForceUnitContract.Kilonewton),
                            NodeId = putNodeRequest.Id
                        }
                    );
                }
            }
            else if (request is PutElement1dRequest putElement1DRequest)
            {
                modelBuilder.AddElement1ds(putElement1DRequest);
            }
            else
            {
                throw new NotImplementedException(
                    $"Type {request.GetType()} is not implemented in speckle receive endpoint"
                );
            }
        }

        return modelBuilder.ToDto();
    }
}
