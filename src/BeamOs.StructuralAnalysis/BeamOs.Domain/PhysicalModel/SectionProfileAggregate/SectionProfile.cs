using BeamOs.Domain.Common.Models;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.SectionProfileAggregate;

public class SectionProfile : AggregateRoot<SectionProfileId>
{
    public SectionProfile(
        ModelId modelId,
        Area area,
        AreaMomentOfInertia strongAxisMomentOfInertia,
        AreaMomentOfInertia weakAxisMomentOfInertia,
        AreaMomentOfInertia polarMomentOfInertia,
        Area strongAxisShearArea,
        Area weakAxisShearArea,
        SectionProfileId? id = null
    )
        : base(id ?? new())
    {
        this.ModelId = modelId;
        this.Area = area;
        this.StrongAxisMomentOfInertia = strongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = weakAxisMomentOfInertia;
        this.PolarMomentOfInertia = polarMomentOfInertia;
        this.StrongAxisShearArea = strongAxisShearArea;
        this.WeakAxisShearArea = weakAxisShearArea;
    }

    public ModelId ModelId { get; set; }
    public Area Area { get; set; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; set; }
    public Area StrongAxisShearArea { get; set; }
    public Area WeakAxisShearArea { get; set; }
}
