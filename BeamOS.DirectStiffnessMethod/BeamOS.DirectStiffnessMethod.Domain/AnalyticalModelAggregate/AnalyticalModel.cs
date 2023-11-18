using BeamOS.Common.Domain.Enums;
using BeamOS.Common.Domain.Models;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalElement1DAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate;
using BeamOS.DirectStiffnessMethod.Domain.AnalyticalNodeAggregate.ValueObjects;
using BeamOS.DirectStiffnessMethod.Domain.Common.ValueObjects;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOS.DirectStiffnessMethod.Domain.AnalyticalModelAggregate;
public class AnalyticalModel : BeamOSEntity<AnalyticalModelId>
{
    private AnalyticalModel() : base(new()) { }
    public UnitSettings UnitSettings { get; private set; }
    //public List<AnalyticalNodeId> NodeIds { get; set; }
    //public List<Element1DId> Element1DIds { get; set; }
    //private readonly AnalyticalModelSettings settings;
    //private readonly List<AnalyticalNodeId> analyticalNodeIds = [];
    //public IReadOnlyList<AnalyticalNodeId> AnalyticalNodeIds => this.analyticalNodeIds.AsReadOnly();
    //private readonly List<AnalyticalElement1DId> element1DIds = [];
    //public IReadOnlyList<AnalyticalElement1DId> Element1DIds => this.element1DIds.AsReadOnly();

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
        List<AnalyticalElement1D> element1Ds,
        List<AnalyticalNode> nodes)
    {
        AnalyticalModel model = new()
        {
            UnitSettings = unitSettings
        };
        model.InitializeIdentifierMaps(nodes);
        model.JointDisplacementVector = model.GetJointDisplacementVector(element1Ds, nodes);
        model.ReactionVector = model.GetReactionVector(element1Ds);

        return model;
    }

    private void InitializeIdentifierMaps(IEnumerable<AnalyticalNode> nodes)
    {
        foreach (var node in nodes)
        {
            foreach (CoordinateSystemDirection3D direction in Enum.GetValues(typeof(CoordinateSystemDirection3D)))
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


    private VectorIdentified GetReactionVector(List<AnalyticalElement1D> element1Ds)
    {
        VectorIdentified reactions = new(this.BoundaryConditionIds);
        foreach (AnalyticalElement1D element1D in element1Ds)
        {
            VectorIdentified globalMemberEndForcesVector = element1D.GetGlobalMemberEndForcesVectorIdentified(this.JointDisplacementVector);
            reactions.AddEntriesWithMatchingIdentifiers(globalMemberEndForcesVector);
        }

        return reactions;
    }

    private VectorIdentified GetJointDisplacementVector(
        List<AnalyticalElement1D> element1Ds,
        IEnumerable<AnalyticalNode> nodes)
    {
        MatrixIdentified<UnsupportedStructureDisplacementId> sMatrix = this
            .BuildStructureStiffnessMatrix(element1Ds);
        VectorIdentified loadVector = this.BuildLoadVector(nodes);

        Vector<double> dofDisplacementMathnetVector = sMatrix.Build().Inverse() * loadVector.Build();
        VectorIdentified dofDisplacementVector = new(
            this.DegreeOfFreedomIds,
            dofDisplacementMathnetVector.ToArray());

        return dofDisplacementVector;
    }

    private VectorIdentified BuildLoadVector(IEnumerable<AnalyticalNode> nodes)
    {
        VectorIdentified loadVector = new(this.DegreeOfFreedomIds);
        foreach (AnalyticalNode node in nodes)
        {
            VectorIdentified localLoadVector = node
                .GetForceVectorIdentifiedInGlobalCoordinates(this.UnitSettings.ForceUnit, this.UnitSettings.TorqueUnit);
            loadVector.AddEntriesWithMatchingIdentifiers(localLoadVector);
        }

        return loadVector;
    }

    private MatrixIdentified<UnsupportedStructureDisplacementId> BuildStructureStiffnessMatrix(
        List<AnalyticalElement1D> element1Ds)
    {
        MatrixIdentified<UnsupportedStructureDisplacementId> sMatrix = new(this.DegreeOfFreedomIds);
        foreach (AnalyticalElement1D element1D in element1Ds)
        {
            MatrixIdentified<UnsupportedStructureDisplacementId> globalMatrixWithIdentifiers = element1D
                .GetGlobalStiffnessMatrixIdentified();
            sMatrix.AddEntriesWithMatchingIdentifiers(globalMatrixWithIdentifiers);
        }

        return sMatrix;
    }
}
