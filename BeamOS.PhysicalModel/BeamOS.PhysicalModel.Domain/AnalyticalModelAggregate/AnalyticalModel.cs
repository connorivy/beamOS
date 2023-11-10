using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate;
using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Domain.AnalyticalModelAggregate;
public class AnalyticalModel : AggregateRoot<AnalyticalModelId>
{
    public AnalyticalModel(
        string name,
        string description,
        AnalyticalModelSettings settings,
        AnalyticalModelId? id = null) : base(id ?? new())
    {
        this.Name = name;
        this.Description = description;
        this.Settings = settings;
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

    private readonly List<SectionProfileId> sectionProfileIds = [];
    public IReadOnlyList<SectionProfileId> SectionProfileIds => this.sectionProfileIds.AsReadOnly();

    public AnalyticalNode AddNode(
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit? coordinateLengthUnit = null,
        Restraints? restraint = null
        )
    {
        LengthUnit lengthUnit = coordinateLengthUnit ?? this.Settings.UnitSettings.LengthUnit;
        AnalyticalNode node = new(this.Id, xCoordinate, yCoordinate, zCoordinate, lengthUnit, restraint);

        return this.AddNode(node);
    }
    public AnalyticalNode AddNode(AnalyticalNode node)
    {
        this.analyticalNodeIds.Add(node.Id);

        return node;
    }

    public Element1D AddElement1D(
        AnalyticalNodeId startNodeId,
        AnalyticalNodeId endNodeId,
        MaterialId materialId,
        SectionProfileId sectionProfileId
        )
    {
        Element1D el = new(this.Id, startNodeId, endNodeId, materialId, sectionProfileId);
        return this.AddElement1D(el);
    }
    public Element1D AddElement1D(Element1D element1D)
    {
        this.element1DIds.Add(element1D.Id);

        return element1D;
    }

    public Material AddMaterial(Material material)
    {
        this.materialIds.Add(material.Id);

        return material;
    }

    public SectionProfile AddSectionProfile(SectionProfile sectionProfile)
    {
        this.sectionProfileIds.Add(sectionProfile.Id);

        return sectionProfile;
    }
}
