using BeamOS.Common.Domain.Models;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate.ValueObjects;
public class Material : BeamOSValueObject
{
    public Material(Pressure modulusOfElasticity, Pressure modulusOfRigidity)
    {
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
    }

    public Pressure ModulusOfElasticity { get; set; }
    public Pressure ModulusOfRigidity { get; set; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.ModulusOfElasticity;
        yield return this.ModulusOfRigidity;
    }
}
