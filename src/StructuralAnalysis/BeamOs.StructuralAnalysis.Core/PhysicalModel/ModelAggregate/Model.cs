using BeamOs.Common.Domain.Models;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCases;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MomentLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.PointLoadAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;

internal class Model : BeamOsEntity<ModelId>
{
    public Model(string name, string description, ModelSettings settings, ModelId? id = null)
        : base(id ?? new())
    {
        this.Name = name;
        this.Description = description;
        this.Settings = settings;
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public ModelSettings Settings { get; set; }
    public DateTimeOffset LastModified { get; set; } = DateTimeOffset.UtcNow;

    // public Octree? NodeOctree { get; private set; }
    // public OctreeId? NodeOctreeId { get; set; }

    // public double MinX { get; set; } = double.MaxValue;
    // public double MinY { get; set; } = double.MaxValue;
    // public double MinZ { get; set; } = double.MaxValue;
    // public double MaxX { get; set; } = double.MinValue;
    // public double MaxY { get; set; } = double.MinValue;
    // public double MaxZ { get; set; } = double.MinValue;

    public IList<Node>? Nodes { get; set; }
    public int MaxNodeId { get; set; }
    public IList<InternalNode>? InternalNodes { get; set; }
    public int MaxInternalNodeId { get; set; }
    public IList<Element1d>? Element1ds { get; set; }
    public int MaxElement1dId { get; set; }
    public IList<Material>? Materials { get; set; }
    public int MaxMaterialId { get; set; }
    public IList<SectionProfile>? SectionProfiles { get; set; }
    public int MaxSectionProfileId { get; set; }
    public IList<SectionProfileFromLibrary>? SectionProfilesFromLibrary { get; set; }
    public int MaxSectionProfileFromLibraryId { get; set; }
    public IList<PointLoad>? PointLoads { get; set; }
    public int MaxPointLoadId { get; set; }
    public IList<MomentLoad>? MomentLoads { get; set; }
    public int MaxMomentLoadId { get; set; }
    public IList<LoadCase>? LoadCases { get; set; }
    public int MaxLoadCaseId { get; set; }
    public IList<LoadCombination>? LoadCombinations { get; set; }
    public int MaxLoadCombinationId { get; set; }
    public IList<ResultSet>? ResultSets { get; set; }
    public IList<EnvelopeResultSet>? EnvelopeResultSets { get; set; }
    public IList<ModelProposal>? ModelProposals { get; set; }
    public int MaxModelProposalId { get; set; }

    // public void AddNode(Node node)
    // {
    //     this.Nodes ??= [];
    //     this.Nodes.Add(node);
    //     double x = node.LocationPoint.X.Meters;
    //     double y = node.LocationPoint.Y.Meters;
    //     double z = node.LocationPoint.Z.Meters;

    //     if (this.NodeOctree is null)
    //     {
    //         throw new InvalidOperationException("NodeOctree is not loaded.");
    //     }

    //     this.MinX = Math.Min(this.MinX, x);
    //     this.MinY = Math.Min(this.MinY, y);
    //     this.MinZ = Math.Min(this.MinZ, z);
    //     this.MaxX = Math.Max(this.MaxX, x);
    //     this.MaxY = Math.Max(this.MaxY, y);
    //     this.MaxZ = Math.Max(this.MaxZ, z);

    //     this.NodeOctree.Add(node);
    // }

    [Obsolete("EF Core Constructor", true)]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected Model() { }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
