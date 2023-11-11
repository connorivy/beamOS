using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.DirectStiffnessMethod.Domain.Services;
public class SolverService
{
    public static void Solve(
        UnitSettings unitSettings,
        List<AnalyticalElement1D> element1Ds,
        List<AnalyticalNode> nodes)
    {
        UnsupportedStructureDisplacementRepo displacementOrDOFRepo = new(nodes);

        var jointDisplacementVector = GetJointDisplacementVector(unitSettings, element1Ds, nodes, displacementOrDOFRepo);

        var reactionVector = GetReactionVector(element1Ds, displacementOrDOFRepo, jointDisplacementVector);
    }

    private static VectorIdentified GetReactionVector(
        List<AnalyticalElement1D> element1Ds,
        UnsupportedStructureDisplacementRepo displacementOrDOFRepo,
        VectorIdentified jointDisplacementVector)
    {
        List<UnsupportedStructureDisplacementId> bcIds = displacementOrDOFRepo.BoundaryConditionIds;
        VectorIdentified reactions = new(bcIds);
        foreach (AnalyticalElement1D element1D in element1Ds)
        {
            VectorIdentified globalMemberEndForcesVector = element1D.GetGlobalMemberEndForcesVectorIdentified(jointDisplacementVector);
            reactions.AddEntriesWithMatchingIdentifiers(globalMemberEndForcesVector);
        }

        return reactions;
    }

    private static VectorIdentified GetJointDisplacementVector(
        UnitSettings unitSettings,
        List<AnalyticalElement1D> element1Ds,
        IEnumerable<AnalyticalNode> nodes,
        UnsupportedStructureDisplacementRepo displacementOrDOFRepo)
    {
        List<UnsupportedStructureDisplacementId> dofIds = displacementOrDOFRepo.DegreeOfFreedomIds;
        MatrixIdentified<UnsupportedStructureDisplacementId> sMatrix = BuildStructureStiffnessMatrix(element1Ds, dofIds);
        VectorIdentified loadVector = BuildLoadVector(unitSettings, nodes, dofIds);

        Vector<double> dofDisplacementMathnetVector = sMatrix.Build().Inverse() * loadVector.Build();
        VectorIdentified dofDisplacementVector = new(
            dofIds,
            dofDisplacementMathnetVector.ToArray()
        );
        return dofDisplacementVector;
    }

    private static VectorIdentified BuildLoadVector(
        UnitSettings unitSettings,
        IEnumerable<AnalyticalNode> nodes,
        List<UnsupportedStructureDisplacementId> dofIds)
    {
        VectorIdentified loadVector = new(dofIds);
        foreach (AnalyticalNode node in nodes)
        {
            VectorIdentified localLoadVector = node
                .GetForceVectorIdentifiedInGlobalCoordinates(unitSettings.ForceUnit, unitSettings.TorqueUnit);
            loadVector.AddEntriesWithMatchingIdentifiers(localLoadVector);
        }

        return loadVector;
    }

    private static MatrixIdentified<UnsupportedStructureDisplacementId> BuildStructureStiffnessMatrix(
        List<AnalyticalElement1D> element1Ds,
        List<UnsupportedStructureDisplacementId> dofIds)
    {
        MatrixIdentified<UnsupportedStructureDisplacementId> sMatrix = new(dofIds);
        foreach (AnalyticalElement1D element1D in element1Ds)
        {
            MatrixIdentified<UnsupportedStructureDisplacementId> globalMatrixWithIdentifiers = element1D
                .GetGlobalStiffnessMatrixIdentified();
            sMatrix.AddEntriesWithMatchingIdentifiers(globalMatrixWithIdentifiers);
        }

        return sMatrix;
    }
}
