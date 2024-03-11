using BeamOs.Domain.AnalyticalResults.AnalyticalNodeAggregate;
using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using UnitsNet;

namespace BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate;

public class AnalyticalModel : BeamOSEntity<AnalyticalModelId>
{
    private AnalyticalModel()
        : base(new()) { }

    public UnitSettings UnitSettings { get; private set; }

    //public List<AnalyticalNodeId> NodeIds { get; set; }
    //public List<Element1DId> Element1DIds { get; set; }
    //private readonly AnalyticalModelSettings settings;
    //private readonly List<AnalyticalNodeId> analyticalNodeIds = [];
    //public IReadOnlyList<AnalyticalNodeId> AnalyticalNodeIds => this.analyticalNodeIds.AsReadOnly();
    //private readonly List<AnalyticalElement1DId> element1DIds = [];
    //public IReadOnlyList<AnalyticalElement1DId> Element1DIds => this.element1DIds.AsReadOnly();

    public MatrixIdentified StructureStiffnessMatrix { get; private set; }
    public VectorIdentified KnownJointDisplacementVector { get; private set; }
    public VectorIdentified UnknownJointDisplacementVector { get; private set; }
    public VectorIdentified KnownReactionVector { get; private set; }
    public VectorIdentified UnknownReactionVector { get; private set; }
    public List<AnalyticalNode> AnalyticalNodes { get; private set; }
    public List<UnsupportedStructureDisplacementId> DegreeOfFreedomIds { get; } = [];
    public List<UnsupportedStructureDisplacementId> BoundaryConditionIds { get; } = [];

    //public AnalyticalModel(
    //    UnitSettings unitSettings,
    //    VectorIdentified jointDisplacementVector,
    //    VectorIdentified reactionVector,
    //    List<UnsupportedStructureDisplacementId> degreeOfFreedomIds,
    //    List<UnsupportedStructureDisplacementId> boundaryConditionIds,
    //    AnalyticalModelId? id = null) : base(id ?? new())
    //{
    //    this.UnitSettings = unitSettings;
    //    this.JointDisplacementVector = jointDisplacementVector;
    //    this.ReactionVector = reactionVector;
    //    this.DegreeOfFreedomIds = degreeOfFreedomIds;
    //    this.BoundaryConditionIds = boundaryConditionIds;
    //}

    public static AnalyticalModel RunAnalysis(
        UnitSettings unitSettings,
        IEnumerable<AnalyticalElement1D> element1Ds,
        IEnumerable<DsmNode> nodes
    )
    {
        AnalyticalModel model = new() { UnitSettings = unitSettings };
        model.InitializeIdentifierMaps(nodes);
        model.StructureStiffnessMatrix = model.BuildStructureStiffnessMatrix(element1Ds);
        model.KnownJointDisplacementVector = model.BuildKnownDisplacementVector(nodes);
        model.UnknownJointDisplacementVector = model.GetJointDisplacementVector(nodes);
        model.UnknownReactionVector = model.GetReactionVector(element1Ds);
        model.AnalyticalNodes = model.GetAnalyticalNodes();

        return model;
    }

    private void InitializeIdentifierMaps(IEnumerable<DsmNode> nodes)
    {
        foreach (var node in nodes)
        {
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

                // if UnsupportedStructureDisplacement is degree of freedom
                if (node.Restraint.GetValueInDirection(direction) == true)
                {
                    this.DegreeOfFreedomIds.Add(new(node.Id, direction));
                }
                else
                {
                    this.BoundaryConditionIds.Add(new(node.Id, direction));
                }
            }
        }
    }

    private VectorIdentified GetReactionVector(IEnumerable<AnalyticalElement1D> element1Ds)
    {
        VectorIdentified reactions = new(this.BoundaryConditionIds);
        foreach (var element1D in element1Ds)
        {
            var globalMemberEndForcesVector = element1D.GetGlobalMemberEndForcesVectorIdentified(
                this.UnknownJointDisplacementVector,
                this.UnitSettings.ForceUnit,
                this.UnitSettings.ForcePerLengthUnit,
                this.UnitSettings.TorqueUnit
            );
            reactions.AddEntriesWithMatchingIdentifiers(globalMemberEndForcesVector);
        }

        return reactions;
    }

    private VectorIdentified GetJointDisplacementVector(IEnumerable<DsmNode> nodes)
    {
        this.KnownReactionVector = this.BuildKnownReactionVector(nodes);

        var dofDisplacementMathnetVector =
            this.StructureStiffnessMatrix.Build().Inverse() * this.KnownReactionVector.Build();
        VectorIdentified dofDisplacementVector =
            new(this.DegreeOfFreedomIds, dofDisplacementMathnetVector.ToArray());

        return dofDisplacementVector;
    }

    private VectorIdentified BuildKnownReactionVector(IEnumerable<DsmNode> nodes)
    {
        VectorIdentified loadVector = new(this.DegreeOfFreedomIds);
        foreach (var node in nodes)
        {
            var localLoadVector = node.GetForceVectorIdentifiedInGlobalCoordinates(
                this.UnitSettings.ForceUnit,
                this.UnitSettings.TorqueUnit
            );
            loadVector.AddEntriesWithMatchingIdentifiers(localLoadVector);
        }

        return loadVector;
    }

    private VectorIdentified BuildKnownDisplacementVector(IEnumerable<DsmNode> nodes)
    {
        // TODO : support non-zero known displacements
        double[] hardcodedNodeDisplacements = Enumerable
            .Repeat(0.0, this.BoundaryConditionIds.Count)
            .ToArray();

        return new VectorIdentified(this.BoundaryConditionIds, hardcodedNodeDisplacements);
    }

    private MatrixIdentified BuildStructureStiffnessMatrix(
        IEnumerable<AnalyticalElement1D> element1Ds
    )
    {
        MatrixIdentified sMatrix = new(this.DegreeOfFreedomIds);
        foreach (var element1D in element1Ds)
        {
            var globalMatrixWithIdentifiers = element1D.GetGlobalStiffnessMatrixIdentified(
                this.UnitSettings.ForceUnit,
                this.UnitSettings.ForcePerLengthUnit,
                this.UnitSettings.TorqueUnit
            );
            sMatrix.AddEntriesWithMatchingIdentifiers(globalMatrixWithIdentifiers);
        }

        return sMatrix;
    }

    private List<AnalyticalNode> GetAnalyticalNodes()
    {
        List<AnalyticalNode> analyticalNodes = [];

        var displacementVectorDict = this.UnknownJointDisplacementVector
            .Concat(this.KnownJointDisplacementVector)
            .GroupBy(kvp => kvp.Key.NodeId)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());
        var reactionVectorDict = this.UnknownReactionVector
            .Concat(this.KnownReactionVector)
            .GroupBy(kvp => kvp.Key.NodeId)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        // this is another way to do the above join and group. Is it better?
        //var x = (from urxns in this.UnknownReactionVector
        //         join krxns in this.KnownReactionVector on urxns.Key.NodeId equals krxns.Key.NodeId into rxns
        //         select rxns)
        //        .ToDictionary(enumerable => enumerable.First().Key.NodeId);

        foreach (var nodeId in displacementVectorDict.Keys)
        {
            Dictionary<CoordinateSystemDirection3D, Length> displacementLengths = [];
            Dictionary<CoordinateSystemDirection3D, Angle> displacementAngles = [];
            Dictionary<CoordinateSystemDirection3D, Force> forceForces = [];
            Dictionary<CoordinateSystemDirection3D, Torque> forceTorques = [];
            foreach (var kvp in displacementVectorDict[nodeId])
            {
                if (kvp.Key.Direction.IsLinearDirection())
                {
                    displacementLengths.Add(
                        kvp.Key.Direction,
                        new Length(kvp.Value, this.UnitSettings.LengthUnit)
                    );
                }
                else
                {
                    displacementAngles.Add(
                        kvp.Key.Direction,
                        new Angle(kvp.Value, UnitsNet.Units.AngleUnit.Radian) // TODO : support degree conversion
                    );
                }
            }
            foreach (var kvp in reactionVectorDict[nodeId])
            {
                if (kvp.Key.Direction.IsLinearDirection())
                {
                    forceForces.Add(
                        kvp.Key.Direction,
                        new Force(kvp.Value, this.UnitSettings.ForceUnit)
                    );
                }
                else
                {
                    forceTorques.Add(
                        kvp.Key.Direction,
                        new Torque(kvp.Value, this.UnitSettings.TorqueUnit)
                    );
                }
            }
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
            analyticalNodes.Add(new AnalyticalNode(nodeId, forcesResponse, displacementResponse));
        }

        return analyticalNodes;
    }
}
