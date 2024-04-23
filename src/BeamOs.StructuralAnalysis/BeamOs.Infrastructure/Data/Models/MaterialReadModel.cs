using BeamOs.Application.PhysicalModel.Materials.Interfaces;
using UnitsNet;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class MaterialReadModel : ReadModelBase, IMaterialData
{
    public Guid ModelId { get; private set; }
    public Pressure ModulusOfElasticity { get; private set; }
    public Pressure ModulusOfRigidity { get; private set; }
}
