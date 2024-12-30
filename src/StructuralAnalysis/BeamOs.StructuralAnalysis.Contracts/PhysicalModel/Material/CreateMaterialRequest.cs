using System.Diagnostics.CodeAnalysis;
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
