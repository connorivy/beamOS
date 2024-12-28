using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;

public class Material : BeamOsModelEntity<MaterialId>
{
    public Material(
        ModelId modelId,
        Pressure modulusOfElasticity,
        Pressure modulusOfRigidity,
        MaterialId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.ModelId = modelId;
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
    }

    public Pressure ModulusOfElasticity { get; private set; }
    public Pressure ModulusOfRigidity { get; private set; }
}
