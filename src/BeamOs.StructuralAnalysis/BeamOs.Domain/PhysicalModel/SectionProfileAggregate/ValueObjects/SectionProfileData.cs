using BeamOs.Domain.Common.Models;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;

public class SectionProfileData : BeamOSValueObject
{
    public SectionProfileData(
        Area area,
        AreaMomentOfInertia strongAxisMomentOfInertia,
        AreaMomentOfInertia weakAxisMomentOfInertia,
        AreaMomentOfInertia polarMomentOfInertia
    )
    {
        this.Area = area;
        this.StrongAxisMomentOfInertia = strongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = weakAxisMomentOfInertia;
        this.PolarMomentOfInertia = polarMomentOfInertia;
    }

    public Area Area { get; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Area;
        yield return this.StrongAxisMomentOfInertia;
        yield return this.WeakAxisMomentOfInertia;
        yield return this.PolarMomentOfInertia;
    }
}
