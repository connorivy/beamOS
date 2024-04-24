using BeamOs.Domain.Common.Models;
using MathNet.Numerics.LinearAlgebra;
using UnitsNet;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class MomentLoadReadModel : BeamOSEntity<Guid>
{
    public Guid NodeId { get; private set; }
    public Torque Torque { get; private set; }
    public Vector<double> NormalizedAxisDirection { get; private set; }
    public NodeReadModel? Node { get; private set; }
}
