using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

public class Model : BeamOsEntity<ModelId>
{
    public Model(string name, string description, ModelSettings settings, ModelId? id = null)
        : base(id ?? new())
    {
        this.Name = name;
        this.Description = description;
        this.Settings = settings;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public ModelSettings Settings { get; private set; }
    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;

    public IList<Node>? Nodes { get; init; }
    public IList<Element1d>? Element1ds { get; init; }
    public IList<Material>? Materials { get; init; }
    public IList<SectionProfile>? SectionProfiles { get; init; }
    public IList<PointLoad>? PointLoads { get; init; }
    public IList<MomentLoad>? MomentLoads { get; init; }
    public IList<ResultSet>? ResultSets { get; init; }

    //public AnalyticalResults? AnalyticalResults { get; init; }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Model() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
