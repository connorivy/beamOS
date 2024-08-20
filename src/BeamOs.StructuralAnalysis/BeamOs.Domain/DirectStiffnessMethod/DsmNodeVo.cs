using BeamOs.Common.Domain.Models;
using BeamOs.Domain.DirectStiffnessMethod.Common.Extensions;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using UnitsNet.Units;

namespace BeamOs.Domain.DirectStiffnessMethod;

public class DsmNodeVo(Node node) : BeamOSValueObject
{
    public VectorIdentified GetForceVectorIdentifiedInGlobalCoordinates(
        ForceUnit forceUnit,
        TorqueUnit torqueUnit
    )
    {
        return new(
            node.Id.GetUnsupportedStructureDisplacementIds().ToList(),
            node.GetForcesInGlobalCoordinates().ToArray(forceUnit, torqueUnit)
        );
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return node;
    }
}
