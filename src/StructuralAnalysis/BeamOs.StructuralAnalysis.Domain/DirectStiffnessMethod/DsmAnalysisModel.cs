using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;

public class DsmAnalysisModel(Model model)
{
    private readonly DsmElement1d[] dsmElement1Ds = model
        .Settings
        .AnalysisSettings
        .Element1DAnalysisType switch
    {
        Element1dAnalysisType.Euler
            => model.Element1ds.Select(el => new DsmElement1d(el)).ToArray(),
        Element1dAnalysisType.Timoshenko
            => model.Element1ds.Select(el => new TimoshenkoDsmElement1d(el)).ToArray(),
        Element1dAnalysisType.Undefined
        or _
            => throw new Exception(
                $"Unsupported Element1DAnalysisType {model.Settings.AnalysisSettings.Element1DAnalysisType}"
            )
    };

    private readonly DsmNodeVo[] dsmNodes = model.Nodes.Select(el => new DsmNodeVo(el)).ToArray();

    public ResultSet RunAnalysis()
    {
        //var (degreeOfFreedomIds, boundaryConditionIds) = this.GetSortedUnsupportedStructureIds();

        //MatrixIdentified structureStiffnessMatrix = this.BuildStructureStiffnessMatrix();

        VectorIdentified knownJointDisplacementVector = this.BuildKnownJointDisplacementVector();

        VectorIdentified knownReactionVector = this.BuildKnownJointReactionVector();

        VectorIdentified unknownJointDisplacementVector = this.GetUnknownJointDisplacementVector();

        VectorIdentified unknownReactionVector = this.GetUnknownJointReactionVector();

        ResultSetId resultSetId = new();
        List<NodeResult> nodeResults = this.GetAnalyticalNodes(
            resultSetId,
            unknownJointDisplacementVector,
            knownJointDisplacementVector,
            unknownReactionVector,
            knownReactionVector
        );

        ShearForceDiagram[] shearForceDiagrams = new ShearForceDiagram[this.dsmElement1Ds.Length];
        MomentDiagram[] momentDiagrams = new MomentDiagram[this.dsmElement1Ds.Length];
        Element1dResult[] element1DResults = new Element1dResult[this.dsmElement1Ds.Length];

        double globalShearMin = double.MaxValue;
        double globalShearMax = double.MinValue;
        double globalMomentMin = double.MaxValue;
        double globalMomentMax = double.MinValue;

        for (int i = 0; i < this.dsmElement1Ds.Length; i++)
        {
            double shearMin = double.MaxValue;
            double shearMax = double.MinValue;
            double momentMin = double.MaxValue;
            double momentMax = double.MinValue;

            var localMemberEndForcesVector = this.dsmElement1Ds[i].GetLocalMemberEndForcesVector(
                unknownJointDisplacementVector,
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.ForcePerLengthUnit,
                model.Settings.UnitSettings.TorqueUnit
            );

            var sfd = ShearForceDiagram.Create(
                model.Id,
                resultSetId,
                this.dsmElement1Ds[i].Element1dId,
                this.dsmElement1Ds[i].StartPoint,
                this.dsmElement1Ds[i].EndPoint,
                this.dsmElement1Ds[i].SectionProfileRotation,
                this.dsmElement1Ds[i].Length,
                localMemberEndForcesVector,
                model.Settings.UnitSettings.LengthUnit,
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.TorqueUnit,
                LinearCoordinateDirection3D.AlongY
            );
            shearForceDiagrams[i] = sfd;
            sfd.MinMax(ref shearMin, ref shearMax);

            momentDiagrams[i] = MomentDiagram.Create(
                model.Id,
                resultSetId,
                this.dsmElement1Ds[i].Element1dId,
                this.dsmElement1Ds[i].StartPoint,
                this.dsmElement1Ds[i].EndPoint,
                this.dsmElement1Ds[i].SectionProfileRotation,
                this.dsmElement1Ds[i].Length,
                localMemberEndForcesVector,
                model.Settings.UnitSettings.LengthUnit,
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.TorqueUnit,
                LinearCoordinateDirection3D.AlongY,
                sfd
            );
            momentDiagrams[i].MinMax(ref momentMin, ref momentMax);

            var localElementDisplacements = this.dsmElement1Ds[i].GetLocalEndDisplacementVector(
                unknownJointDisplacementVector
            );

            List<double[]> hello = [];

            int numIntervals = 10;
            double beamLength = this.dsmElement1Ds[i]
                .Length
                .As(model.Settings.UnitSettings.LengthUnit);

            double displacementMin = double.MaxValue;
            double displacementMax = double.MinValue;
            for (int j = 0; j < numIntervals; j++)
            {
                double step = (double)j / numIntervals;

                var displacements = DeflectedShapeShapeFunctionCalculator.Solve(
                    step,
                    beamLength,
                    localElementDisplacements
                );

                var displacement = Math.Sqrt(
                    Math.Pow(displacements[0], 2)
                        + Math.Pow(displacements[1], 2)
                        + Math.Pow(displacements[2], 2)
                );

                displacementMin = Math.Min(displacementMin, displacement);
                displacementMax = Math.Max(displacementMax, displacement);
            }

            Element1dResult element1DResult =
                new(model.Id, resultSetId, this.dsmElement1Ds[i].Element1dId)
                {
                    MaxMoment = new(momentMax, model.Settings.UnitSettings.TorqueUnit),
                    MinMoment = new(momentMin, model.Settings.UnitSettings.TorqueUnit),
                    MaxShear = new(shearMax, model.Settings.UnitSettings.ForceUnit),
                    MinShear = new(shearMin, model.Settings.UnitSettings.ForceUnit),
                    MaxDisplacement = new(displacementMax, model.Settings.UnitSettings.LengthUnit),
                    MinDisplacement = new(displacementMin, model.Settings.UnitSettings.LengthUnit),
                    LocalStartForces = new Forces(
                        new(localMemberEndForcesVector[0], model.Settings.UnitSettings.ForceUnit),
                        new(localMemberEndForcesVector[1], model.Settings.UnitSettings.ForceUnit),
                        new(localMemberEndForcesVector[2], model.Settings.UnitSettings.ForceUnit),
                        new(localMemberEndForcesVector[3], model.Settings.UnitSettings.TorqueUnit),
                        new(localMemberEndForcesVector[4], model.Settings.UnitSettings.TorqueUnit),
                        new(localMemberEndForcesVector[5], model.Settings.UnitSettings.TorqueUnit)
                    ),
                    LocalEndForces = new Forces(
                        new(localMemberEndForcesVector[6], model.Settings.UnitSettings.ForceUnit),
                        new(localMemberEndForcesVector[7], model.Settings.UnitSettings.ForceUnit),
                        new(localMemberEndForcesVector[8], model.Settings.UnitSettings.ForceUnit),
                        new(localMemberEndForcesVector[9], model.Settings.UnitSettings.TorqueUnit),
                        new(localMemberEndForcesVector[10], model.Settings.UnitSettings.TorqueUnit),
                        new(localMemberEndForcesVector[11], model.Settings.UnitSettings.TorqueUnit)
                    ),
                    LocalStartDisplacements = new Displacements(
                        new(localElementDisplacements[0], model.Settings.UnitSettings.LengthUnit),
                        new(localElementDisplacements[1], model.Settings.UnitSettings.LengthUnit),
                        new(localElementDisplacements[2], model.Settings.UnitSettings.LengthUnit),
                        new(localElementDisplacements[3], AngleUnit.Radian),
                        new(localElementDisplacements[4], AngleUnit.Radian),
                        new(localElementDisplacements[5], AngleUnit.Radian)
                    ),
                    LocalEndDisplacements = new Displacements(
                        new(localElementDisplacements[6], model.Settings.UnitSettings.LengthUnit),
                        new(localElementDisplacements[7], model.Settings.UnitSettings.LengthUnit),
                        new(localElementDisplacements[8], model.Settings.UnitSettings.LengthUnit),
                        new(localElementDisplacements[9], AngleUnit.Radian),
                        new(localElementDisplacements[10], AngleUnit.Radian),
                        new(localElementDisplacements[11], AngleUnit.Radian)
                    )
                };
            element1DResults[i] = element1DResult;

            globalShearMin = Math.Min(globalShearMin, shearMin);
            globalShearMax = Math.Max(globalShearMax, shearMax);
            globalMomentMin = Math.Min(globalMomentMin, momentMin);
            globalMomentMax = Math.Max(globalMomentMax, momentMax);
        }

        ResultSet resultSet =
            new(model.Id, resultSetId)
            {
                NodeResults = nodeResults,
                ShearForceDiagrams = shearForceDiagrams,
                MomentDiagrams = momentDiagrams,
                Element1dResults = element1DResults,
            };

        //AnalyticalResults analyticalModelResults =
        //    new(
        //        model.Id,
        //        new(shearMax, model.Settings.UnitSettings.ForceUnit),
        //        new(shearMin, model.Settings.UnitSettings.ForceUnit),
        //        new(momentMax, model.Settings.UnitSettings.TorqueUnit),
        //        new(momentMin, model.Settings.UnitSettings.TorqueUnit),
        //        resultSetId
        //    )
        //    {
        //        NodeResults = nodeResults,
        //        ShearForceDiagrams = shearForceDiagrams,
        //        MomentDiagrams = momentDiagrams,
        //    };

        return resultSet;
    }

    private static void AssignMinMax(ref double min, ref double max, double rootValue)
    {
        min = Math.Min(min, rootValue);
        max = Math.Max(max, rootValue);
    }

    private SortedUnsupportedStructureIds? sortedUnsupportedStructureIds;

    internal SortedUnsupportedStructureIds GetSortedUnsupportedStructureIds()
    {
        if (this.sortedUnsupportedStructureIds is not null)
        {
            return this.sortedUnsupportedStructureIds;
        }

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
        foreach (var element1D in this.dsmElement1Ds)
        {
            var globalMatrixWithIdentifiers = element1D.GetGlobalStiffnessMatrixIdentified(
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.ForcePerLengthUnit,
                model.Settings.UnitSettings.TorqueUnit
            );
            sMatrix.AddEntriesWithMatchingIdentifiers(globalMatrixWithIdentifiers);
        }

        return this.structuralStiffnessMatrix = sMatrix;
    }

    private VectorIdentified? knownJointDisplacementVector;

    private VectorIdentified BuildKnownJointDisplacementVector()
    {
        if (this.knownJointDisplacementVector is not null)
        {
            return this.knownJointDisplacementVector;
        }

        var (_, boundaryConditionIds) = this.GetSortedUnsupportedStructureIds();

        // TODO : support non-zero known displacements
        double[] hardcodedNodeDisplacements = Enumerable
            .Repeat(0.0, boundaryConditionIds.Count)
            .ToArray();

        return this.knownJointDisplacementVector = new VectorIdentified(
            boundaryConditionIds,
            hardcodedNodeDisplacements
        );
    }

    private VectorIdentified? knownJointReactionVector;

    internal VectorIdentified BuildKnownJointReactionVector()
    {
        if (this.knownJointReactionVector is not null)
        {
            return this.knownJointReactionVector;
        }

        var (degreeOfFreedomIds, _) = this.GetSortedUnsupportedStructureIds();
        VectorIdentified loadVector = new(degreeOfFreedomIds);
        foreach (var node in this.dsmNodes)
        {
            var localLoadVector = node.GetForceVectorIdentifiedInGlobalCoordinates(
                model.Settings.UnitSettings.ForceUnit,
                model.Settings.UnitSettings.TorqueUnit
            );
            loadVector.AddEntriesWithMatchingIdentifiers(localLoadVector);
        }

        return this.knownJointReactionVector = loadVector;
    }

    private VectorIdentified? unknownDisplacementVector;

    internal VectorIdentified GetUnknownJointDisplacementVector()
    {
        if (this.unknownDisplacementVector is not null)
        {
            return this.unknownDisplacementVector;
        }

        var (degreeOfFreedomIds, _) = this.GetSortedUnsupportedStructureIds();
        var structureStiffnessMatrix = this.BuildStructureStiffnessMatrix();
        var knownReactionVector = this.BuildKnownJointReactionVector();

        var dofDisplacementMathnetVector = structureStiffnessMatrix
            .Build()
            .Solve(knownReactionVector.Build());
        VectorIdentified dofDisplacementVector =
            new(degreeOfFreedomIds, dofDisplacementMathnetVector.Cast<double>().ToArray());

        return this.unknownDisplacementVector = dofDisplacementVector;
    }

    private VectorIdentified? unknownReactionVector;

    internal VectorIdentified GetUnknownJointReactionVector()
    {
        if (this.unknownReactionVector is not null)
        {
            return this.unknownReactionVector;
        }

        var (_, boundaryConditionIds) = this.GetSortedUnsupportedStructureIds();
        var unknownJointDisplacementVector = this.GetUnknownJointDisplacementVector();

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

        return this.unknownReactionVector = reactions;
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
                new NodeResult(model.Id, resultSetId, nodeId, forcesResponse, displacementResponse)
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
