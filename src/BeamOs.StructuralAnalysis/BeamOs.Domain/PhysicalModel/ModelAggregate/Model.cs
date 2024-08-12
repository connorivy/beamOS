using BeamOs.Common.Domain.Models;
using BeamOs.Domain.AnalyticalResults.ModelResultAggregate;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.Domain.PhysicalModel.MaterialAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.ModelAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using BeamOs.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate;
using BeamOs.Domain.PhysicalModel.SectionProfileAggregate.ValueObjects;
using UnitsNet.Units;

namespace BeamOs.Domain.PhysicalModel.ModelAggregate;

public class Model : AggregateRoot<ModelId>
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

    public ICollection<Node> Nodes { get; init; } = [];
    public ICollection<Element1D> Element1ds { get; init; } = [];
    public ICollection<Material> Materials { get; init; } = [];
    public ICollection<SectionProfile> SectionProfiles { get; init; } = [];
    public ICollection<ModelResult> ModelResults { get; init; }
    public ICollection<PointLoad>? PointLoads { get; init; }

    public Node AddNode(
        double xCoordinate,
        double yCoordinate,
        double zCoordinate,
        LengthUnit? coordinateLengthUnit = null,
        Restraint? restraint = null
    )
    {
        var lengthUnit = coordinateLengthUnit ?? this.Settings.UnitSettings.LengthUnit;
        Node node = new(this.Id, xCoordinate, yCoordinate, zCoordinate, lengthUnit);

        return AddNode(node);
    }

    public static Node AddNode(Node node)
    {
        //this.nodeIds.Add(node.Id);

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
        return AddElement1D(el);
    }

    public static Element1D AddElement1D(Element1D element1D)
    {
        //this.element1DIds.Add(element1D.Id);

        return element1D;
    }

    public static Material AddMaterial(Material material)
    {
        //this.materialIds.Add(material.Id);

        return material;
    }

    public static SectionProfile AddSectionProfile(SectionProfile sectionProfile)
    {
        //this.sectionProfileIds.Add(sectionProfile.Id);

        return sectionProfile;
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Model() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
