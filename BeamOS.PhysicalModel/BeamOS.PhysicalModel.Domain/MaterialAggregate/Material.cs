using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using UnitsNet;

namespace BeamOS.PhysicalModel.Domain.MaterialAggregate;
public class Material : AggregateRoot<MaterialId>
{
    private Material(MaterialId id, Pressure modulusOfElasticity, Pressure modulusOfRigidity) : base(id)
    {
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
    }
    public static Material Create(Pressure modulusOfElasticity, Pressure modulusOfRigidity)
    {
        return new(new MaterialId(), modulusOfElasticity, modulusOfRigidity);
    }
    public Pressure ModulusOfElasticity { get; set; }
    public Pressure ModulusOfRigidity { get; set; }
}
