using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Extensions;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.DsmNodeAggregate;
using BeamOs.Domain.DirectStiffnessMethod.DsmNodeAggregate.ValueObjects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate;

public class AnalyticalElement1D : AggregateRoot<AnalyticalElement1DId>
{
    public AnalyticalElement1D(
        Angle sectionProfileRotation,
        Pressure modulusOfElasticity,
        Pressure modulusOfRigidity,
        Area area,
        AreaMomentOfInertia strongAxisMomentOfInertia,
        AreaMomentOfInertia weakAxisMomentOfInertia,
        AreaMomentOfInertia polarMomentOfInertia,
        Line baseLine,
        DsmNodeId startNodeId,
        DsmNodeId endNodeId,
        AnalyticalElement1DId? id = null
    )
        : base(id ?? new())
    {
        this.SectionProfileRotation = sectionProfileRotation;
        this.ModulusOfElasticity = modulusOfElasticity;
        this.ModulusOfRigidity = modulusOfRigidity;
        this.Area = area;
        this.StrongAxisMomentOfInertia = strongAxisMomentOfInertia;
        this.WeakAxisMomentOfInertia = weakAxisMomentOfInertia;
        this.PolarMomentOfInertia = polarMomentOfInertia;
        this.BaseLine = baseLine;
        this.StartNodeId = startNodeId;
        this.EndNodeId = endNodeId;
    }

    public AnalyticalElement1D(
        Angle sectionProfileRotation,
        DsmNode startNode,
        DsmNode endNode,
        Material material,
        SectionProfile sectionProfile,
        AnalyticalElement1DId? id = null
    )
        : this(
            sectionProfileRotation,
            material.ModulusOfElasticity,
            material.ModulusOfRigidity,
            sectionProfile.Area,
            sectionProfile.StrongAxisMomentOfInertia,
            sectionProfile.WeakAxisMomentOfInertia,
            sectionProfile.PolarMomentOfInertia,
            new Line(startNode.LocationPoint, endNode.LocationPoint),
            startNode.Id,
            endNode.Id,
            id
        ) { }

    public Angle SectionProfileRotation { get; set; }

    //public List<Load> Load { get; private set; }

    public Pressure ModulusOfElasticity { get; set; }
    public Pressure ModulusOfRigidity { get; set; }

    public Area Area { get; set; }
    public AreaMomentOfInertia StrongAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia WeakAxisMomentOfInertia { get; set; }
    public AreaMomentOfInertia PolarMomentOfInertia { get; set; }

    public Line BaseLine { get; }
    public Length Length => this.BaseLine.Length;

    public DsmNodeId StartNodeId { get; set; }
    public DsmNodeId EndNodeId { get; set; }

    public IEnumerable<UnsupportedStructureDisplacementId> GetUnsupportedStructureDisplacementIds()
    {
        for (var i = 0; i < 2; i++)
        {
            var nodeId = i == 0 ? this.StartNodeId : this.EndNodeId;
            foreach (
                CoordinateSystemDirection3D direction in Enum.GetValues(
                    typeof(CoordinateSystemDirection3D)
                )
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

    public Matrix<double> GetRotationMatrix()
    {
        var rxx =
            (this.BaseLine.EndPoint.XCoordinate - this.BaseLine.StartPoint.XCoordinate)
            / this.Length;
        var rxy =
            (this.BaseLine.EndPoint.YCoordinate - this.BaseLine.StartPoint.YCoordinate)
            / this.Length;
        var rxz =
            (this.BaseLine.EndPoint.ZCoordinate - this.BaseLine.StartPoint.ZCoordinate)
            / this.Length;

        var cosG = Math.Cos(this.SectionProfileRotation.Radians);
        var sinG = Math.Sin(this.SectionProfileRotation.Radians);

        var sqrtRxx2Rxz2 = Math.Sqrt(rxx * rxx + rxz * rxz);

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
            r21 = (-rxx * rxy * cosG - rxz * sinG) / sqrtRxx2Rxz2;
            r22 = sqrtRxx2Rxz2 * cosG;
            r23 = (-rxy * rxz * cosG + rxx * sinG) / sqrtRxx2Rxz2;
            r31 = (rxx * rxy * sinG - rxz * cosG) / sqrtRxx2Rxz2;
            r32 = -sqrtRxx2Rxz2 * sinG;
            r33 = (rxy * rxz * sinG + rxx * cosG) / sqrtRxx2Rxz2;
        }

        return DenseMatrix.OfArray(
            new[,]
            {
                { rxx, rxy, rxz },
                { r21, r22, r23 },
                { r31, r32, r33 },
            }
        );
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

    public Matrix<double> GetLocalStiffnessMatrix(
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
        VectorIdentified globalEndDisplacementVector =
            new(this.GetUnsupportedStructureDisplacementIds().ToList());
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
