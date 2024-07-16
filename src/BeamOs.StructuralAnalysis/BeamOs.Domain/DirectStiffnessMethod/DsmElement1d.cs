using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Extensions;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet.Units;

namespace BeamOs.Domain.DirectStiffnessMethod;

public class DsmElement1d(Element1D element1d) : BeamOSValueObject
{
    public IEnumerable<UnsupportedStructureDisplacementId2> GetUnsupportedStructureDisplacementIds()
    {
        for (var i = 0; i < 2; i++)
        {
            var nodeId = i == 0 ? element1d.StartNodeId : element1d.EndNodeId;
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

    public Matrix<double> GetTransformationMatrix()
    {
        var rotationMatrix = DenseMatrix.OfArray(element1d.GetRotationMatrix());
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
        var E = element1d.Material.ModulusOfElasticity;
        var G = element1d.Material.ModulusOfRigidity;
        var A = element1d.SectionProfile.Area;
        var L = element1d.Length;
        var Is = element1d.SectionProfile.StrongAxisMomentOfInertia;
        var Iw = element1d.SectionProfile.WeakAxisMomentOfInertia;
        var J = element1d.SectionProfile.PolarMomentOfInertia;
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

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return element1d;
    }
}
