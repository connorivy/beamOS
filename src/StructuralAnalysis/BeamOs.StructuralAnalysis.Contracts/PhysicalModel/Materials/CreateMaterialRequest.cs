using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Materials;

public record MaterialData
{
    public MaterialData() { }

    [SetsRequiredMembers]
    public MaterialData(
        double modulusOfElasticity,
        double modulusOfRigidity,
        PressureUnit pressureUnit
    )
    {
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
        this.PressureUnit = pressureUnit;
    }

    public required double ModulusOfElasticity { get; init; }
    internal Pressure ModulusOfElasticityInternal =>
        new(this.ModulusOfElasticity, this.PressureUnit);
    public required double ModulusOfRigidity { get; init; }
    internal Pressure ModulusOfRigidityInternal => new(this.ModulusOfRigidity, this.PressureUnit);
    public required PressureUnit PressureUnit { get; init; }
}

public record CreateMaterialRequest : MaterialData
{
    public int? Id { get; init; }
}

public record PutMaterialRequest : MaterialData, IHasIntId
{
    public int Id { get; init; }
}
