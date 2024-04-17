using UnitsNet;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class SectionProfileReadModel : ReadModelBase
{
    public Guid ModelId { get; set; }
    public Area Area { get; set; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; set; }
}
