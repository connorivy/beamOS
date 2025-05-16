using BeamOs.Common.Api;
using BeamOs.Common.Contracts;
using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfiles;
using BeamOs.StructuralAnalysis.CsSdk.Mappers;
using BeamOs.StructuralAnalysis.Sdk;
using UnitsNet;

namespace BeamOs.SpeckleConnector;

[BeamOsRoute("speckle-receive")]
[BeamOsEndpointType(Http.Post)]
[BeamOsRequiredAuthorizationLevel(UserAuthorizationLevel.Authenticated)]
public class ConvertToBeamOs
    : BeamOsFromBodyResultBaseEndpoint<SpeckleReceiveParameters, BeamOsModelBuilderDto>
{
    public override async Task<Result<BeamOsModelBuilderDto>> ExecuteRequestAsync(
        SpeckleReceiveParameters req,
        CancellationToken ct = default
    )
    {
        BeamOsDynamicModelBuilder modelBuilder = new(
            "doesn't matter",
            new(UnitSettingsContract.kN_M),
            "doesn't matter",
            "doesn't matter"
        );

        modelBuilder.AddSectionProfilesFromLibrary(
            new SectionProfileFromLibrary()
            {
                Id = 1636,
                Name = "w16x36",
                Library = StructuralCode.AISC_360_16,
            }
        );

        modelBuilder.AddMaterials(
            new PutMaterialRequest()
            {
                ModulusOfElasticity = 29000,
                ModulusOfRigidity = 11_153.85,
                PressureUnit = PressureUnitContract.KilopoundForcePerSquareInch,
                Id = 992,
            }
        );

        var recieveOperation = new BeamOsSpeckleReceiveOperation()
        {
            Element1dRequestModifier = static el =>
                el with
                {
                    MaterialId = 992,
                    SectionProfileId = 1636,
                },
            NodeRequestModifier = static el =>
                el with
                {
                    LocationPoint = new(
                        el.LocationPoint.X,
                        el.LocationPoint.Z,
                        -el.LocationPoint.Y,
                        el.LocationPoint.LengthUnit
                    ),
                },
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
                            LoadCaseId = 1,
                            Direction = new()
                            {
                                X = 0,
                                Y = -1,
                                Z = 0,
                            },
                            Force = new(100, ForceUnitContract.Kilonewton),
                            NodeId = putNodeRequest.Id,
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
