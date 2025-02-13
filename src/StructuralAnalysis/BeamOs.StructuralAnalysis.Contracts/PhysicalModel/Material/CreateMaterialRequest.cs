using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.Common;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Material;

public record CreateMaterialRequest
{
    public CreateMaterialRequest() { }

    [SetsRequiredMembers]
    public CreateMaterialRequest(
        PressureContract modulusOfElasticity,
        PressureContract modulusOfRigidity,
        int? id = null
    )
    {
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
        this.Id = id;
    }

    public required PressureContract ModulusOfElasticity { get; init; }
    public required PressureContract ModulusOfRigidity { get; init; }
    public int? Id { get; init; }
}

public record MaterialRequestData
{
    public MaterialRequestData() { }

    [SetsRequiredMembers]
    public MaterialRequestData(
        PressureContract modulusOfElasticity,
        PressureContract modulusOfRigidity
    )
    {
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
    }

    public required PressureContract ModulusOfElasticity { get; init; }
    public required PressureContract ModulusOfRigidity { get; init; }
}

public record PutMaterialRequest : MaterialRequestData, IHasIntId
{
    public int Id { get; init; }
}
