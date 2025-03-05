using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;

public record MaterialRequestData
{
    public MaterialRequestData() { }

    [SetsRequiredMembers]
    public MaterialRequestData(
        double modulusOfElasticity,
        double modulusOfRigidity,
        PressureUnitContract pressureUnit
    )
    {
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
        this.PressureUnit = pressureUnit;
    }

    public required double ModulusOfElasticity { get; init; }
    public required double ModulusOfRigidity { get; init; }
    public required PressureUnitContract PressureUnit { get; init; }
}

public record CreateMaterialRequest : MaterialRequestData
{
    public int? Id { get; init; }
}

public record PutMaterialRequest : MaterialRequestData, IHasIntId
{
    public int Id { get; init; }
}
