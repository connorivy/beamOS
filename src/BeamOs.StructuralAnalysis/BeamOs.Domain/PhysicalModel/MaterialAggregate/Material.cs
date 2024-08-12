using BeamOs.Common.Domain.Models;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.MaterialAggregate;

public class Material : AggregateRoot<MaterialId>
{
    public Material(
        ModelId modelId,
        Pressure modulusOfElasticity,
        Pressure modulusOfRigidity,
        MaterialId? id = null
    )
        : base(id ?? new())
    {
        this.ModelId = modelId;
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
    }

    public ModelId ModelId { get; private set; }
    public Pressure ModulusOfElasticity { get; private set; }
    public Pressure ModulusOfRigidity { get; private set; }

    public MaterialData GetData()
    {
        return new(this.ModulusOfElasticity, this.ModulusOfRigidity);
    }
}
