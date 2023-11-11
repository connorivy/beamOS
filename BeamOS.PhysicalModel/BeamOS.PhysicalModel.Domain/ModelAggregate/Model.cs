using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.PhysicalModel.Domain.ModelAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.NodeAggregate;
using BeamOS.PhysicalModel.Domain.NodeAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.Element1DAggregate;
using BeamOS.PhysicalModel.Domain.Element1DAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.MaterialAggregate;
using BeamOS.PhysicalModel.Domain.MaterialAggregate.ValueObjects;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate;
using BeamOS.PhysicalModel.Domain.SectionProfileAggregate.ValueObjects;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Domain.ModelAggregate;
public class Model : AggregateRoot<ModelId>
{
    public Model(
        string name,
        string description,
        ModelSettings settings,
        ModelId? id = null) : base(id ?? new())
    {
        this.Name = name;
        this.Description = description;
        this.Settings = settings;
    }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public ModelSettings Settings { get; private set; }

    private readonly List<NodeId> nodeIds = [];
    public IReadOnlyList<NodeId> NodeIds => this.nodeIds.AsReadOnly();

    private readonly List<Element1DId> element1DIds = [];
    public IReadOnlyList<Element1DId> Element1DIds => this.element1DIds.AsReadOnly();

    private readonly List<MaterialId> materialIds = [];
    public IReadOnlyList<MaterialId> MaterialIds => this.materialIds.AsReadOnly();

    private readonly List<SectionProfileId> sectionProfileIds = [];
    public IReadOnlyList<SectionProfileId> SectionProfileIds => this.sectionProfileIds.AsReadOnly();

    public Node AddNode(
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit? coordinateLengthUnit = null,
        Restraints? restraint = null
        )
    {
        LengthUnit lengthUnit = coordinateLengthUnit ?? this.Settings.UnitSettings.LengthUnit;
        Node node = new(this.Id, xCoordinate, yCoordinate, zCoordinate, lengthUnit, restraint);

        return this.AddNode(node);
    }
    public Node AddNode(Node node)
    {
        this.nodeIds.Add(node.Id);

        return node;
    }

    public Element1D AddElement1D(
        NodeId startNodeId,
        NodeId endNodeId,
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
