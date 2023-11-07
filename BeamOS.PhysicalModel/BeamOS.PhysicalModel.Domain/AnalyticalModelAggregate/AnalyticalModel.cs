using BeamOS.Common.Domain.Models;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;

namespace BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate;
public class AnalyticalModel : AggregateRoot<AnalyticalModelId>
{
    private AnalyticalModel(
        AnalyticalModelId id,
        string name,
        string description,
        AnalyticalModelSettings settings) : base(id)
    {
        this.Name = name;
        this.Description = description;
        this.Settings = settings;
    }

    public static AnalyticalModel Create(
        string name,
        string description,
        AnalyticalModelSettings settings)
    {
        return new(AnalyticalModelId.CreateUnique(), name, description, settings);
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public AnalyticalModelSettings Settings { get; private set; }

    private readonly List<AnalyticalNodeId> analyticalNodeIds = [];
    public IReadOnlyList<AnalyticalNodeId> AnalyticalNodeIds => this.analyticalNodeIds.AsReadOnly();

    private readonly List<Element1DId> element1DIds = [];
    public IReadOnlyList<Element1DId> Element1DIds => this.element1DIds.AsReadOnly();

    private readonly List<MaterialId> materialIds = [];
    public IReadOnlyList<MaterialId> MaterialIds => this.materialIds.AsReadOnly();

    private readonly List<MaterialId> sectionProfileIds = [];
    public IReadOnlyList<MaterialId> SectionProfileIds => this.sectionProfileIds.AsReadOnly();
}
