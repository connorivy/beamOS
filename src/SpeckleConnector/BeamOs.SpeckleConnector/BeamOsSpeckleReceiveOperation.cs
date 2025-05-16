using System.Runtime.CompilerServices;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using Speckle.Core.Api.GraphQL.Models;
using Speckle.Core.Credentials;
using Speckle.Core.Logging;
using Speckle.Core.Models;
using Speckle.Core.Models.Extensions;
using Speckle.Core.Transports;

namespace BeamOs.SpeckleConnector;

public class BeamOsSpeckleReceiveOperation
{
    public Func<PutElement1dRequest, PutElement1dRequest>? Element1dRequestModifier { get; init; }
    public Func<PutNodeRequest, PutNodeRequest>? NodeRequestModifier { get; init; }

    private bool isBuilt;

    static BeamOsSpeckleReceiveOperation()
    {
        var speckleLoggerField =
            typeof(SpeckleLog).GetField(
                "s_logger",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
            ) ?? throw new ArgumentException("Could not find private speckle logger field");

        // we need to assign a value to the speckle logger because the initialize method throws an exception.
        // it tries to use an MD5 hash which is not implemented in webAssembly.
        speckleLoggerField.SetValue(null, Serilog.Core.Logger.None);
    }

    public async IAsyncEnumerable<IBeamOsEntityRequest> Receive(
        string apiToken,
        string projectId,
        string objectId,
        string serverUrl = "https://app.speckle.systems/",
        [EnumeratorCancellation] CancellationToken ct = default
    )
    {
        if (this.isBuilt)
        {
            throw new InvalidOperationException(
                "Receive object should only be for one receive operation"
            );
        }
        this.isBuilt = true;

        var account = new Account();
        account.token = apiToken;
        account.serverInfo = new ServerInfo { url = serverUrl };

        using ServerTransport transport = new(account, projectId);
        Base commitObject = await Speckle
            .Core.Api.Operations.Receive(
                objectId,
                transport,
                //localTransport: new DummyLocalTransport(),
                cancellationToken: ct
            )
            .ConfigureAwait(false);

        foreach (var item in commitObject.Flatten())
        {
            if (item is Objects.Structural.Geometry.Element1D element1d)
            {
                if (!this.nodeUniqueIdToBeamOsId.ContainsKey(element1d.end1Node.id))
                {
                    yield return this.ToBeamOs(element1d.end1Node);
                }
                if (!this.nodeUniqueIdToBeamOsId.ContainsKey(element1d.end2Node.id))
                {
                    yield return this.ToBeamOs(element1d.end2Node);
                }
                yield return this.ToBeamOs(element1d);
            }
            if (item is Objects.BuiltElements.Revit.RevitBeam revitBeam) { }
        }
    }

    private PutElement1dRequest ToBeamOs(Objects.Structural.Geometry.Element1D el)
    {
        if (!int.TryParse(el.name, out int id) && !int.TryParse(el.applicationId, out id))
        {
            id = el.id.GetHashCode();
        }

        var req = new PutElement1dRequest()
        {
            Id = id,
            StartNodeId = this.nodeUniqueIdToBeamOsId[el.end1Node.id],
            EndNodeId = this.nodeUniqueIdToBeamOsId[el.end2Node.id],
            MaterialId = 0, // todo
            SectionProfileId = 0, // todo
        };

        if (this.Element1dRequestModifier is not null)
        {
            req = this.Element1dRequestModifier(req);
        }
        return req;
    }

    private readonly Dictionary<string, int> nodeUniqueIdToBeamOsId = [];

    private PutNodeRequest ToBeamOs(Objects.Structural.Geometry.Node node)
    {
        if (!int.TryParse(node.name, out int id) && !int.TryParse(node.applicationId, out id))
        {
            id = node.id.GetHashCode();
        }
        this.nodeUniqueIdToBeamOsId.Add(node.id, id);

        var req = new PutNodeRequest()
        {
            Id = id,
            LocationPoint = new(
                node.basePoint.x,
                node.basePoint.y,
                node.basePoint.z,
                SpeckleUnitsToUnitsNet(node.basePoint.units)
            ),
            Restraint = ParseRestraintCode(node.restraint.code),
        };

        if (this.NodeRequestModifier is not null)
        {
            req = this.NodeRequestModifier(req);
        }
        return req;
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
            _ => throw new NotSupportedException(),
        };
    }
}

public class DummyLocalTransport : ITransport
{
    public string TransportName { get; set; } = "Not real";
    public Dictionary<string, object> TransportContext { get; } = [];
    public TimeSpan Elapsed => TimeSpan.Zero;
    public int SavedObjectCount => 0;
    public CancellationToken CancellationToken { get; set; }
    public Action<string, int>? OnProgressAction { get; set; }
    public Action<string, Exception>? OnErrorAction { get; set; }

    public void BeginWrite() { }

    public Task<string> CopyObjectAndChildren(
        string id,
        ITransport targetTransport,
        Action<int>? onTotalChildrenCountKnown = null
    ) => Task.FromResult("");

    public void EndWrite() { }

    public string? GetObject(string id) => null;

    public Task<Dictionary<string, bool>> HasObjects(IReadOnlyList<string> objectIds) =>
        Task.FromResult(objectIds.ToDictionary(x => x, x => false));

    public void SaveObject(string id, string serializedObject) { }

    public void SaveObject(string id, ITransport sourceTransport) { }

    public Task WriteComplete() => Task.CompletedTask;
}
