using BeamOS.Common.Domain.Models;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.Element1DAggregate.ValueObjects;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
public class AnalyticalModel : BeamOSEntity<AnalyticalModelId>
{
    public AnalyticalModel(
        AnalyticalModelId id,
        UnitSettings unitSettings
        //List<AnalyticalNodeId> nodeIds,
        //List<Element1DId> element1DIds
    ) : base(id)
    {
        this.UnitSettings = unitSettings;
        //this.NodeIds = nodeIds;
        //this.Element1DIds = element1DIds;
    }

    public static AnalyticalModel Create(
        UnitSettings unitSettings
        //List<AnalyticalNodeId> nodeIds,
        //List<Element1DId> element1DIds
    )
    {
        //return new(AnalyticalModelId.CreateUnique(), unitSettings, nodeIds, element1DIds);
        return new(AnalyticalModelId.CreateUnique(), unitSettings);
    }

    public UnitSettings UnitSettings { get; set; }
    //public List<AnalyticalNodeId> NodeIds { get; set; }
    //public List<Element1DId> Element1DIds { get; set; }
    private readonly AnalyticalModelSettings settings;
    private readonly List<AnalyticalNodeId> analyticalNodeIds = [];
    public IReadOnlyList<AnalyticalNodeId> AnalyticalNodeIds => this.analyticalNodeIds.AsReadOnly();
    private readonly List<Element1DId> element1DIds = [];
    public IReadOnlyList<Element1DId> Element1DIds => this.element1DIds.AsReadOnly();
}
