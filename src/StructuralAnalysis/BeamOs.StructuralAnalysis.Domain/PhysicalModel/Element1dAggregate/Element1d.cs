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
    public NodeDefinition? StartNode { get; set; }
    public NodeId EndNodeId { get; set; }
    public NodeDefinition? EndNode { get; set; }
    public MaterialId MaterialId { get; set; }
    public Material? Material { get; set; }
    public SectionProfileId SectionProfileId { get; set; }
    public SectionProfileInfoBase? SectionProfile { get; set; }
    public IList<InternalNode>? InternalNodes { get; set; }

    //public ICollection<ShearForceDiagram>? ShearForceDiagrams { get; init; }
    //public ICollection<MomentDiagram>? MomentDiagrams { get; init; }

    public Length Length => this.BaseLine.Length;

    public Line BaseLine
    {
        get
        {
            var (startNode, endNode) = this.GetNodesOrThrow();
            return new(startNode.GetLocationPoint(), endNode.GetLocationPoint());
        }
    }

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; set; }

    public static Line GetBaseLine(Point startPoint, Point endPoint)
    {
        return new(startPoint, endPoint);
    }

    public IEnumerable<Element1d> BreakBetweenInternalNodes(Func<Element1dId, int, int> idGenerator)
    {
        _ =
            this.InternalNodes
            ?? throw new InvalidOperationException(
                "InternalNodeIds must be set before breaking the element."
            );

        if (this.InternalNodes.Count == 0)
        {
            yield return this;
            yield break;
        }

        if (
            this.StartNode is null
            || this.EndNode is null
            || this.Material is null
            || this.SectionProfile is null
        )
        {
            throw new InvalidOperationException(
                "StartNode, EndNode, Material, and SectionProfile must be set before breaking the element."
            );
        }

        var startNode = this.StartNode.ToNode();
        var endNode = this.EndNode.ToNode();
        for (int i = 0; i < this.InternalNodes.Count; i++)
        {
            var internalNode = this.InternalNodes[i].ToNode();

            yield return CreateHydated(
                this.ModelId,
                startNode,
                internalNode,
                this.Material,
                this.SectionProfile,
                this.SectionProfileRotation,
                new Element1dId(idGenerator(this.Id, i))
            );
            startNode = internalNode;
        }

        yield return CreateHydated(
            this.ModelId,
            startNode,
            endNode,
            this.Material,
            this.SectionProfile,
            this.SectionProfileRotation,
            new Element1dId(idGenerator(this.Id, this.InternalNodes.Count))
        );
    }

    public static Element1d CreateHydated(
        ModelId modelId,
        NodeDefinition startNode,
        NodeDefinition endNode,
        Material material,
        SectionProfileInfoBase sectionProfile,
        Angle sectionProfileRotation,
        Element1dId id
    )
    {
        if (startNode is null || endNode is null)
        {
            throw new ArgumentNullException(
                "StartNode and EndNode must be set before creating an Element1d."
            );
        }

        return new Element1d(modelId, startNode.Id, endNode.Id, material.Id, sectionProfile.Id)
        {
            StartNode = startNode,
            EndNode = endNode,
            Material = material,
            SectionProfile = sectionProfile,
            SectionProfileRotation = sectionProfileRotation,
            Id = id,
        };
    }

    private (NodeDefinition StartNode, NodeDefinition EndNode) GetNodesOrThrow()
    {
        if (this.StartNode is null || this.EndNode is null)
        {
            throw new InvalidOperationException(
                "StartNode and EndNode must be set before accessing the nodes."
            );
        }
        return (this.StartNode, this.EndNode);
    }

    public double[,] GetRotationMatrix()
    {
        var (startNode, endNode) = this.GetNodesOrThrow();
        return GetRotationMatrix(
            endNode.GetLocationPoint(),
            startNode.GetLocationPoint(),
            this.SectionProfileRotation
        );
    }

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

    public double[,] GetTransformationMatrix()
    {
        var (startNode, endNode) = this.GetNodesOrThrow();
        return GetTransformationMatrix(
            endNode.GetLocationPoint(),
            startNode.GetLocationPoint(),
            this.SectionProfileRotation
        );
    }

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

    internal Point GetPointAtRatio(UnitsNet.Ratio ratioAlongElement1d)
    {
        var decimalFraction = ratioAlongElement1d.As(UnitsNet.Units.RatioUnit.DecimalFraction);
        if (decimalFraction is < 0 or > 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(ratioAlongElement1d),
                "Ratio along element must be between 0 and 1."
            );
        }

        var (startNode, endNode) = this.GetNodesOrThrow();
        var startLocation = startNode.GetLocationPoint();
        var endLocation = endNode.GetLocationPoint();

        var x = startLocation.X + (endLocation.X - startLocation.X) * decimalFraction;

        var y = startLocation.Y + (endLocation.Y - startLocation.Y) * decimalFraction;

        var z = startLocation.Z + (endLocation.Z - startLocation.Z) * decimalFraction;

        return new Point(x, y, z);
    }

    [Obsolete("EF Core Constructor", true)]
    protected Element1d()
        : base() { }
}
