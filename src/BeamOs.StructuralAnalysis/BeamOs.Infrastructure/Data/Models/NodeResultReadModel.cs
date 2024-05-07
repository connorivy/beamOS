using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class NodeResultReadModel : BeamOSEntity<Guid>
{
    public Guid ModelId { get; private set; }
    public Guid NodeId { get; private set; }
    public NodeReadModel? Node { get; private set; }
    public Forces Forces { get; private set; }
    public Displacements Displacements { get; private set; }
}
