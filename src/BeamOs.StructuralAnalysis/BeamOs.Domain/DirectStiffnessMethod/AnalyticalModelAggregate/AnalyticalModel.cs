using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.Models;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using MathNet.Numerics.LinearAlgebra;

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
    public VectorIdentified JointDisplacementVector { get; private set; }
    public VectorIdentified ReactionVector { get; private set; }
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
        IEnumerable<AnalyticalNode> nodes
    )
    {
        AnalyticalModel model = new() { UnitSettings = unitSettings };
        model.InitializeIdentifierMaps(nodes);
        model.StructureStiffnessMatrix = model.BuildStructureStiffnessMatrix(element1Ds);
        model.JointDisplacementVector = model.GetJointDisplacementVector(nodes);
        model.ReactionVector = model.GetReactionVector(element1Ds);

        return model;
    }

    private void InitializeIdentifierMaps(IEnumerable<AnalyticalNode> nodes)
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
                this.JointDisplacementVector,
                this.UnitSettings.ForceUnit,
                this.UnitSettings.ForcePerLengthUnit,
                this.UnitSettings.TorqueUnit
            );
            reactions.AddEntriesWithMatchingIdentifiers(globalMemberEndForcesVector);
        }

        return reactions;
    }

    private VectorIdentified GetJointDisplacementVector(IEnumerable<AnalyticalNode> nodes)
    {
        var loadVector = this.BuildLoadVector(nodes);

        var dofDisplacementMathnetVector =
            this.StructureStiffnessMatrix.Build().Inverse() * loadVector.Build();
        VectorIdentified dofDisplacementVector =
            new(this.DegreeOfFreedomIds, dofDisplacementMathnetVector.ToArray());

        return dofDisplacementVector;
    }

    private VectorIdentified BuildLoadVector(IEnumerable<AnalyticalNode> nodes)
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

    private static void AssignReactionValuesToNodes() { }
}
