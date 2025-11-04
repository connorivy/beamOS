using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCases;

public record LoadCaseData
{
    [SetsRequiredMembers]
    public LoadCaseData(string name)
    {
        this.Name = name;
    }

    public LoadCaseData() { }

    public required string Name { get; set; }
}

public record LoadCase : LoadCaseData, IHasIntId
{
    public required int Id { get; init; }
}
