using BeamOs.Common.Domain.Models;
using UnitsNet;

namespace BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;

public class MaterialData : BeamOSValueObject
{
    public MaterialData(Pressure modulusOfElasticity, Pressure modulusOfRigidity)
    {
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
    }

    public Pressure ModulusOfElasticity { get; private set; }
    public Pressure ModulusOfRigidity { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.ModulusOfElasticity;
        yield return this.ModulusOfRigidity;
    }

    public static MaterialData Undefined { get; } = new(new(), new());
}
