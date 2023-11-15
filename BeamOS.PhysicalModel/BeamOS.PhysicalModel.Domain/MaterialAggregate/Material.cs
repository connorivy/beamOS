using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.MaterialAggregate;
public class Material : AggregateRoot<MaterialId>
{
    private Material(Pressure modulusOfElasticity, Pressure modulusOfRigidity, MaterialId? id = null)
        : base(id ?? new())
    {
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
    }
    public Pressure ModulusOfElasticity { get; set; }
    public Pressure ModulusOfRigidity { get; set; }
}
