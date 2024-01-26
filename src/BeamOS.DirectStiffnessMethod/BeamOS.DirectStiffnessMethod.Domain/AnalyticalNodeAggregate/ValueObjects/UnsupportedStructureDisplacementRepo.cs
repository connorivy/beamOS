using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Models;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;

//public class UnsupportedStructureDisplacementRepo : BeamOSValueObject
//{
//    private readonly Dictionary<UnsupportedStructureDisplacementId, UnsupportedStructureDisplacement> dofIdentifierMap = [];
//    private readonly Dictionary<UnsupportedStructureDisplacementId, UnsupportedStructureDisplacement> restraintIdentifierMap = [];
//    public UnsupportedStructureDisplacementRepo(IEnumerable<AnalyticalNode> nodes)
//    {
//        this.InitializeIdentifierMaps(nodes);
//    }

//    private void InitializeIdentifierMaps(IEnumerable<AnalyticalNode> nodes)
//    {
//        List<(AnalyticalNodeId, CoordinateSystemDirection3D)> knownForces = [];
//        List<(AnalyticalNodeId, CoordinateSystemDirection3D)> knownDisplacements = [];

//        foreach (AnalyticalNode node in nodes)
//        {
//            PopulateKnownForcesAndDisplacementsOfNodeWithId(
//                node,
//                knownForces,
//                knownDisplacements);
//        }

//        this.PopulateIdentifierMaps(knownForces, knownDisplacements);
//    }

//    private static void PopulateKnownForcesAndDisplacementsOfNodeWithId(
//        AnalyticalNode node,
//        List<(AnalyticalNodeId, CoordinateSystemDirection3D)> knownForces,
//        List<(AnalyticalNodeId, CoordinateSystemDirection3D)> knownDisplacements)
//    {
//        foreach (CoordinateSystemDirection3D direction in Enum.GetValues(typeof(CoordinateSystemDirection3D)))
//        {
//            if (direction == CoordinateSystemDirection3D.Undefined)
//            {
//                continue;
//            }

//            // if nodeOrDisplacement is degree of freedom
//            if (node.Restraints.GetValueInDirection(direction) == true)
//            {
//                knownForces.Add((node.Id, direction));
//            }
//            else
//            {
//                knownDisplacements.Add((node.Id, direction));
//            }
//        }
//    }

//    private void PopulateIdentifierMaps(
//        List<(AnalyticalNodeId, CoordinateSystemDirection3D)> knownForces,
//        List<(AnalyticalNodeId, CoordinateSystemDirection3D)> knownDisplacements
//    )
//    {
//        foreach ((AnalyticalNodeId, CoordinateSystemDirection3D) nodeIdAndDirection in knownForces)
//        {
//            UnsupportedStructureDisplacementId identifier = new(this.dofIdentifierMap.Count + this.restraintIdentifierMap.Count);
//            this.dofIdentifierMap
//                .Add(identifier, new(identifier, nodeIdAndDirection.Item1, nodeIdAndDirection.Item2));
//        }
//        foreach ((AnalyticalNodeId, CoordinateSystemDirection3D) nodeIdAndDirection in knownDisplacements)
//        {
//            UnsupportedStructureDisplacementId identifier = new(this.dofIdentifierMap.Count + this.restraintIdentifierMap.Count);
//            this.restraintIdentifierMap
//                .Add(identifier, new(identifier, nodeIdAndDirection.Item1, nodeIdAndDirection.Item2));
//        }
//    }

//    public UnsupportedStructureDisplacement GetById(UnsupportedStructureDisplacementId id)
//    {
//        if (this.dofIdentifierMap.TryGetValue(id, out var dof))
//        {
//            return dof;
//        }
//        if (this.restraintIdentifierMap.TryGetValue(id, out var restraint))
//        {
//            return restraint;
//        }
//        throw new KeyNotFoundException();
//    }

//    public ICollection<UnsupportedStructureDisplacement> GetDOFs()
//    {
//        return this.dofIdentifierMap.Values;
//    }
//    public ICollection<UnsupportedStructureDisplacementId> GetDOFIdentifiers()
//    {
//        return this.dofIdentifierMap.Keys;
//    }
//    public ICollection<UnsupportedStructureDisplacement> GetRestraints()
//    {
//        return this.restraintIdentifierMap.Values;
//    }
//    public ICollection<UnsupportedStructureDisplacementId> GetRestraintIdentifiers()
//    {
//        return this.restraintIdentifierMap.Keys;
//    }
//    public IEnumerable<UnsupportedStructureDisplacementId> GetOrderedIdentifiersForElement1D(Element1D element)
//    {
//        return Enumerable.Concat(
//            this.GetOrderedIdentifiersForNode(element.StartNodeId),
//            this.GetOrderedIdentifiersForNode(element.EndNodeId)
//        );
//    }
//    public IEnumerable<UnsupportedStructureDisplacementId> GetOrderedIdentifiersForNode(AnalyticalNodeId nodeId)
//    {
//        foreach (CoordinateSystemDirection3D direction in Enum.GetValues(typeof(CoordinateSystemDirection3D)))
//        {
//            if (direction == CoordinateSystemDirection3D.Undefined)
//            {
//                continue;
//            }
//            foreach (KeyValuePair<UnsupportedStructureDisplacementId, UnsupportedStructureDisplacement> kvp in this.dofIdentifierMap)
//            {
//                if (kvp.Value.NodeId == nodeId && kvp.Value.Direction == direction)
//                {
//                    yield return kvp.Key;
//                    continue;
//                }
//            }
//            foreach (KeyValuePair<UnsupportedStructureDisplacementId, UnsupportedStructureDisplacement> kvp in this.restraintIdentifierMap)
//            {
//                if (kvp.Value.NodeId == nodeId && kvp.Value.Direction == direction)
//                {
//                    yield return kvp.Key;
//                    continue;
//                }
//            }
//        }
//    }

//    protected override IEnumerable<object> GetEqualityComponents() => throw new NotImplementedException();
//}

public class UnsupportedStructureDisplacementRepo : BeamOSValueObject
{
    public List<UnsupportedStructureDisplacementId> DegreeOfFreedomIds { get; } = [];
    public List<UnsupportedStructureDisplacementId> BoundaryConditionIds { get; } = [];

    public UnsupportedStructureDisplacementRepo(IEnumerable<AnalyticalNode> nodes)
    {
        this.InitializeIdentifierMaps(nodes);
    }

    private void InitializeIdentifierMaps(IEnumerable<AnalyticalNode> nodes)
    {
        foreach (var node in nodes)
        {
            foreach (
                CoordinateSystemDirection3D direction in Enum.GetValues(
                    typeof(CoordinateSystemDirection3D)
                )
            )
            {
                if (direction == CoordinateSystemDirection3D.Undefined)
                {
                    continue;
                }

                // if UnsupportedStructureDisplacement is degree of freedom
                if (node.Restraint.GetValueInDirection(direction) == true)
                {
                    this.DegreeOfFreedomIds.Add(new(node.Id, direction));
                }
                else
                {
                    this.BoundaryConditionIds.Add(new(node.Id, direction));
                }
            }
        }
    }

    protected override IEnumerable<object> GetEqualityComponents() =>
        throw new NotImplementedException();
}
