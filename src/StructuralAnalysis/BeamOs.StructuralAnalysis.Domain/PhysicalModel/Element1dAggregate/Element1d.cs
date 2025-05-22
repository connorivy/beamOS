using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

public class Element1d : BeamOsModelEntity<Element1dId>
{
    public Element1d(
        ModelId modelId,
        NodeId startNodeId,
        NodeId endNodeId,
        MaterialId materialId,
        SectionProfileId sectionProfileId,
        Element1dId? id = null
    )
        : base(id ?? new(), modelId)
    {
        this.ModelId = modelId;
        this.StartNodeId = startNodeId;
        this.EndNodeId = endNodeId;
        this.MaterialId = materialId;
        this.SectionProfileId = sectionProfileId;
    }

    public NodeId StartNodeId { get; set; }
    public Node? StartNode { get; set; }
    public NodeId EndNodeId { get; set; }
    public Node? EndNode { get; set; }
    public MaterialId MaterialId { get; set; }
    public Material? Material { get; set; }
    public SectionProfileId SectionProfileId { get; set; }
    public SectionProfileInfoBase? SectionProfile { get; set; }

    //public ICollection<ShearForceDiagram>? ShearForceDiagrams { get; init; }
    //public ICollection<MomentDiagram>? MomentDiagrams { get; init; }

    public Length Length => this.BaseLine.Length;

    public Line BaseLine => new(this.StartNode.LocationPoint, this.EndNode.LocationPoint);

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; set; }

    public static Line GetBaseLine(Point startPoint, Point endPoint)
    {
        return new(startPoint, endPoint);
    }

    //public void AddPointLoad(Ratio locationAlongBeam, ImmutablePointLoad pointLoad)
    //{
    //    if (locationAlongBeam.As(UnitsNet.Units.RatioUnit.DecimalFraction) is < 0 or > 1)
    //    {
    //        throw new ArgumentException("Provided location along beam must be between 0 and 1");
    //    }

    //    this.PointLoads.Add(locationAlongBeam, new(new(), pointLoad));
    //}

    public double[,] GetRotationMatrix() =>
        GetRotationMatrix(
            this.EndNode.LocationPoint,
            this.StartNode.LocationPoint,
            this.SectionProfileRotation
        );

    public static double[,] GetRotationMatrix(
        Point endLocation,
        Point startLocation,
        Angle sectionProfileRotation
    )
    {
        Length length = Line.GetLength(startLocation, endLocation);
        var rxx = (endLocation.X - startLocation.X) / length;
        var rxy = (endLocation.Y - startLocation.Y) / length;
        var rxz = (endLocation.Z - startLocation.Z) / length;

        var cosG = Math.Cos(sectionProfileRotation.Radians);
        var sinG = Math.Sin(sectionProfileRotation.Radians);

        var sqrtRxx2Rxz2 = Math.Sqrt((rxx * rxx) + (rxz * rxz));

        double r21,
            r22,
            r23,
            r31,
            r32,
            r33;

        if (sqrtRxx2Rxz2 < .0001)
        {
            r21 = -rxy * cosG;
            r22 = 0;
            r23 = sinG;
            r31 = rxy * sinG;
            r32 = 0;
            r33 = cosG;
        }
        else
        {
            r21 = ((-rxx * rxy * cosG) - (rxz * sinG)) / sqrtRxx2Rxz2;
            r22 = sqrtRxx2Rxz2 * cosG;
            r23 = ((-rxy * rxz * cosG) + (rxx * sinG)) / sqrtRxx2Rxz2;
            r31 = ((rxx * rxy * sinG) - (rxz * cosG)) / sqrtRxx2Rxz2;
            r32 = -sqrtRxx2Rxz2 * sinG;
            r33 = ((rxy * rxz * sinG) + (rxx * cosG)) / sqrtRxx2Rxz2;
        }

        return new[,]
        {
            { rxx, rxy, rxz },
            { r21, r22, r23 },
            { r31, r32, r33 },
        };
    }

    public double[,] GetTransformationMatrix() =>
        GetTransformationMatrix(
            this.EndNode.LocationPoint,
            this.StartNode.LocationPoint,
            this.SectionProfileRotation
        );

    public static double[,] GetTransformationMatrix(
        Point endLocation,
        Point startLocation,
        Angle sectionProfileRotation
    )
    {
        var rotationMatrix = GetRotationMatrix(endLocation, startLocation, sectionProfileRotation);
        var transformationMatrix = new double[12, 12];
        for (int i = 0; i < 4; i++)
        {
            int offset = i * 3;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    transformationMatrix[offset + row, offset + col] = rotationMatrix[row, col];
                }
            }
        }

        return transformationMatrix;
    }

    [Obsolete("EF Core Constructor", true)]
    protected Element1d()
        : base() { }
}

public sealed class Element1dProposal
    : BeamOsModelProposalEntity<Element1dProposalId, Element1d, Element1dId>
{
    public Element1dProposal(
        ModelId modelId,
        ModelProposalId modelProposalId,
        ExistingOrProposedNodeId startNodeId,
        ExistingOrProposedNodeId endNodeId,
        ExistingOrProposedMaterialId materialId,
        ExistingOrProposedSectionProfileId sectionProfileId,
        Element1dId? existingId = null,
        Element1dProposalId? id = null
    )
        : base(id ?? new(), modelProposalId, modelId, existingId)
    {
        this.StartNodeId = startNodeId;
        this.EndNodeId = endNodeId;
        this.MaterialId = materialId;
        this.SectionProfileId = sectionProfileId;
    }

    public Element1dProposal(
        Element1d element1d,
        ModelProposalId modelProposalId,
        ExistingOrProposedNodeId? startNodeId,
        ExistingOrProposedNodeId? endNodeId,
        ExistingOrProposedMaterialId? materialId,
        ExistingOrProposedSectionProfileId? sectionProfileId,
        Element1dProposalId? id = null
    )
        : this(
            element1d.ModelId,
            modelProposalId,
            startNodeId ?? new(element1d.StartNodeId),
            endNodeId ?? new(element1d.EndNodeId),
            materialId ?? new(element1d.MaterialId),
            sectionProfileId ?? new(element1d.SectionProfileId),
            element1d.Id,
            id
        ) { }

    public ExistingOrProposedNodeId StartNodeId { get; private set; }
    public ExistingOrProposedNodeId EndNodeId { get; private set; }
    public ExistingOrProposedMaterialId MaterialId { get; private set; }
    public ExistingOrProposedSectionProfileId SectionProfileId { get; private set; }

    public Element1d ToDomain(
        Dictionary<NodeProposalId, Node> nodeProposalIdToNewIdDict,
        Dictionary<MaterialProposalId, Material> materialProposalIdToNewIdDict,
        Dictionary<
            SectionProfileProposalId,
            SectionProfileInfoBase
        > sectionProfileProposalIdToNewIdDict
    )
    {
        var (startNodeId, startNode) = this.StartNodeId.ToIdAndEntity(nodeProposalIdToNewIdDict);
        var (endNodeId, endNode) = this.EndNodeId.ToIdAndEntity(nodeProposalIdToNewIdDict);
        var (materialId, material) = this.MaterialId.ToIdAndEntity(materialProposalIdToNewIdDict);
        var (sectionProfileId, sectionProfile) = this.SectionProfileId.ToIdAndEntity(
            sectionProfileProposalIdToNewIdDict
        );
        return new(
            this.ModelId,
            startNodeId,
            endNodeId,
            materialId,
            sectionProfileId,
            this.ExistingId
        )
        {
            StartNode = startNode,
            EndNode = endNode,
            Material = material,
            SectionProfile = sectionProfile,
        };
    }

    [Obsolete("EF Core Constructor")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private Element1dProposal()
        : base() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
