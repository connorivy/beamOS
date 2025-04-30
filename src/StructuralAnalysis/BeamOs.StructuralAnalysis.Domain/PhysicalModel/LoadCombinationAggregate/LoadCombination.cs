using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinationAggregate;

public class LoadCombination : BeamOsModelEntity<LoadCombinationId>
{
    public LoadCombination(ModelId modelId, string name, LoadCombinationId? id = null)
        : base(id ?? new(), modelId)
    {
        this.Name = name;
    }

    public string Name { get; set; }

    public List<LoadCaseInfo> LoadCases { get; set; } = [];

    [Obsolete("EF Core Constructor", true)]
    protected LoadCombination()
        : base() { }
}

public class LoadCaseInfo(LoadCaseId id, double factor)
{
    public LoadCaseId Id { get; set; } = id;
    public double Factor { get; set; } = factor;
}
