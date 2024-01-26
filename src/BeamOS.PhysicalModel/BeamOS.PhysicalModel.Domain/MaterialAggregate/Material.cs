using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.MaterialAggregate;

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
}
