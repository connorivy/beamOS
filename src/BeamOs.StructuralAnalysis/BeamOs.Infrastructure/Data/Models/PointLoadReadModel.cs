using BeamOs.Domain.Common.Models;
using MathNet.Spatial.Euclidean;
using UnitsNet;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class PointLoadReadModel : BeamOSEntity<Guid>
{
    public Guid ModelId { get; private set; }
    public Guid NodeId { get; private set; }
    public Force Force { get; private set; }
    public Vector3D Direction { get; private set; }
    public NodeReadModel? Node { get; private set; }
}
