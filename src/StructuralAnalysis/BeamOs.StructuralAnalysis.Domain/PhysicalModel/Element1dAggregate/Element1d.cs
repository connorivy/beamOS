using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.MaterialAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using UnitsNet;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;

public class Element1d : BeamOsModelEntity<Element1dId>, IHydratedElement1d
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

    public NodeId StartNodeId { get; private set; }
    public Node? StartNode { get; set; }
    public NodeId EndNodeId { get; private set; }
    public Node? EndNode { get; set; }
    public MaterialId MaterialId { get; private set; }
    public Material? Material { get; set; }
    public SectionProfileId SectionProfileId { get; private set; }
    public SectionProfile? SectionProfile { get; set; }

    //public ICollection<ShearForceDiagram>? ShearForceDiagrams { get; init; }
    //public ICollection<MomentDiagram>? MomentDiagrams { get; init; }

    public Length Length => this.BaseLine.Length;

    public Line BaseLine => new(this.StartNode.LocationPoint, this.EndNode.LocationPoint);

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; set; }

    Area IHydratedElement1d.Area => this.SectionProfile.Area;
    Element1dId IHydratedElement1d.Element1dId => this.Id;
    Point IHydratedElement1d.StartPoint => this.StartNode.LocationPoint;
    Point IHydratedElement1d.EndPoint => this.EndNode.LocationPoint;
    Pressure IHydratedElement1d.ModulusOfElasticity => this.Material.ModulusOfElasticity;
    Pressure IHydratedElement1d.ModulusOfRigidity => this.Material.ModulusOfRigidity;
    AreaMomentOfInertia IHydratedElement1d.PolarMomentOfInertia =>
        this.SectionProfile.PolarMomentOfInertia;
    AreaMomentOfInertia IHydratedElement1d.StrongAxisMomentOfInertia =>
        this.SectionProfile.StrongAxisMomentOfInertia;
    AreaMomentOfInertia IHydratedElement1d.WeakAxisMomentOfInertia =>
        this.SectionProfile.WeakAxisMomentOfInertia;

    //private readonly SortedList<double, PointLoad> loads = new();
    //public IReadOnlyDictionary<double, PointLoad> Loads => this.loads.AsReadOnly();

    //public Line BaseLine { get; }
    //public Length Length => this.BaseLine.Length;
    //public Dictionary<Ratio, PointLoad> PointLoads { get; private set; } = [];

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
