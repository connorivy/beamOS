using BeamOs.Domain.Common.Models;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class MomentLoadReadModel : BeamOSEntity<Guid>
{
    public Guid ModelId { get; private set; }
    public Guid NodeId { get; private set; }
    public Torque Torque { get; private set; }
    public Vector<double> AxisDirection { get; private set; }
    public NodeReadModel? Node { get; private set; }
}
