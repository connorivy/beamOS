using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.Common.Extensions;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.SectionProfileAggregate;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;

public class DsmElement1d(
    Element1dId element1dId,
    Angle sectionProfileRotation,
    Pressure modulusOfElasticity,
    Pressure modulusOfRigidity,
    Area area,
    AreaMomentOfInertia strongAxisMomentOfInertia,
    AreaMomentOfInertia weakAxisMomentOfInertia,
    AreaMomentOfInertia polarMomentOfInertia,
    Point startPoint,
    Point endPoint,
    NodeId startNodeId,
    NodeId endNodeId
) : IHydratedElement1d
{
    public DsmElement1d(Element1d element1d, SectionProfile sectionProfile)
        : this(
            element1d.Id,
            element1d.SectionProfileRotation,
            element1d.Material.ModulusOfElasticity,
            element1d.Material.ModulusOfRigidity,
            sectionProfile.Area,
            sectionProfile.StrongAxisMomentOfInertia,
            sectionProfile.WeakAxisMomentOfInertia,
            sectionProfile.PolarMomentOfInertia,
            element1d.StartNode.LocationPoint,
            element1d.EndNode.LocationPoint,
            element1d.StartNode.Id,
            element1d.EndNode.Id
        ) { }

    public Element1dId Element1dId { get; } = element1dId;
    public Angle SectionProfileRotation { get; } = sectionProfileRotation;
    public Pressure ModulusOfElasticity { get; } = modulusOfElasticity;
    public Pressure ModulusOfRigidity { get; } = modulusOfRigidity;
    public Area Area { get; } = area;
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; } = strongAxisMomentOfInertia;
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; } = weakAxisMomentOfInertia;
    public AreaMomentOfInertia PolarMomentOfInertia { get; } = polarMomentOfInertia;
    public Point StartPoint { get; } = startPoint;
    public Point EndPoint { get; } = endPoint;
    public Length Length { get; } = Line.GetLength(startPoint, endPoint);
    public NodeId StartNodeId { get; } = startNodeId;
    public NodeId EndNodeId { get; } = endNodeId;

    public (Length x, Length y, Length z) GetPointAlongBeam(double percentage) =>
        (
            this.StartPoint.X + (this.EndPoint.X - this.StartPoint.X) * percentage,
            this.StartPoint.Y + (this.EndPoint.Y - this.StartPoint.Y) * percentage,
            this.StartPoint.Z + (this.EndPoint.Z - this.StartPoint.Z) * percentage
        );

    public IEnumerable<UnsupportedStructureDisplacementId> GetUnsupportedStructureDisplacementIds()
    {
        for (var i = 0; i < 2; i++)
        {
            var nodeId = i == 0 ? this.StartNodeId : this.EndNodeId;
            foreach (
                CoordinateSystemDirection3D direction in Enum.GetValues<CoordinateSystemDirection3D>()
            )
            {
                if (direction == CoordinateSystemDirection3D.Undefined)
                {
                    continue;
                }
                yield return new(nodeId, direction);
            }
        }
    }

    //public void GetUnsupportedStructureDisplacementIds(
    //    Span<UnsupportedStructureDisplacementId> destinationSpan
    //)
    //{
    //    destinationSpan[0] = new(this.StartNodeId, CoordinateSystemDirection3D.AlongX);
    //    destinationSpan[1] = new(this.StartNodeId, CoordinateSystemDirection3D.AlongY);
    //    destinationSpan[2] = new(this.StartNodeId, CoordinateSystemDirection3D.AlongZ);
    //    destinationSpan[3] = new(this.StartNodeId, CoordinateSystemDirection3D.AboutX);
    //    destinationSpan[4] = new(this.StartNodeId, CoordinateSystemDirection3D.AboutY);
    //    destinationSpan[5] = new(this.StartNodeId, CoordinateSystemDirection3D.AboutZ);
    //    destinationSpan[6] = new(this.EndNodeId, CoordinateSystemDirection3D.AlongX);
    //    destinationSpan[7] = new(this.EndNodeId, CoordinateSystemDirection3D.AlongY);
    //    destinationSpan[8] = new(this.EndNodeId, CoordinateSystemDirection3D.AlongZ);
    //    destinationSpan[9] = new(this.EndNodeId, CoordinateSystemDirection3D.AboutX);
    //    destinationSpan[10] = new(this.EndNodeId, CoordinateSystemDirection3D.AboutY);
    //    destinationSpan[11] = new(this.EndNodeId, CoordinateSystemDirection3D.AboutZ);
    //}

    public double[,] GetRotationMatrix() =>
        Element1d.GetRotationMatrix(this.EndPoint, this.StartPoint, this.SectionProfileRotation);

    public Matrix<double> GetTransformationMatrix()
    {
        var rotationMatrix = DenseMatrix.OfArray(this.GetRotationMatrix());
        var transformationMatrix = Matrix<double>.Build.Dense(12, 12);
        transformationMatrix.SetSubMatrix(0, 0, rotationMatrix);
        transformationMatrix.SetSubMatrix(3, 3, rotationMatrix);
        transformationMatrix.SetSubMatrix(6, 6, rotationMatrix);
        transformationMatrix.SetSubMatrix(9, 9, rotationMatrix);
        return transformationMatrix;
    }

    public virtual Matrix<double> GetLocalStiffnessMatrix(
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
#pragma warning disable IDE1006 // Naming Styles
        var E = this.ModulusOfElasticity;
        var G = this.ModulusOfRigidity;
        var A = this.Area;
        var L = this.Length;
        var Is = this.StrongAxisMomentOfInertia;
        var Iw = this.WeakAxisMomentOfInertia;
        var J = this.PolarMomentOfInertia;
        var L2 = L * L;
        var L3 = L2 * L;

        // ForcePerLength (N/m, k/in)
        var ExA_L = (E * A / L).As(forcePerLengthUnit);
        var ExIs_L3 = E.MultiplyBy(Is.DivideBy(L3)).As(forcePerLengthUnit);
        var ExIw_L3 = E.MultiplyBy(Iw.DivideBy(L3)).As(forcePerLengthUnit);

        // Force (N, k)
        var ExIs_L2 = E.MultiplyBy(Is.DivideBy(L2)).As(forceUnit);
        var ExIw_L2 = E.MultiplyBy(Iw.DivideBy(L2)).As(forceUnit);

        // Torque (Nm, k-in)
        var ExIs_L = E.MultiplyBy(Is / L).As(torqueUnit);
        var ExIw_L = E.MultiplyBy(Iw / L).As(torqueUnit);
        var GxJ_L = G.MultiplyBy(J / L).As(torqueUnit);
#pragma warning restore IDE1006 // Naming Styles

        return DenseMatrix.OfArray(
            new[,]
            {
                { ExA_L, 0, 0, 0, 0, 0, -ExA_L, 0, 0, 0, 0, 0 },
                { 0, 12 * ExIs_L3, 0, 0, 0, 6 * ExIs_L2, 0, -12 * ExIs_L3, 0, 0, 0, 6 * ExIs_L2 },
                { 0, 0, 12 * ExIw_L3, 0, -6 * ExIw_L2, 0, 0, 0, -12 * ExIw_L3, 0, -6 * ExIw_L2, 0 },
                { 0, 0, 0, GxJ_L, 0, 0, 0, 0, 0, -GxJ_L, 0, 0 },
                { 0, 0, -6 * ExIw_L2, 0, 4 * ExIw_L, 0, 0, 0, 6 * ExIw_L2, 0, 2 * ExIw_L, 0 },
                { 0, 6 * ExIs_L2, 0, 0, 0, 4 * ExIs_L, 0, -6 * ExIs_L2, 0, 0, 0, 2 * ExIs_L },
                { -ExA_L, 0, 0, 0, 0, 0, ExA_L, 0, 0, 0, 0, 0 },
                { 0, -12 * ExIs_L3, 0, 0, 0, -6 * ExIs_L2, 0, 12 * ExIs_L3, 0, 0, 0, -6 * ExIs_L2 },
                { 0, 0, -12 * ExIw_L3, 0, 6 * ExIw_L2, 0, 0, 0, 12 * ExIw_L3, 0, 6 * ExIw_L2, 0 },
                { 0, 0, 0, -GxJ_L, 0, 0, 0, 0, 0, GxJ_L, 0, 0 },
                { 0, 0, -6 * ExIw_L2, 0, 2 * ExIw_L, 0, 0, 0, 6 * ExIw_L2, 0, 4 * ExIw_L, 0 },
                { 0, 6 * ExIs_L2, 0, 0, 0, 2 * ExIs_L, 0, -6 * ExIs_L2, 0, 0, 0, 4 * ExIs_L },
            }
        );
    }

    public Matrix<double> GetGlobalStiffnessMatrix(
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
        var transformationMatrix = this.GetTransformationMatrix();
        var localStiffnessMatrix = this.GetLocalStiffnessMatrix(
            forceUnit,
            forcePerLengthUnit,
            torqueUnit
        );
        return transformationMatrix.Transpose() * localStiffnessMatrix * transformationMatrix;
    }

    public MatrixIdentified GetGlobalStiffnessMatrixIdentified(
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
        return new(
            this.GetUnsupportedStructureDisplacementIds().ToList(),
            this.GetGlobalStiffnessMatrix(forceUnit, forcePerLengthUnit, torqueUnit).ToArray()
        );
    }

    private VectorIdentified ToVectorIdentified(Vector<double> vector)
    {
        return new(this.GetUnsupportedStructureDisplacementIds().ToList(), vector.ToArray());
    }

    public Vector<double> GetGlobalEndDisplacementVector(VectorIdentified jointDisplacementVector)
    {
        VectorIdentified globalEndDisplacementVector = new(
            this.GetUnsupportedStructureDisplacementIds().ToList()
        );
        globalEndDisplacementVector.AddEntriesWithMatchingIdentifiers(jointDisplacementVector);
        return globalEndDisplacementVector.Build();
    }

    public Vector<double> GetLocalEndDisplacementVector(VectorIdentified jointDisplacementVector)
    {
        return this.GetTransformationMatrix()
            * this.GetGlobalEndDisplacementVector(jointDisplacementVector);
    }

    public Vector<double> GetLocalMemberEndForcesVector(
        VectorIdentified jointDisplacementVector,
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
        return this.GetLocalStiffnessMatrix(forceUnit, forcePerLengthUnit, torqueUnit)
            * this.GetLocalEndDisplacementVector(jointDisplacementVector);
    }

    public Vector<double> GetGlobalMemberEndForcesVector(
        VectorIdentified jointDisplacementVector,
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
        return this.GetTransformationMatrix().Transpose()
            * this.GetLocalMemberEndForcesVector(
                jointDisplacementVector,
                forceUnit,
                forcePerLengthUnit,
                torqueUnit
            );
    }

    public VectorIdentified GetGlobalMemberEndForcesVectorIdentified(
        VectorIdentified jointDisplacementVector,
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
        return this.ToVectorIdentified(
            this.GetGlobalMemberEndForcesVector(
                jointDisplacementVector,
                forceUnit,
                forcePerLengthUnit,
                torqueUnit
            )
        );
    }
}
