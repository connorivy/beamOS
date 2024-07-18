using System.Collections.Generic;
using BeamOs.Domain.AnalyticalResults.Common.ValueObjects;
using BeamOs.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.Diagrams.ShearForceDiagramAggregate;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.PhysicalModel.Element1DAggregate;
using BeamOs.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Domain.DirectStiffnessMethod;

public class DsmAnalysisModel(Model model)
{
    private readonly DsmElement1d[] dsmElement1Ds = model
        .Element1ds
        .Select(el => new DsmElement1d(el))
        .ToArray();

    private readonly DsmNodeVo[] dsmNodes = model.Nodes.Select(el => new DsmNodeVo(el)).ToArray();

    public ModelResults RunAnalysis()
    {
        var (degreeOfFreedomIds, boundaryConditionIds) = this.GetSortedUnsupportedStructureIds();

        MatrixIdentified structureStiffnessMatrix = this.BuildStructureStiffnessMatrix(
            degreeOfFreedomIds
        );

        VectorIdentified knownJointDisplacementVector = BuildKnownJointDisplacementVector(
            boundaryConditionIds
        );

        VectorIdentified knownReactionVector = this.BuildKnownJointReactionVector(
            degreeOfFreedomIds
        );

        VectorIdentified unknownJointDisplacementVector = GetUnknownJointDisplacementVector(
            structureStiffnessMatrix,
            knownReactionVector,
            degreeOfFreedomIds
        );

        VectorIdentified unknownReactionVector = this.GetUnknownJointReactionVector(
            boundaryConditionIds,
            unknownJointDisplacementVector
        );

        List<NodeResult> nodeResults = this.GetAnalyticalNodes(
            unknownJointDisplacementVector,
            knownJointDisplacementVector,
            unknownReactionVector,
            knownReactionVector
        );

        List<ShearForceDiagram> shearForceDiagrams = [];

        foreach (var dsmElement1d in this.dsmElement1Ds)
        {
            var localMemberEndForcesVector = dsmElement1d.GetLocalMemberEndForcesVector(
                unknownJointDisplacementVector,
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.ForcePerLengthUnit,
                model.Settings.UnitSettings.TorqueUnit
            );
            shearForceDiagrams.Add(
                ShearForceDiagram.Create(
                    dsmElement1d.Element1DId,
                    dsmElement1d.StartPoint,
                    dsmElement1d.EndPoint,
                    dsmElement1d.SectionProfileRotation,
                    dsmElement1d.Length,
                    localMemberEndForcesVector,
                    model.Settings.UnitSettings.LengthUnit,
                    model.Settings.UnitSettings.ForceUnit,
                    model.Settings.UnitSettings.TorqueUnit,
                    LinearCoordinateDirection3D.AlongY
                )
            );
        }

        return new ModelResults
        {
            NodeResults = nodeResults,
            ShearForceDiagrams = shearForceDiagrams
        };
    }

    internal SortedUnsupportedStructureIds GetSortedUnsupportedStructureIds()
    {
        List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds = [];
        List<UnsupportedStructureDisplacementId2> boundaryConditionIds = [];
        foreach (var node in model.Nodes)
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
                    degreeOfFreedomIds.Add(new(node.Id, direction));
                }
                else
                {
                    boundaryConditionIds.Add(new(node.Id, direction));
                }
            }
        }
        return new SortedUnsupportedStructureIds(
            degreeOfFreedomIds: degreeOfFreedomIds,
            boundaryConditionIds: boundaryConditionIds
        );
    }

    internal MatrixIdentified BuildStructureStiffnessMatrix(
        List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds
    )
    {
        MatrixIdentified sMatrix = new(degreeOfFreedomIds);
        foreach (var element1D in this.dsmElement1Ds)
        {
            var globalMatrixWithIdentifiers = element1D.GetGlobalStiffnessMatrixIdentified(
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.ForcePerLengthUnit,
                model.Settings.UnitSettings.TorqueUnit
            );
            sMatrix.AddEntriesWithMatchingIdentifiers(globalMatrixWithIdentifiers);
        }

        return sMatrix;
    }

    private static VectorIdentified BuildKnownJointDisplacementVector(
        List<UnsupportedStructureDisplacementId2> boundaryConditionIds
    )
    {
        // TODO : support non-zero known displacements
        double[] hardcodedNodeDisplacements = Enumerable
            .Repeat(0.0, boundaryConditionIds.Count)
            .ToArray();

        return new VectorIdentified(boundaryConditionIds, hardcodedNodeDisplacements);
    }

    internal VectorIdentified BuildKnownJointReactionVector(
        List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds
    )
    {
        VectorIdentified loadVector = new(degreeOfFreedomIds);
        foreach (var node in this.dsmNodes)
        {
            var localLoadVector = node.GetForceVectorIdentifiedInGlobalCoordinates(
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.TorqueUnit
            );
            loadVector.AddEntriesWithMatchingIdentifiers(localLoadVector);
        }

        return loadVector;
    }

    internal VectorIdentified GetUnknownJointDisplacementVector(
        MatrixIdentified structureStiffnessMatrix,
        VectorIdentified knownReactionVector,
        List<UnsupportedStructureDisplacementId2> degreeOfFreedomIds
    )
    {
        var dofDisplacementMathnetVector =
            structureStiffnessMatrix.Build().Inverse() * knownReactionVector.Build();
        VectorIdentified dofDisplacementVector =
            new(degreeOfFreedomIds, dofDisplacementMathnetVector.ToArray());

        return dofDisplacementVector;
    }

    internal VectorIdentified GetUnknownJointReactionVector(
        List<UnsupportedStructureDisplacementId2> boundaryConditionIds,
        VectorIdentified unknownJointDisplacementVector
    )
    {
        VectorIdentified reactions = new(boundaryConditionIds);
        foreach (var element1D in this.dsmElement1Ds)
        {
            var globalMemberEndForcesVector = element1D.GetGlobalMemberEndForcesVectorIdentified(
                unknownJointDisplacementVector,
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.ForcePerLengthUnit,
                model.Settings.UnitSettings.TorqueUnit
            );
            reactions.AddEntriesWithMatchingIdentifiers(globalMemberEndForcesVector);
        }

        return reactions;
    }

    private List<NodeResult> GetAnalyticalNodes(
        VectorIdentified unknownJointDisplacementVector,
        VectorIdentified knownJointDisplacementVector,
        VectorIdentified unknownJointReactionVector,
        VectorIdentified knownJointReactionVector
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

            this.CreateStronglyTypedDisplacementsFromResultVectors(
                displacementVectorDict[nodeId],
                displacementLengths,
                displacementAngles
            );
            this.CreateStronglyTypedReactionsFromResultVectors(
                reactionVectorDict[nodeId],
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
            analyticalNodes.Add(
                new NodeResult(model.Id, nodeId, forcesResponse, displacementResponse)
            );
        }

        return analyticalNodes;
    }

    private void CreateStronglyTypedReactionsFromResultVectors(
        IEnumerable<KeyValuePair<UnsupportedStructureDisplacementId2, double>> reactionVectorValues,
        Dictionary<CoordinateSystemDirection3D, Force> forceForces,
        Dictionary<CoordinateSystemDirection3D, Torque> forceTorques
    )
    {
        foreach (var kvp in reactionVectorValues)
        {
            if (kvp.Key.Direction.IsLinearDirection())
            {
                forceForces.Add(
                    kvp.Key.Direction,
                    new Force(kvp.Value, model.Settings.UnitSettings.ForceUnit)
                );
            }
            else
            {
                forceTorques.Add(
                    kvp.Key.Direction,
                    new Torque(kvp.Value, model.Settings.UnitSettings.TorqueUnit)
                );
            }
        }
    }

    private void CreateStronglyTypedDisplacementsFromResultVectors(
        IEnumerable<
            KeyValuePair<UnsupportedStructureDisplacementId2, double>
        > displacementVectorValues,
        Dictionary<CoordinateSystemDirection3D, Length> displacementLengths,
        Dictionary<CoordinateSystemDirection3D, Angle> displacementAngles
    )
    {
        foreach (var kvp in displacementVectorValues)
        {
            if (kvp.Key.Direction.IsLinearDirection())
            {
                displacementLengths.Add(
                    kvp.Key.Direction,
                    new Length(kvp.Value, model.Settings.UnitSettings.LengthUnit)
                );
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
