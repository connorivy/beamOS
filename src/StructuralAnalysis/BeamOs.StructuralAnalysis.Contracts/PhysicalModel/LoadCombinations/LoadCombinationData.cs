using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;

public record LoadCombinationData
{
    [SetsRequiredMembers]
    public LoadCombinationData(Dictionary<int, double> loadCaseFactors)
    {
        this.LoadCaseFactors = loadCaseFactors;
    }

    [SetsRequiredMembers]
    public LoadCombinationData(params Span<(int, double)> loadCaseFactors)
        : this(ToDict(loadCaseFactors)) { }

    public LoadCombinationData() { }

    public required Dictionary<int, double> LoadCaseFactors { get; init; }

    protected static Dictionary<int, double> ToDict(Span<(int, double)> loadCaseFactors)
    {
        var dict = new Dictionary<int, double>(loadCaseFactors.Length);
        for (int i = 0; i < loadCaseFactors.Length; i++)
        {
            dict.Add(loadCaseFactors[i].Item1, loadCaseFactors[i].Item2);
        }
        return dict;
    }
}

public record LoadCombination : LoadCombinationData, IHasIntId
{
    public required int Id { get; init; }

    [SetsRequiredMembers]
    public LoadCombination(int id, Dictionary<int, double> loadCaseFactors)
    {
        this.Id = id;
        this.LoadCaseFactors = loadCaseFactors;
    }

    [SetsRequiredMembers]
    public LoadCombination(int id, params Span<(int, double)> loadCaseFactors)
        : this(id, ToDict(loadCaseFactors)) { }

    public LoadCombination() { }
}
