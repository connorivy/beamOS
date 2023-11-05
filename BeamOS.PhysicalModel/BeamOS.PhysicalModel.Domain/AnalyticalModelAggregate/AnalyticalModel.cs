using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate;
internal class AnalyticalModel : AggregateRoot<AnalyticalModelId>
{
    public AnalyticalModel(AnalyticalModelId id) : base(id)
    {
    }

    private readonly AnalyticalModelSettings settings;
    private readonly List<AnalyticalNodeId> analyticalNodeIds = [];
    public IReadOnlyList<AnalyticalNodeId> AnalyticalNodeIds => this.analyticalNodeIds.AsReadOnly();
    private readonly List<Element1DId> element1DIds = [];
    public IReadOnlyList<Element1DId> Element1DIds => this.element1DIds.AsReadOnly();
}
