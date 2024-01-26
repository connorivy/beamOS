using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.SectionProfileAggregate;

public class SectionProfile : AggregateRoot<SectionProfileId>
{
    public SectionProfile(
        ModelId modelId,
        Area area,
        AreaMomentOfInertia strongAxisMomentOfInertia,
        AreaMomentOfInertia weakAxisMomentOfInertia,
        AreaMomentOfInertia polarMomentOfInertia,
        SectionProfileId? id = null
    )
        : base(id ?? new())
    {
        this.ModelId = modelId;
        this.Area = area;
        this.StrongAxisMomentOfInertia = strongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = weakAxisMomentOfInertia;
        this.PolarMomentOfInertia = polarMomentOfInertia;
    }

    public ModelId ModelId { get; set; }
    public Area Area { get; set; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; set; }
}
