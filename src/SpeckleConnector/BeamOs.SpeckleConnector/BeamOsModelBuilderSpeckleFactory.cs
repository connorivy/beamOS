using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1d;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.SectionProfile;
using BeamOs.StructuralAnalysis.CsSdk;
using Speckle.Core.Api.GraphQL.Models;
using Speckle.Core.Credentials;
using Speckle.Core.Models;
using Speckle.Core.Models.Extensions;
using Speckle.Core.Transports;

namespace BeamOs.SpeckleConnector;

public class BeamOsModelBuilderSpeckleFactory(
    string guidString,
    PhysicalModelSettings physicalModelSettings,
    string name,
    string description
)
{
    private readonly BeamOsDynamicModelBuilder modelBuilder =
        new(guidString, physicalModelSettings, name, description);

    private bool isBuilt;

    public async Task<BeamOsModelBuilder> Build(
        string apiToken,
        string projectId,
        string objectId,
        string serverUrl = "https://app.speckle.systems/",
        CancellationToken ct = default
    )
    {
        if (this.isBuilt)
        {
            throw new InvalidOperationException("Builder object should only be used once");
        }
        this.isBuilt = true;

        var account = new Account();
        account.token = apiToken;
        account.serverInfo = new ServerInfo { url = serverUrl };

        using ServerTransport transport = new(account, projectId);
        Base commitObject = await Speckle
            .Core
            .Api
            .Operations
            .Receive(objectId, transport, cancellationToken: ct)
            .ConfigureAwait(false);

        this.modelBuilder.AddSectionProfiles(
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

        this.modelBuilder.AddMaterials(
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

        foreach (var item in commitObject.Flatten())
        {
            if (item is Objects.Structural.Geometry.Element1D element1d)
            {
                this.AddNode(element1d.end1Node);
                this.AddNode(element1d.end2Node);

                int elId = int.Parse(element1d.name);
                this.modelBuilder.AddElement1ds(
                    new PutElement1dRequest()
                    {
                        Id = elId,
                        StartNodeId = this.nodeIncomingIdsToBeamOsIds[element1d.end1Node.id],
                        EndNodeId = this.nodeIncomingIdsToBeamOsIds[element1d.end2Node.id],
                        MaterialId = 992,
                        SectionProfileId = 1636
                    }
                );
            }
        }

        return this.modelBuilder;
    }

    //private readonly Lock @lock = new Lock();
    private readonly Dictionary<string, int> nodeIncomingIdsToBeamOsIds = [];

    private void AddNode(Objects.Structural.Geometry.Node node)
    {
        if (this.nodeIncomingIdsToBeamOsIds.ContainsKey(node.id))
        {
            return;
        }

        var id = this.nodeIncomingIdsToBeamOsIds.Count + 1;
        this.nodeIncomingIdsToBeamOsIds[node.id] = id;
        this.modelBuilder.AddNodes(
            new PutNodeRequest()
            {
                Id = id,
                LocationPoint = new(
                    node.basePoint.x,
                    node.basePoint.z,
                    -node.basePoint.y,
                    SpeckleUnitsToUnitsNet(node.basePoint.units)
                ),
                Restraint = ParseRestraintCode(node.restraint.code)
            }
        );

        if (node.basePoint.z > 10000)
        {
            this.modelBuilder.AddPointLoads(
                new PutPointLoadRequest()
                {
                    Id = id,
                    Direction = new()
                    {
                        X = 0,
                        Y = -1,
                        Z = 0
                    },
                    Force = new(100, ForceUnitContract.Kilonewton),
                    NodeId = id
                }
            );
        }
    }

    private static Restraint ParseRestraintCode(string restraintCode)
    {
        Span<bool> translatedCodes = stackalloc bool[6];
        for (int i = 0; i < 6; i++)
        {
            translatedCodes[i] = restraintCode[i] == 'R'; // release
        }

        return new(
            translatedCodes[0],
            translatedCodes[1],
            translatedCodes[2],
            translatedCodes[3],
            translatedCodes[4],
            translatedCodes[5]
        );
    }

    private static LengthUnitContract SpeckleUnitsToUnitsNet(string speckleUnit)
    {
        return speckleUnit switch
        {
            "mm" => LengthUnitContract.Millimeter,
            "meter" => LengthUnitContract.Meter,
            _ => throw new NotSupportedException()
        };
    }
}
