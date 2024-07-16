using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.Extensions;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.DirectStiffnessMethod;

public class DsmNodeVo(Node node) : BeamOSValueObject
{
    public Forces GetForcesInGlobalCoordinates()
    {
        var forceAlongX = Force.Zero;
        var forceAlongY = Force.Zero;
        var forceAlongZ = Force.Zero;
        var momentAboutX = Torque.Zero;
        var momentAboutY = Torque.Zero;
        var momentAboutZ = Torque.Zero;

        foreach (var linearLoad in node.PointLoads)
        {
            forceAlongX += linearLoad.Force * linearLoad.Direction.X;
            forceAlongY += linearLoad.Force * linearLoad.Direction.Y;
            forceAlongZ += linearLoad.Force * linearLoad.Direction.Z;
        }
        foreach (var momentLoad in node.MomentLoads)
        {
            momentAboutX += momentLoad.Torque * momentLoad.AxisDirection[0];
            momentAboutY += momentLoad.Torque * momentLoad.AxisDirection[1];
            momentAboutZ += momentLoad.Torque * momentLoad.AxisDirection[2];
        }
        return new(forceAlongX, forceAlongY, forceAlongZ, momentAboutX, momentAboutY, momentAboutZ);
    }

    public VectorIdentified GetForceVectorIdentifiedInGlobalCoordinates(
        ForceUnit forceUnit,
        TorqueUnit torqueUnit
    )
    {
        return new(
            node.Id.GetUnsupportedStructureDisplacementIds().ToList(),
            this.GetForcesInGlobalCoordinates().ToArray(forceUnit, torqueUnit)
        );
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return node;
    }
}
