using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Services.ValueObjects;
using BeamOs.Domain.PhysicalModel.NodeAggregate.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.DirectStiffnessMethod.Services;

public sealed class DirectStiffnessMethodSolver
{
    public static ModelResults RunAnalysis(
        UnitSettings unitSettings,
        IEnumerable<DsmElement1dVo> element1Ds,
        IEnumerable<DsmNodeVo> nodes
    )
    {
        var (degreeOfFreedomIds, boundaryConditionIds) = GetSortedUnsupportedStructureIds(nodes);

        MatrixIdentified2 structureStiffnessMatrix = BuildStructureStiffnessMatrix(
            degreeOfFreedomIds,
            element1Ds,
            unitSettings.ForceUnit,
            unitSettings.ForcePerLengthUnit,
            unitSettings.TorqueUnit
        );

        VectorIdentified2 knownJointDisplacementVector = BuildKnownJointDisplacementVector(
            boundaryConditionIds
        );

        VectorIdentified2 knownReactionVector = BuildKnownJointReactionVector(
            degreeOfFreedomIds,
            nodes,
            unitSettings.ForceUnit,
            unitSettings.TorqueUnit
        );
        VectorIdentified2 unknownJointDisplacementVector = GetUnknownJointDisplacementVector(
            structureStiffnessMatrix,
            knownReactionVector,
            degreeOfFreedomIds
        );

        VectorIdentified2 unknownReactionVector = GetUnknownJointReactionVector(
            boundaryConditionIds,
            unknownJointDisplacementVector,
            element1Ds,
            unitSettings.ForceUnit,
            unitSettings.ForcePerLengthUnit,
            unitSettings.TorqueUnit
        );

        List<NodeResult> nodeResults = GetAnalyticalNodes(
            unknownJointDisplacementVector,
            knownJointDisplacementVector,
            unknownReactionVector,
            knownReactionVector,
            unitSettings.LengthUnit,
            unitSettings.ForceUnit,
            unitSettings.TorqueUnit
        );

        return new(nodeResults, []);
    }

    private static SortedUnsupportedStructureIds GetSortedUnsupportedStructureIds(
        IEnumerable<DsmNodeVo> nodes
    )
    {
        List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds = [];
        List<UnsupportedStructureDisplacementId2> boundaryConditionIds = [];
        foreach (var node in nodes)
        {
            foreach (
                CoordinateSystemDirection3D direction in Enum.GetValues<CoordinateSystemDirection3D>()
            )
            {
                if (direction == CoordinateSystemDirection3D.Undefined)
                {
                    continue;
                }

                // if UnsupportedStructureDisplacement is degree of freedom
                if (node.Restraint.GetValueInDirection(direction) == true)
                {
                    degreeOfFreedomIds.Add(new(node.NodeId, direction));
                }
                else
                {
                    boundaryConditionIds.Add(new(node.NodeId, direction));
                }
            }
        }
        return new SortedUnsupportedStructureIds(
            degreeOfFreedomIds: degreeOfFreedomIds,
            boundaryConditionIds: boundaryConditionIds
        );
    }

    private static MatrixIdentified2 BuildStructureStiffnessMatrix(
        List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds,
        IEnumerable<DsmElement1dVo> element1ds,
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
        MatrixIdentified2 sMatrix = new(degreeOfFreedomIds);
        foreach (var element1D in element1ds)
        {
            var globalMatrixWithIdentifiers = element1D.GetGlobalStiffnessMatrixIdentified(
                forceUnit,
                forcePerLengthUnit,
                torqueUnit
            );
            sMatrix.AddEntriesWithMatchingIdentifiers(globalMatrixWithIdentifiers);
        }

        return sMatrix;
    }

    private static VectorIdentified2 BuildKnownJointDisplacementVector(
        List<UnsupportedStructureDisplacementId2> boundaryConditionIds
    )
    {
        // TODO : support non-zero known displacements
        double[] hardcodedNodeDisplacements = Enumerable
            .Repeat(0.0, boundaryConditionIds.Count)
            .ToArray();

        return new VectorIdentified2(boundaryConditionIds, hardcodedNodeDisplacements);
    }

    private static VectorIdentified2 BuildKnownJointReactionVector(
        List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds,
        IEnumerable<DsmNodeVo> nodes,
        ForceUnit forceUnit,
        TorqueUnit torqueUnit
    )
    {
        VectorIdentified2 loadVector = new(degreeOfFreedomIds);
        foreach (var node in nodes)
        {
            var localLoadVector = node.GetForceVectorIdentifiedInGlobalCoordinates(
                forceUnit,
                torqueUnit
            );
            loadVector.AddEntriesWithMatchingIdentifiers(localLoadVector);
        }

        return loadVector;
    }

    private static VectorIdentified2 GetUnknownJointDisplacementVector(
        MatrixIdentified2 structureStiffnessMatrix,
        VectorIdentified2 knownReactionVector,
        List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds
    )
    {
        var dofDisplacementMathnetVector =
            structureStiffnessMatrix.Build().Inverse() * knownReactionVector.Build();
        VectorIdentified2 dofDisplacementVector =
            new(degreeOfFreedomIds, dofDisplacementMathnetVector.ToArray());

        return dofDisplacementVector;
    }

    private static VectorIdentified2 GetUnknownJointReactionVector(
        List<UnsupportedStructureDisplacementId2> boundaryConditionIds,
        VectorIdentified2 unknownJointDisplacementVector,
        IEnumerable<DsmElement1dVo> element1Ds,
        ForceUnit forceUnit,
        ForcePerLengthUnit forcePerLengthUnit,
        TorqueUnit torqueUnit
    )
    {
        VectorIdentified2 reactions = new(boundaryConditionIds);
        foreach (var element1D in element1Ds)
        {
            var globalMemberEndForcesVector = element1D.GetGlobalMemberEndForcesVectorIdentified(
                unknownJointDisplacementVector,
                forceUnit,
                forcePerLengthUnit,
                torqueUnit
            );
            reactions.AddEntriesWithMatchingIdentifiers(globalMemberEndForcesVector);
        }

        return reactions;
    }

    private static List<NodeResult> GetAnalyticalNodes(
        VectorIdentified2 unknownJointDisplacementVector,
        VectorIdentified2 knownJointDisplacementVector,
        VectorIdentified2 unknownJointReactionVector,
        VectorIdentified2 knownJointReactionVector,
        LengthUnit lengthUnit,
        ForceUnit forceUnit,
        TorqueUnit torqueUnit
    )
    {
        List<NodeResult> analyticalNodes = [];

        var displacementVectorDict = unknownJointDisplacementVector
            .Concat(knownJointDisplacementVector)
            .GroupBy(kvp => kvp.Key.NodeId)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());
        var reactionVectorDict = unknownJointReactionVector
            .Concat(knownJointReactionVector)
            .GroupBy(kvp => kvp.Key.NodeId)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        // this is another way to do the above join and group. Is it better?
        //var x = (from urxns in unknownReactionVector
        //         join krxns in knownReactionVector on urxns.Key.NodeId equals krxns.Key.NodeId into rxns
        //         select rxns)
        //        .ToDictionary(enumerable => enumerable.First().Key.NodeId);

        foreach (var nodeId in displacementVectorDict.Keys)
        {
            Dictionary<CoordinateSystemDirection3D, Length> displacementLengths = [];
            Dictionary<CoordinateSystemDirection3D, Angle> displacementAngles = [];
            Dictionary<CoordinateSystemDirection3D, Force> forceForces = [];
            Dictionary<CoordinateSystemDirection3D, Torque> forceTorques = [];
            CreateStronglyTypedDisplacementsFromResultVectors(
                lengthUnit,
                displacementVectorDict,
                nodeId,
                displacementLengths,
                displacementAngles
            );
            CreateStronglyTypedReactionsFromResultVectors(
                forceUnit,
                torqueUnit,
                reactionVectorDict,
                nodeId,
                forceForces,
                forceTorques
            );
            var forcesResponse = new Forces(
                forceForces[CoordinateSystemDirection3D.AlongX],
                forceForces[CoordinateSystemDirection3D.AlongY],
                forceForces[CoordinateSystemDirection3D.AlongZ],
                forceTorques[CoordinateSystemDirection3D.AboutX],
                forceTorques[CoordinateSystemDirection3D.AboutY],
                forceTorques[CoordinateSystemDirection3D.AboutZ]
            );

            var displacementResponse = new Displacements(
                displacementLengths[CoordinateSystemDirection3D.AlongX],
                displacementLengths[CoordinateSystemDirection3D.AlongY],
                displacementLengths[CoordinateSystemDirection3D.AlongZ],
                displacementAngles[CoordinateSystemDirection3D.AboutX],
                displacementAngles[CoordinateSystemDirection3D.AboutY],
                displacementAngles[CoordinateSystemDirection3D.AboutZ]
            );
            analyticalNodes.Add(new NodeResult(nodeId, forcesResponse, displacementResponse));
        }

        return analyticalNodes;
    }

    private static void CreateStronglyTypedReactionsFromResultVectors(
        ForceUnit forceUnit,
        TorqueUnit torqueUnit,
        Dictionary<
            NodeId,
            IEnumerable<KeyValuePair<UnsupportedStructureDisplacementId2, double>>
        > reactionVectorDict,
        NodeId nodeId,
        Dictionary<CoordinateSystemDirection3D, Force> forceForces,
        Dictionary<CoordinateSystemDirection3D, Torque> forceTorques
    )
    {
        foreach (var kvp in reactionVectorDict[nodeId])
        {
            if (kvp.Key.Direction.IsLinearDirection())
            {
                forceForces.Add(kvp.Key.Direction, new Force(kvp.Value, forceUnit));
            }
            else
            {
                forceTorques.Add(kvp.Key.Direction, new Torque(kvp.Value, torqueUnit));
            }
        }
    }

    private static void CreateStronglyTypedDisplacementsFromResultVectors(
        LengthUnit lengthUnit,
        Dictionary<
            NodeId,
            IEnumerable<KeyValuePair<UnsupportedStructureDisplacementId2, double>>
        > displacementVectorDict,
        NodeId nodeId,
        Dictionary<CoordinateSystemDirection3D, Length> displacementLengths,
        Dictionary<CoordinateSystemDirection3D, Angle> displacementAngles
    )
    {
        foreach (var kvp in displacementVectorDict[nodeId])
        {
            if (kvp.Key.Direction.IsLinearDirection())
            {
                displacementLengths.Add(kvp.Key.Direction, new Length(kvp.Value, lengthUnit));
            }
            else
            {
                displacementAngles.Add(
                    kvp.Key.Direction,
                    new Angle(kvp.Value, AngleUnit.Radian) // TODO : support degree conversion
                );
            }
        }
    }
}
