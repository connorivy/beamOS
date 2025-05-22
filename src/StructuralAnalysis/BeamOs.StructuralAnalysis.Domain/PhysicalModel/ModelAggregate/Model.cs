using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
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
    private Octree? nodeOctree;

    public double MinX { get; set; } = double.MaxValue;
    public double MinY { get; set; } = double.MaxValue;
    public double MinZ { get; set; } = double.MaxValue;
    public double MaxX { get; set; } = double.MinValue;
    public double MaxY { get; set; } = double.MinValue;
    public double MaxZ { get; set; } = double.MinValue;

    public IList<Node>? Nodes { get; set; }
    public IList<Element1d>? Element1ds { get; init; }
    public IList<Material>? Materials { get; init; }
    public IList<SectionProfile>? SectionProfiles { get; init; }
    public IList<SectionProfileFromLibrary>? SectionProfilesFromLibrary { get; init; }
    public IList<PointLoad>? PointLoads { get; init; }

    public void AddNode(Node node)
    {
        this.Nodes ??= [node];
        double x = node.LocationPoint.X.Meters;
        double y = node.LocationPoint.Y.Meters;
        double z = node.LocationPoint.Z.Meters;

        if (this.nodeOctree == null)
        {
            this.MinX = this.MaxX = x;
            this.MinY = this.MaxY = y;
            this.MinZ = this.MaxZ = z;
            Point3D center = new(x, y, z);
            double halfSize = 1.0;
            this.nodeOctree = new Octree(this.Id, center, halfSize);
        }
        else
        {
            this.MinX = Math.Min(this.MinX, x);
            this.MinY = Math.Min(this.MinY, y);
            this.MinZ = Math.Min(this.MinZ, z);
            this.MaxX = Math.Max(this.MaxX, x);
            this.MaxY = Math.Max(this.MaxY, y);
            this.MaxZ = Math.Max(this.MaxZ, z);
        }
        this.nodeOctree.Add(node);
    }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Model() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
