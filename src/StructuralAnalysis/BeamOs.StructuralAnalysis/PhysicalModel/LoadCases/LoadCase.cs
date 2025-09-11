using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;

public class LoadCase : BeamOsModelEntity<LoadCaseId>
{
    public LoadCase(ModelId modelId, string name, LoadCaseId? id = null)
        : base(id ?? new(), modelId)
    {
        this.Name = name;
    }

    public string Name { get; set; }

    [Obsolete("EF Core Constructor", true)]
    protected LoadCase()
        : base() { }
}
