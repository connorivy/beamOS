using BeamOs.Application.Common.Interfaces;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.SectionProfiles.Interfaces;

public interface ISectionProfileData : IEntityData
{
    public Guid ModelId { get; set; }
    public Area Area { get; set; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; set; }
    public Area StrongAxisShearArea { get; set; }
    public Area WeakAxisShearArea { get; set; }
}
