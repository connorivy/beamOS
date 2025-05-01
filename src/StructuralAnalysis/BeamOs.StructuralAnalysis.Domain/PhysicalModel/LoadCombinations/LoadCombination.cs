using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;

public class LoadCombination : BeamOsModelEntity<LoadCombinationId>
{
    public LoadCombination(ModelId modelId, LoadCombinationId? id = null)
        : base(id ?? new(), modelId) { }

    public Dictionary<LoadCaseId, double> LoadCaseFactors { get; set; } = [];

    public double GetFactor(LoadCaseId loadCaseId) =>
        this.LoadCaseFactors.GetValueOrDefault(loadCaseId);

    public void SetFactor(LoadCaseId loadCaseId, double factor) =>
        this.LoadCaseFactors[loadCaseId] = factor;

    [Obsolete("EF Core Constructor", true)]
    protected LoadCombination()
        : base() { }
}
