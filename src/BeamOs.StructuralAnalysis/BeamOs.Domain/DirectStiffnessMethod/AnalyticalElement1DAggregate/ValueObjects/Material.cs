using BeamOs.Domain.Common.Models;
using UnitsNet;

namespace BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;

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
