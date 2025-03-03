using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.MomentDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams.ShearForceDiagramAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.NodeResultAggregate;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.StructuralAnalysis.Domain.DirectStiffnessMethod;

public class DsmAnalysisModel
{
    public DsmAnalysisModel(Model model, UnitSettings? unitSettings = null)
    {
        this.dsmElement1Ds = model.Settings.AnalysisSettings.Element1DAnalysisType switch
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

        this.dsmNodes = model.Nodes.Select(el => new DsmNodeVo(el)).ToArray();
        this.unitSettings = unitSettings ?? model.Settings.UnitSettings;
        this.modelId = model.Id;
    }

    private readonly ModelId modelId;
    private readonly UnitSettings unitSettings;

    private readonly DsmElement1d[] dsmElement1Ds;

    private readonly DsmNodeVo[] dsmNodes;

    public AnalysisResults RunAnalysis()
    {
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

        //ShearForceDiagram[] shearForceDiagrams = new ShearForceDiagram[this.dsmElement1Ds.Length];
        //MomentDiagram[] momentDiagrams = new MomentDiagram[this.dsmElement1Ds.Length];
        //Element1dResult[] element1DResults = new Element1dResult[this.dsmElement1Ds.Length];
        //DeflectionDiagrams[] displacementResults = new DeflectionDiagrams[
        //    this.dsmElement1Ds.Length
        //];

        //GlobalStresses globalStresses = this.CalculateElement1dResults(
        //    unknownJointDisplacementVector,
        //    resultSetId,
        //    shearForceDiagrams,
        //    momentDiagrams,
        //    displacementResults,
        //    element1DResults
        //);

        ResultSet resultSet =
            new(this.modelId, resultSetId)
            {
                NodeResults = nodeResults,
                //Element1dResults = element1DResults,
            };

        var otherResults = resultSet.ComputeDiagramsAndElement1dResults(
            this.dsmElement1Ds,
            this.unitSettings
        );

        //return new()
        //{
        //    ResultSet = resultSet,
        //    OtherAnalyticalResults = new()
        //    {
        //        Id = resultSetId,
        //        ModelId = modelId,
        //        ShearDiagrams = shearForceDiagrams,
        //        MomentDiagrams = momentDiagrams,
        //        DeflectionDiagrams = displacementResults,
        //        GlobalStresses = globalStresses
        //    }
        //};

        return new() { ResultSet = resultSet, OtherAnalyticalResults = otherResults };
    }

    //private GlobalStresses CalculateElement1dResults(
    //    VectorIdentified unknownJointDisplacementVector,
    //    ResultSetId resultSetId,
    //    ShearForceDiagram[] shearForceDiagrams,
    //    MomentDiagram[] momentDiagrams,
    //    DeflectionDiagrams[] displacementResults,
    //    Element1dResult[] element1DResults
    //)
    //{
    //    double globalShearMin = double.MaxValue;
    //    double globalShearMax = double.MinValue;
    //    double globalMomentMin = double.MaxValue;
    //    double globalMomentMax = double.MinValue;

    //    for (int i = 0; i < this.dsmElement1Ds.Length; i++)
    //    {
    //        double shearMin = double.MaxValue;
    //        double shearMax = double.MinValue;
    //        double momentMin = double.MaxValue;
    //        double momentMax = double.MinValue;

    //        var localMemberEndForcesVector = this.dsmElement1Ds[i].GetLocalMemberEndForcesVector(
    //            unknownJointDisplacementVector,
    //            this.unitSettings.ForceUnit,
    //            this.unitSettings.ForcePerLengthUnit,
    //            this.unitSettings.TorqueUnit
    //        );

    //        var sfd = ShearForceDiagram.Create(
    //            this.modelId,
    //            resultSetId,
    //            this.dsmElement1Ds[i].Element1dId,
    //            this.dsmElement1Ds[i].StartPoint,
    //            this.dsmElement1Ds[i].EndPoint,
    //            this.dsmElement1Ds[i].SectionProfileRotation,
    //            this.dsmElement1Ds[i].Length,
    //            localMemberEndForcesVector,
    //            this.unitSettings.LengthUnit,
    //            this.unitSettings.ForceUnit,
    //            this.unitSettings.TorqueUnit,
    //            LinearCoordinateDirection3D.AlongY
    //        );
    //        shearForceDiagrams[i] = sfd;
    //        sfd.MinMax(ref shearMin, ref shearMax);

    //        momentDiagrams[i] = MomentDiagram.Create(
    //            this.modelId,
    //            resultSetId,
    //            this.dsmElement1Ds[i].Element1dId,
    //            this.dsmElement1Ds[i].StartPoint,
    //            this.dsmElement1Ds[i].EndPoint,
    //            this.dsmElement1Ds[i].SectionProfileRotation,
    //            this.dsmElement1Ds[i].Length,
    //            localMemberEndForcesVector,
    //            this.unitSettings.LengthUnit,
    //            this.unitSettings.ForceUnit,
    //            this.unitSettings.TorqueUnit,
    //            LinearCoordinateDirection3D.AlongY,
    //            sfd
    //        );
    //        momentDiagrams[i].MinMax(ref momentMin, ref momentMax);

    //        var localElementDisplacements = this.dsmElement1Ds[i].GetLocalEndDisplacementVector(
    //            unknownJointDisplacementVector
    //        );

    //        displacementResults[i] = DeflectionDiagrams.Create(
    //            this.dsmElement1Ds[i].Element1dId,
    //            this.dsmElement1Ds[i].StartPoint,
    //            this.dsmElement1Ds[i].EndPoint,
    //            this.dsmElement1Ds[i].SectionProfileRotation,
    //            this.unitSettings.LengthUnit,
    //            localElementDisplacements,
    //            out var displacementMin,
    //            out var displacementMax
    //        );

    //        element1DResults[i] = new(this.modelId, resultSetId, this.dsmElement1Ds[i].Element1dId)
    //        {
    //            MaxMoment = new(momentMax, this.unitSettings.TorqueUnit),
    //            MinMoment = new(momentMin, this.unitSettings.TorqueUnit),
    //            MaxShear = new(shearMax, this.unitSettings.ForceUnit),
    //            MinShear = new(shearMin, this.unitSettings.ForceUnit),
    //            MaxDisplacement = new(displacementMax, this.unitSettings.LengthUnit),
    //            MinDisplacement = new(displacementMin, this.unitSettings.LengthUnit),
    //            //LocalStartForces = new Forces(
    //            //    new(localMemberEndForcesVector[0], this.unitSettings.ForceUnit),
    //            //    new(localMemberEndForcesVector[1], this.unitSettings.ForceUnit),
    //            //    new(localMemberEndForcesVector[2], this.unitSettings.ForceUnit),
    //            //    new(localMemberEndForcesVector[3], this.unitSettings.TorqueUnit),
    //            //    new(localMemberEndForcesVector[4], this.unitSettings.TorqueUnit),
    //            //    new(localMemberEndForcesVector[5], this.unitSettings.TorqueUnit)
    //            //),
    //            //LocalEndForces = new Forces(
    //            //    new(localMemberEndForcesVector[6], this.unitSettings.ForceUnit),
    //            //    new(localMemberEndForcesVector[7], this.unitSettings.ForceUnit),
    //            //    new(localMemberEndForcesVector[8], this.unitSettings.ForceUnit),
    //            //    new(localMemberEndForcesVector[9], this.unitSettings.TorqueUnit),
    //            //    new(localMemberEndForcesVector[10], this.unitSettings.TorqueUnit),
    //            //    new(localMemberEndForcesVector[11], this.unitSettings.TorqueUnit)
    //            //),
    //            //LocalStartDisplacements = new Displacements(
    //            //    new(localElementDisplacements[0], this.unitSettings.LengthUnit),
    //            //    new(localElementDisplacements[1], this.unitSettings.LengthUnit),
    //            //    new(localElementDisplacements[2], this.unitSettings.LengthUnit),
    //            //    new(localElementDisplacements[3], AngleUnit.Radian),
    //            //    new(localElementDisplacements[4], AngleUnit.Radian),
    //            //    new(localElementDisplacements[5], AngleUnit.Radian)
    //            //),
    //            //LocalEndDisplacements = new Displacements(
    //            //    new(localElementDisplacements[6], this.unitSettings.LengthUnit),
    //            //    new(localElementDisplacements[7], this.unitSettings.LengthUnit),
    //            //    new(localElementDisplacements[8], this.unitSettings.LengthUnit),
    //            //    new(localElementDisplacements[9], AngleUnit.Radian),
    //            //    new(localElementDisplacements[10], AngleUnit.Radian),
    //            //    new(localElementDisplacements[11], AngleUnit.Radian)
    //            //)
    //        };

    //        globalShearMin = Math.Min(globalShearMin, shearMin);
    //        globalShearMax = Math.Max(globalShearMax, shearMax);
    //        globalMomentMin = Math.Min(globalMomentMin, momentMin);
    //        globalMomentMax = Math.Max(globalMomentMax, momentMax);
    //    }

    //    return new()
    //    {
    //        MaxMoment = new(globalMomentMax, this.unitSettings.TorqueUnit),
    //        MaxShear = new(globalShearMax, this.unitSettings.ForceUnit),
    //        MinMoment = new(globalMomentMin, this.unitSettings.TorqueUnit),
    //        MinShear = new(globalShearMin, this.unitSettings.ForceUnit)
    //    };
    //}

    private SortedUnsupportedStructureIds? sortedUnsupportedStructureIds;

    internal SortedUnsupportedStructureIds GetSortedUnsupportedStructureIds()
    {
        if (this.sortedUnsupportedStructureIds is not null)
        {
            return this.sortedUnsupportedStructureIds;
        }

        List<UnsupportedStructureDisplacementId> degreeOfFreedomIds = [];
        List<UnsupportedStructureDisplacementId> boundaryConditionIds = [];
        foreach (var dsmNode in this.dsmNodes)
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
        foreach (var element1D in this.dsmElement1Ds)
        {
            var globalMatrixWithIdentifiers = element1D.GetGlobalStiffnessMatrixIdentified(
                this.unitSettings.ForceUnit,
                this.unitSettings.ForcePerLengthUnit,
                this.unitSettings.TorqueUnit
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

    private VectorIdentified? knownJointReactionVector;

    internal VectorIdentified BuildKnownJointReactionVector()
    {
        if (this.knownJointReactionVector is null)
        {
            var (degreeOfFreedomIds, _) = this.GetSortedUnsupportedStructureIds();
            VectorIdentified loadVector = new(degreeOfFreedomIds);
            foreach (var node in this.dsmNodes)
            {
                var localLoadVector = node.GetForceVectorIdentifiedInGlobalCoordinates(
                    this.unitSettings.ForceUnit,
                    this.unitSettings.TorqueUnit
                );
                loadVector.AddEntriesWithMatchingIdentifiers(localLoadVector);
            }

            this.knownJointReactionVector = loadVector;
        }

        return this.knownJointReactionVector.Value;
    }

    private VectorIdentified? unknownDisplacementVector;

    internal VectorIdentified GetUnknownJointDisplacementVector()
    {
        if (this.unknownDisplacementVector is null)
        {
            var (degreeOfFreedomIds, _) = this.GetSortedUnsupportedStructureIds();
            var structureStiffnessMatrix = this.BuildStructureStiffnessMatrix();
            var knownReactionVector = this.BuildKnownJointReactionVector();

            var dofDisplacementMathnetVector = structureStiffnessMatrix
                .Build()
                .Solve(knownReactionVector.Build());
            VectorIdentified dofDisplacementVector =
                new(degreeOfFreedomIds, dofDisplacementMathnetVector.Cast<double>().ToArray());
            this.unknownDisplacementVector = dofDisplacementVector;
        }

        return this.unknownDisplacementVector.Value;
    }

    private VectorIdentified? unknownReactionVector;

    internal VectorIdentified GetUnknownJointReactionVector()
    {
        if (this.unknownReactionVector is null)
        {
            var (_, boundaryConditionIds) = this.GetSortedUnsupportedStructureIds();
            var unknownJointDisplacementVector = this.GetUnknownJointDisplacementVector();

            VectorIdentified reactions = new(boundaryConditionIds);
            foreach (var element1D in this.dsmElement1Ds)
            {
                var globalMemberEndForcesVector =
                    element1D.GetGlobalMemberEndForcesVectorIdentified(
                        unknownJointDisplacementVector,
                        this.unitSettings.ForceUnit,
                        this.unitSettings.ForcePerLengthUnit,
                        this.unitSettings.TorqueUnit
                    );
                reactions.AddEntriesWithMatchingIdentifiers(globalMemberEndForcesVector);
            }
            this.unknownReactionVector = reactions;
        }
        return this.unknownReactionVector.Value;
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
                new NodeResult(
                    this.modelId,
                    resultSetId,
                    nodeId,
                    forcesResponse,
                    displacementResponse
                )
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
                forceForces.Add(
                    kvp.Key.Direction,
                    new Force(kvp.Value, this.unitSettings.ForceUnit)
                );
            }
            else
            {
                forceTorques.Add(
                    kvp.Key.Direction,
                    new Torque(kvp.Value, this.unitSettings.TorqueUnit)
                );
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
                    new Length(kvp.Value, this.unitSettings.LengthUnit)
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
