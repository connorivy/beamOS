using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Extensions;
using BeamOS.Common.Domain.Models;
using BeamOS.Common.Domain.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
public class AnalyticalElement1D : AggregateRoot<AnalyticalElement1DId>
{
    public AnalyticalElement1D(
        AnalyticalElement1DId element1DId,
        Angle sectionProfileRotation,
        UnitSettings unitSettings,
        AnalyticalNode startNode,
        AnalyticalNode endNode,
        Material material,
        SectionProfile sectionProfile
    ) : base(element1DId)
    {
        this.SectionProfileRotation = sectionProfileRotation;
        this.UnitSettings = unitSettings;

        this.StartNodeId = startNode.Id;
        this.EndNodeId = endNode.Id;

        this.ModulusOfElasticity = material.ModulusOfElasticity;
        this.ModulusOfRigidity = material.ModulusOfRigidity;

        this.Area = sectionProfile.Area;
        this.StrongAxisMomentOfInertia = sectionProfile.StrongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = sectionProfile.WeakAxisMomentOfInertia;
        this.PolarMomentOfInertia = sectionProfile.PolarMomentOfInertia;

        this.BaseLine = GetBaseLine(startNode.LocationPoint, endNode.LocationPoint);
    }

    public static AnalyticalElement1D Create(
        Angle sectionProfileRotation,
        UnitSettings unitSettings,
        AnalyticalNode startNode,
        AnalyticalNode endNode,
        Material material,
        SectionProfile sectionProfile)
    {
        return new(AnalyticalElement1DId.CreateUnique(), sectionProfileRotation, unitSettings, startNode, endNode, material, sectionProfile);
    }

    public Angle SectionProfileRotation { get; set; }
    public UnitSettings UnitSettings { get; set; }
    //public List<Load> Load { get; private set; }

    public Pressure ModulusOfElasticity { get; set; }
    public Pressure ModulusOfRigidity { get; set; }

    public Area Area { get; set; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; set; }

    public Line BaseLine { get; }
    public Length Length => this.BaseLine.Length;

    public AnalyticalNodeId StartNodeId { get; set; }
    public AnalyticalNodeId EndNodeId { get; set; }

    public static Line GetBaseLine(Point startPoint, Point endPoint)
    {
        return new(startPoint, endPoint);
    }

    public IEnumerable<UnsupportedStructureDisplacementId> GetUnsupportedStructureDisplacementIds()
    {
        for (var i = 0; i < 2; i++)
        {
            var nodeId = i == 0 ? this.StartNodeId : this.EndNodeId;
            foreach (CoordinateSystemDirection3D direction in Enum.GetValues(typeof(CoordinateSystemDirection3D)))
            {
                if (direction == CoordinateSystemDirection3D.Undefined)
                {
                    continue;
                }
                yield return new(nodeId, direction);
            }
        }
    }

    public Matrix<double> GetRotationMatrix()
    {
        var rxx = (this.BaseLine.EndPoint.XCoordinate - this.BaseLine.StartPoint.XCoordinate) / this.Length;
        var rxy = (this.BaseLine.EndPoint.YCoordinate - this.BaseLine.StartPoint.YCoordinate) / this.Length;
        var rxz = (this.BaseLine.EndPoint.ZCoordinate - this.BaseLine.StartPoint.ZCoordinate) / this.Length;

        var cosG = Math.Cos(this.SectionProfileRotation.Radians);
        var sinG = Math.Sin(this.SectionProfileRotation.Radians);

        var sqrtRxx2Rxz2 = Math.Sqrt(rxx * rxx + rxz * rxz);

        double r21, r22, r23, r31, r32, r33;

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
            r21 = (-rxx * rxy * cosG - rxz * sinG) / sqrtRxx2Rxz2;
            r22 = sqrtRxx2Rxz2 * cosG;
            r23 = (-rxy * rxz * cosG + rxx * sinG) / sqrtRxx2Rxz2;
            r31 = (rxx * rxy * sinG - rxz * cosG) / sqrtRxx2Rxz2;
            r32 = -sqrtRxx2Rxz2 * sinG;
            r33 = (rxy * rxz * sinG + rxx * cosG) / sqrtRxx2Rxz2;
        }

        return DenseMatrix.OfArray(new[,] {
            { rxx, rxy, rxz },
            { r21, r22, r23 },
            { r31, r32, r33 },
        });
    }

    public Matrix<double> GetTransformationMatrix()
    {
        var rotationMatrix = this.GetRotationMatrix();
        var transformationMatrix = Matrix<double>.Build.Dense(12, 12);
        transformationMatrix.SetSubMatrix(0, 0, rotationMatrix);
        transformationMatrix.SetSubMatrix(3, 3, rotationMatrix);
        transformationMatrix.SetSubMatrix(6, 6, rotationMatrix);
        transformationMatrix.SetSubMatrix(9, 9, rotationMatrix);
        return transformationMatrix;
    }

    public Matrix<double> GetLocalStiffnessMatrix()
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
        var ExA_L = (E * A / L).As(this.UnitSettings.ForcePerLengthUnit);
        var ExIs_L3 = E.MultiplyBy(Is.DivideBy(L3)).As(this.UnitSettings.ForcePerLengthUnit);
        var ExIw_L3 = E.MultiplyBy(Iw.DivideBy(L3)).As(this.UnitSettings.ForcePerLengthUnit);

        // Force (N, k)
        var ExIs_L2 = E.MultiplyBy(Is.DivideBy(L2)).As(this.UnitSettings.ForceUnit);
        var ExIw_L2 = E.MultiplyBy(Iw.DivideBy(L2)).As(this.UnitSettings.ForceUnit);

        // Torque (Nm, k-in)
        var ExIs_L = E.MultiplyBy(Is / L).As(this.UnitSettings.TorqueUnit);
        var ExIw_L = E.MultiplyBy(Iw / L).As(this.UnitSettings.TorqueUnit);
        var GxJ_L = G.MultiplyBy(J / L).As(this.UnitSettings.TorqueUnit);
#pragma warning restore IDE1006 // Naming Styles

        return DenseMatrix.OfArray(new[,]
        {
              {  ExA_L,           0,           0,      0,          0,          0, -ExA_L,           0,           0,      0,          0,          0 },
              {      0,  12*ExIs_L3,           0,      0,          0,  6*ExIs_L2,      0, -12*ExIs_L3,           0,      0,          0,  6*ExIs_L2 },
              {      0,           0,  12*ExIw_L3,      0, -6*ExIw_L2,          0,      0,           0, -12*ExIw_L3,      0, -6*ExIw_L2,          0 },
              {      0,           0,           0,  GxJ_L,          0,          0,      0,           0,           0, -GxJ_L,          0,          0 },
              {      0,           0,  -6*ExIw_L2,      0,   4*ExIw_L,          0,      0,           0,   6*ExIw_L2,      0,   2*ExIw_L,          0 },
              {      0,   6*ExIs_L2,           0,      0,          0,   4*ExIs_L,      0,  -6*ExIs_L2,           0,      0,          0,   2*ExIs_L },
              { -ExA_L,           0,           0,      0,          0,          0,  ExA_L,           0,           0,      0,          0,          0 },
              {      0, -12*ExIs_L3,           0,      0,          0, -6*ExIs_L2,      0,  12*ExIs_L3,           0,      0,          0, -6*ExIs_L2 },
              {      0,           0, -12*ExIw_L3,      0,  6*ExIw_L2,          0,      0,           0,  12*ExIw_L3,      0,  6*ExIw_L2,          0 },
              {      0,           0,           0, -GxJ_L,          0,          0,      0,           0,           0,  GxJ_L,          0,          0 },
              {      0,           0,  -6*ExIw_L2,      0,   2*ExIw_L,          0,      0,           0,   6*ExIw_L2,      0,   4*ExIw_L,          0 },
              {      0,   6*ExIs_L2,           0,      0,          0,   2*ExIs_L,      0,  -6*ExIs_L2,           0,      0,          0,   4*ExIs_L },
        });
    }

    public Matrix<double> GetGlobalStiffnessMatrix()
    {
        var transformationMatrix = this.GetTransformationMatrix();
        var localStiffnessMatrix = this.GetLocalStiffnessMatrix();
        return transformationMatrix.Transpose() * localStiffnessMatrix * transformationMatrix;
    }

    public MatrixIdentified<UnsupportedStructureDisplacementId> GetGlobalStiffnessMatrixIdentified()
    {
        return new(
            this.GetUnsupportedStructureDisplacementIds().ToList(),
            this.GetGlobalStiffnessMatrix().ToArray()
            );
    }

    private VectorIdentified ToVectorIdentified(Vector<double> vector)
    {
        return new(
            this.GetUnsupportedStructureDisplacementIds().ToList(),
            vector.ToArray()
            );
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
        return this.GetTransformationMatrix() * this.GetGlobalEndDisplacementVector(jointDisplacementVector);
    }

    public Vector<double> GetLocalMemberEndForcesVector(VectorIdentified jointDisplacementVector)
    {
        return this.GetLocalStiffnessMatrix() * this.GetLocalEndDisplacementVector(jointDisplacementVector);
    }

    public Vector<double> GetGlobalMemberEndForcesVector(VectorIdentified jointDisplacementVector)
    {
        return this.GetTransformationMatrix().Transpose() * this.GetLocalMemberEndForcesVector(jointDisplacementVector);
    }
    public VectorIdentified GetGlobalMemberEndForcesVectorIdentified(VectorIdentified jointDisplacementVector)
    {
        return this.ToVectorIdentified(this.GetGlobalMemberEndForcesVector(jointDisplacementVector));
    }
}
