using BeamOS.Common.Domain.Models;
using BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.ModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.NodeAggregate.ValueObjects;

namespace BeamOS.DirectStiffnessMethod.Domain.ModelAggregate;
public class Model : BeamOSEntity<ModelId>
{
    public Model(
        ModelId id,
        UnitSettings unitSettings
    ) : base(id)
    {
        this.UnitSettings = unitSettings;
        //this.NodeIds = nodeIds;
        //this.Element1DIds = element1DIds;

    }

    public static Model Create(
        UnitSettings unitSettings
    )
    {
        //return new(AnalyticalModelId.CreateUnique(), unitSettings, nodeIds, element1DIds);
        return new(ModelId.CreateUnique(), unitSettings);
    }

    public UnitSettings UnitSettings { get; set; }
    //public List<AnalyticalNodeId> NodeIds { get; set; }
    //public List<Element1DId> Element1DIds { get; set; }
    private readonly ModelSettings settings;
    private readonly List<NodeId> analyticalNodeIds = [];
    public IReadOnlyList<NodeId> AnalyticalNodeIds => this.analyticalNodeIds.AsReadOnly();
    private readonly List<Element1DId> element1DIds = [];
    public IReadOnlyList<Element1DId> Element1DIds => this.element1DIds.AsReadOnly();
}
