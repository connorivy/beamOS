using System.Text.Json.Serialization;
using BeamOs.Contracts.AnalyticalResults.Diagrams;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.Contracts.PhysicalModel.PointLoad;

namespace BeamOs.Contracts.PhysicalModel.Common;

[JsonPolymorphic]
[JsonDerivedType(typeof(ModelResponse), 0)]
[JsonDerivedType(typeof(Element1DResponse), 1)]
[JsonDerivedType(typeof(NodeResponse), 2)]
[JsonDerivedType(typeof(PointLoadResponse), 3)]
[JsonDerivedType(typeof(ShearDiagramResponse), 4)]
[JsonDerivedType(typeof(MomentDiagramResponse), 5)]
public abstract record BeamOsEntityContractBase(string Id) : BeamOsContractBase;
//public abstract record BeamOsEntityContractBase : BeamOsContractBase
//{
//    public string Id { get; init; }

//    [JsonConstructor]
//    public BeamOsEntityContractBase(string id)
//    {
//        this.Id = id;
//    }
//}
