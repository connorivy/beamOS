using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.EnvelopeResultSets;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.LoadCombinations;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using CSparse.Factorization;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;

public sealed class DsmAnalysisModel(
    ModelId modelId,
    UnitSettings unitSettings,
    DsmElement1d[] dsmElement1Ds,
    DsmNodeVo[] dsmNodes
)
{
    public DsmAnalysisModel(Model model, UnitSettings? unitSettings = null)
        : this(
            model.Id,
            unitSettings ?? model.Settings.UnitSettings,
            CreateDsmElement1dsFromModel(model),
            CreateDsmNodesFromModel(model)
        ) { }

    public DsmAnalysisModel Clone()
    {
        return new(modelId, unitSettings, dsmElement1Ds, dsmNodes);
    }

    private static DsmElement1d[] CreateDsmElement1dsFromModel(Model model) =>
        model.Settings.AnalysisSettings.Element1DAnalysisType switch
        {
            Element1dAnalysisType.Euler => model
                .Element1ds.Select(el => new DsmElement1d(el))
                .ToArray(),
            Element1dAnalysisType.Timoshenko => model
                .Element1ds.Select(el => new TimoshenkoDsmElement1d(el))
                .ToArray(),
            Element1dAnalysisType.Undefined or _ => throw new Exception(
                $"Unsupported Element1DAnalysisType {model.Settings.AnalysisSettings.Element1DAnalysisType}"
            ),
        };

    private static DsmNodeVo[] CreateDsmNodesFromModel(Model model) =>
        model.Nodes.Select(el => new DsmNodeVo(el)).ToArray();

    public AnalysisResults RunAnalysis(
        ISolverFactory solverFactory,
        LoadCombination loadCombination
    )
    {
        VectorIdentified knownJointDisplacementVector = this.BuildKnownJointDisplacementVector();

        VectorIdentified knownReactionVector = this.BuildKnownJointReactionVector(loadCombination);

        VectorIdentified unknownJointDisplacementVector = this.GetUnknownJointDisplacementVector(
            solverFactory,
            loadCombination
        );

        VectorIdentified unknownReactionVector = this.GetUnknownJointReactionVector(
            unknownJointDisplacementVector
        );

        ResultSet resultSet = new(modelId, loadCombination.Id);
        List<NodeResult> nodeResults = this.GetAnalyticalNodes(
            resultSet.Id,
            unknownJointDisplacementVector,
            knownJointDisplacementVector,
            unknownReactionVector,
            knownReactionVector
        );
        resultSet.NodeResults = nodeResults;
        EnvelopeResultSet envelopeResultSet = new(modelId);

        var otherResults = resultSet.ComputeDiagramsAndElement1dResults(
            dsmElement1Ds,
            unitSettings,
            envelopeResultSet
        );

        return new() { ResultSet = resultSet, OtherAnalyticalResults = otherResults };
    }

    private SortedUnsupportedStructureIds? sortedUnsupportedStructureIds;

    internal SortedUnsupportedStructureIds GetSortedUnsupportedStructureIds()
    {
        if (this.sortedUnsupportedStructureIds is not null)
        {
            return this.sortedUnsupportedStructureIds;
        }

        List<UnsupportedStructureDisplacementId> degreeOfFreedomIds = [];
        List<UnsupportedStructureDisplacementId> boundaryConditionIds = [];
        foreach (var dsmNode in dsmNodes)
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
                if (dsmNode.Node.Restraint.GetValueInDirection(direction) == true)
                {
                    degreeOfFreedomIds.Add(new(dsmNode.Node.Id, direction));
                }
                else
                {
                    boundaryConditionIds.Add(new(dsmNode.Node.Id, direction));
                }
            }
        }

        return this.sortedUnsupportedStructureIds = new SortedUnsupportedStructureIds(
            degreeOfFreedomIds: degreeOfFreedomIds,
            boundaryConditionIds: boundaryConditionIds
        );
    }

    private MatrixIdentified? structuralStiffnessMatrix;

    internal MatrixIdentified BuildStructureStiffnessMatrix()
    {
        if (this.structuralStiffnessMatrix is not null)
        {
            return this.structuralStiffnessMatrix;
        }

        var (degreeOfFreedomIds, _) = this.GetSortedUnsupportedStructureIds();

        MatrixIdentified sMatrix = new(degreeOfFreedomIds);
        foreach (var element1D in dsmElement1Ds)
        {
            var globalMatrixWithIdentifiers = element1D.GetGlobalStiffnessMatrixIdentified(
                unitSettings.ForceUnit,
                unitSettings.ForcePerLengthUnit,
                unitSettings.TorqueUnit
            );
            sMatrix.AddEntriesWithMatchingIdentifiers(globalMatrixWithIdentifiers);
        }

        return this.structuralStiffnessMatrix = sMatrix;
    }

    private VectorIdentified? knownJointDisplacementVector;

    private VectorIdentified BuildKnownJointDisplacementVector()
    {
        if (this.knownJointDisplacementVector is null)
        {
            var (_, boundaryConditionIds) = this.GetSortedUnsupportedStructureIds();

            // TODO : support non-zero known displacements
            double[] hardcodedNodeDisplacements = Enumerable
                .Repeat(0.0, boundaryConditionIds.Count)
                .ToArray();

            this.knownJointDisplacementVector = new VectorIdentified(
                boundaryConditionIds,
                hardcodedNodeDisplacements
            );
        }

        return this.knownJointDisplacementVector.Value;
    }

    internal VectorIdentified BuildKnownJointReactionVector(LoadCombination loadCombination)
    {
        var (degreeOfFreedomIds, _) = this.GetSortedUnsupportedStructureIds();
        VectorIdentified loadVector = new(degreeOfFreedomIds);
        foreach (var node in dsmNodes)
        {
            var localLoadVector = node.GetForceVectorIdentifiedInGlobalCoordinates(
                unitSettings.ForceUnit,
                unitSettings.TorqueUnit,
                loadCombination
            );
            loadVector.AddEntriesWithMatchingIdentifiers(localLoadVector);
        }

        return loadVector;
    }

    private ISolver<double>? solver;

    internal VectorIdentified GetUnknownJointDisplacementVector(
        ISolverFactory solverFactory,
        LoadCombination loadCombination
    )
    {
        var (degreeOfFreedomIds, _) = this.GetSortedUnsupportedStructureIds();

        // todo : we could save a lot of time by caching the factorization of the stiffness matrix
        // if (this.solver is null)
        {
            var structureStiffnessMatrix = this.BuildStructureStiffnessMatrix();

            var stiffnessMatrix = CSparse.Double.SparseMatrix.OfArray(
                structureStiffnessMatrix.Values
            );

            this.solver = solverFactory.GetSolver(stiffnessMatrix);
        }

        var dofDisplacementVectorValues = new double[degreeOfFreedomIds.Count];

        var knownReactionVector = this.BuildKnownJointReactionVector(loadCombination);
        this.solver.Solve(knownReactionVector.ToArray(), dofDisplacementVectorValues);

        VectorIdentified dofDisplacementVector = new(
            degreeOfFreedomIds,
            dofDisplacementVectorValues
        );

        return dofDisplacementVector;
    }

    internal VectorIdentified GetUnknownJointReactionVector(
        VectorIdentified unknownJointDisplacementVector
    )
    {
        var (_, boundaryConditionIds) = this.GetSortedUnsupportedStructureIds();

        VectorIdentified reactions = new(boundaryConditionIds);
        foreach (var element1D in dsmElement1Ds)
        {
            var globalMemberEndForcesVector = element1D.GetGlobalMemberEndForcesVectorIdentified(
                unknownJointDisplacementVector,
                unitSettings.ForceUnit,
                unitSettings.ForcePerLengthUnit,
                unitSettings.TorqueUnit
            );
            reactions.AddEntriesWithMatchingIdentifiers(globalMemberEndForcesVector);
        }
        return reactions;
    }

    private List<NodeResult> GetAnalyticalNodes(
        ResultSetId resultSetId,
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
                new NodeResult(modelId, resultSetId, nodeId, forcesResponse, displacementResponse)
            );
        }

        return analyticalNodes;
    }

    private void CreateStronglyTypedReactionsFromResultVectors(
        IEnumerable<KeyValuePair<UnsupportedStructureDisplacementId, double>> reactionVectorValues,
        Dictionary<CoordinateSystemDirection3D, Force> forceForces,
        Dictionary<CoordinateSystemDirection3D, Torque> forceTorques
    )
    {
        foreach (var kvp in reactionVectorValues)
        {
            if (kvp.Key.Direction.IsLinearDirection())
            {
                forceForces.Add(kvp.Key.Direction, new Force(kvp.Value, unitSettings.ForceUnit));
            }
            else
            {
                forceTorques.Add(kvp.Key.Direction, new Torque(kvp.Value, unitSettings.TorqueUnit));
            }
        }
    }

    private void CreateStronglyTypedDisplacementsFromResultVectors(
        IEnumerable<
            KeyValuePair<UnsupportedStructureDisplacementId, double>
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
                    new Length(kvp.Value, unitSettings.LengthUnit)
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

public record AnalysisResults
{
    public required ResultSet ResultSet { get; init; }
    public required OtherAnalyticalResults OtherAnalyticalResults { get; init; }
}

public record OtherAnalyticalResults
{
    public required ResultSetId Id { get; init; }
    public required ModelId ModelId { get; init; }
    public required ShearForceDiagram[] ShearDiagrams { get; init; }
    public required MomentDiagram[] MomentDiagrams { get; init; }
    public required DeflectionDiagrams[] DeflectionDiagrams { get; init; }
    public required GlobalStresses GlobalStresses { get; init; }
}

public readonly record struct GlobalStresses
{
    public required Force MaxShear { get; init; }
    public required Force MinShear { get; init; }
    public required Torque MaxMoment { get; init; }
    public required Torque MinMoment { get; init; }
}
