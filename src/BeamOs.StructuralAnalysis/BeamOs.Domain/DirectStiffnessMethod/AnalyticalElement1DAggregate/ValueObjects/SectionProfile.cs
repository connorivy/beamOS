using BeamOs.Domain.Common.Models;
using UnitsNet;

namespace BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;

public class SectionProfile : BeamOSValueObject
{
    public SectionProfile(
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

    public Area Area { get; set; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.Area;
        yield return this.StrongAxisMomentOfInertia;
        yield return this.WeakAxisMomentOfInertia;
        yield return this.PolarMomentOfInertia;
    }
}
