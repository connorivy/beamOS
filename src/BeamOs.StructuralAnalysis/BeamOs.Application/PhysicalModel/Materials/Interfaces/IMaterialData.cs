using BeamOs.Application.Common.Interfaces;
using UnitsNet;

namespace BeamOs.Application.PhysicalModel.Materials.Interfaces;

public interface IMaterialData : IEntityData
{
    public Guid ModelId { get; }
    public Pressure ModulusOfElasticity { get; }
    public Pressure ModulusOfRigidity { get; }
}
