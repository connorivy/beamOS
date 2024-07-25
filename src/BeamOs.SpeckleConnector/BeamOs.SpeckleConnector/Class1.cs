using BeamOs.ApiClient.Builders;
using BeamOs.Contracts.PhysicalModel.Node;
using Speckle.Core.Credentials;
using Speckle.Core.Models;
using Speckle.Core.Models.Collections;
using Speckle.Core.Models.GraphTraversal;
using Speckle.Core.Transports;

namespace BeamOs.SpeckleConnector;

public static class SpeckleConnector
{
    public static async IAsyncEnumerable<CreateModelEntityRequestBuilderBase> ReceiveData(
        string streamId,
        string objectId
    )
    {
        Account account = AccountManager.GetAccounts().First();

        //var urlChunks = url.Split('/');
        //using Client apiClient = new(account);
        //var version = await apiClient
        //  .CommitGet(urlChunks[^3], urlChunks[^1])
        //  .ConfigureAwait(false);

        using ServerTransport transport = new(account, streamId);
        Base commitObject = await Speckle
            .Core
            .Api
            .Operations
            .Receive(objectId, transport)
            .ConfigureAwait(false);

        var objectsGraph = DefaultTraversal
            .CreateTraversalFunc()
            .Traverse(commitObject)
            .Where(obj => obj.Current is not Collection);

        foreach (TraversalContext tc in objectsGraph)
        {
            switch (tc.Current)
            {
                case Objects.Structural.Geometry.Node node:
                    yield return new CreateNodeRequestBuilder()
                    {
                        Id = node.name,
                        LocationPoint = new(
                            node.basePoint.x,
                            node.basePoint.z,
                            node.basePoint.y,
                            SpeckleUnitsToUnitsNet(node.basePoint.units)
                        ),
                        Restraint =
                            node.basePoint.z < .001
                                ? RestraintRequest.Pinned
                                : RestraintRequest.Free
                    };
                    continue;
                case Objects.Structural.Geometry.Element1D element1d:
                    yield return new CreateElement1dRequestBuilder()
                    {
                        StartNodeId = element1d.end1Node.name,
                        EndNodeId = element1d.end2Node.name,
                    };
                    yield return CreateNode(element1d.end1Node);
                    yield return CreateNode(element1d.end2Node);
                    continue;
            }
        }
    }

    private static CreateNodeRequestBuilder CreateNode(Objects.Structural.Geometry.Node node)
    {
        return new CreateNodeRequestBuilder()
        {
            Id = node.name,
            LocationPoint = new(
                node.basePoint.x,
                node.basePoint.z,
                node.basePoint.y,
                SpeckleUnitsToUnitsNet(node.basePoint.units)
            ),
            Restraint = node.basePoint.z < .001 ? RestraintRequest.Pinned : RestraintRequest.Free
        };
    }

    private static string SpeckleUnitsToUnitsNet(string speckleUnit)
    {
        return speckleUnit switch
        {
            "mm" => "Millimeter",
            "meter" => "Meter"
        };
    }
}
