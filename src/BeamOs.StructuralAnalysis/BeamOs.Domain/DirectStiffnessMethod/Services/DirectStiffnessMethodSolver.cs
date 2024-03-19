using BeamOs.Domain.Common.Enums;
using BeamOs.Domain.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalElement1DAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalModelAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate;
using BeamOs.Domain.DirectStiffnessMethod.AnalyticalNodeAggregate.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Common.ValueObjects;
using BeamOs.Domain.DirectStiffnessMethod.Services.ValueObjects;

namespace BeamOs.Domain.DirectStiffnessMethod.Services;

public sealed class DirectStiffnessMethodSolver(
    UnitSettings unitSettings,
    List<AnalyticalElement1D> element1ds,
    List<DsmNode> nodes
)
{
    //public MatrixIdentified StructureStiffnessMatrix { get; private set; }
    //public VectorIdentified KnownJointDisplacementVector { get; private set; }
    //public VectorIdentified UnknownJointDisplacementVector { get; private set; }
    //public VectorIdentified KnownReactionVector { get; private set; }
    //public VectorIdentified UnknownReactionVector { get; private set; }
    //public List<UnsupportedStructureDisplacementId> DegreeOfFreedomIds { get; } = [];
    //public List<UnsupportedStructureDisplacementId> BoundaryConditionIds { get; } = [];

    ////public AnalyticalModel(
    ////    UnitSettings unitSettings,
    ////    VectorIdentified jointDisplacementVector,
    ////    VectorIdentified reactionVector,
    ////    List<UnsupportedStructureDisplacementId> degreeOfFreedomIds,
    ////    List<UnsupportedStructureDisplacementId> boundaryConditionIds,
    ////    AnalyticalModelId? id = null) : base(id ?? new())
    ////{
    ////    unitSettings = unitSettings;
    ////    this.JointDisplacementVector = jointDisplacementVector;
    ////    this.ReactionVector = reactionVector;
    ////    this.DegreeOfFreedomIds = degreeOfFreedomIds;
    ////    this.BoundaryConditionIds = boundaryConditionIds;
    ////}

    //public AnalyticalModel RunAnalysis(
    //    UnitSettings unitSettings,
    //    IEnumerable<AnalyticalElement1D> element1Ds,
    //    IEnumerable<AnalyticalNode> nodes
    //)
    //{
    //    AnalyticalModel model = new() { UnitSettings = unitSettings };
    //    var sortedUnsupportedStructureIds = this.GetSortedUnsupportedStructureIds();

    //    MatrixIdentified structureStiffnessMatrix = this.BuildStructureStiffnessMatrix();

    //    model.StructureStiffnessMatrix = model.BuildStructureStiffnessMatrix(element1Ds);
    //    model.KnownJointDisplacementVector = model.BuildKnownDisplacementVector(nodes);
    //    model.UnknownJointDisplacementVector = model.GetJointDisplacementVector(nodes);
    //    model.UnknownReactionVector = model.GetReactionVector(element1Ds);

    //    return model;
    //}

    //private SortedUnsupportedStructureIds GetSortedUnsupportedStructureIds()
    //{
    //    List<UnsupportedStructureDisplacementId> degreeOfFreedomIds = [];
    //    List<UnsupportedStructureDisplacementId> boundaryConditionIds = [];
    //    foreach (var node in nodes)
    //    {
    //        foreach (
    //            CoordinateSystemDirection3D direction in Enum.GetValues(
    //                typeof(CoordinateSystemDirection3D)
    //            )
    //        )
    //        {
    //            if (direction == CoordinateSystemDirection3D.Undefined)
    //            {
    //                continue;
    //            }

    //            // if UnsupportedStructureDisplacement is degree of freedom
    //            if (node.Restraint.GetValueInDirection(direction) == true)
    //            {
    //                degreeOfFreedomIds.Add(new(node.Id, direction));
    //            }
    //            else
    //            {
    //                boundaryConditionIds.Add(new(node.Id, direction));
    //            }
    //        }
    //    }
    //    return new SortedUnsupportedStructureIds(
    //        degreeOfFreedomIds: degreeOfFreedomIds,
    //        boundaryConditionIds: boundaryConditionIds
    //    );
    //}

    //private VectorIdentified GetReactionVector(IEnumerable<AnalyticalElement1D> element1Ds)
    //{
    //    VectorIdentified reactions = new(this.BoundaryConditionIds);
    //    foreach (var element1D in element1Ds)
    //    {
    //        var globalMemberEndForcesVector = element1D.GetGlobalMemberEndForcesVectorIdentified(
    //            this.UnknownJointDisplacementVector,
    //            unitSettings.ForceUnit,
    //            unitSettings.ForcePerLengthUnit,
    //            unitSettings.TorqueUnit
    //        );
    //        reactions.AddEntriesWithMatchingIdentifiers(globalMemberEndForcesVector);
    //    }

    //    return reactions;
    //}

    //private VectorIdentified GetJointDisplacementVector(IEnumerable<AnalyticalNode> nodes)
    //{
    //    this.KnownReactionVector = this.BuildKnownReactionVector(nodes);

    //    var dofDisplacementMathnetVector =
    //        this.StructureStiffnessMatrix.Build().Inverse() * this.KnownReactionVector.Build();
    //    VectorIdentified dofDisplacementVector =
    //        new(this.DegreeOfFreedomIds, dofDisplacementMathnetVector.ToArray());

    //    return dofDisplacementVector;
    //}

    //private VectorIdentified BuildKnownReactionVector(IEnumerable<AnalyticalNode> nodes)
    //{
    //    VectorIdentified loadVector = new(this.DegreeOfFreedomIds);
    //    foreach (var node in nodes)
    //    {
    //        var localLoadVector = node.GetForceVectorIdentifiedInGlobalCoordinates(
    //            unitSettings.ForceUnit,
    //            unitSettings.TorqueUnit
    //        );
    //        loadVector.AddEntriesWithMatchingIdentifiers(localLoadVector);
    //    }

    //    return loadVector;
    //}

    //private VectorIdentified BuildKnownDisplacementVector(IEnumerable<AnalyticalNode> nodes)
    //{
    //    // TODO : support non-zero known displacements
    //    double[] hardcodedNodeDisplacements = Enumerable
    //        .Repeat(0.0, this.BoundaryConditionIds.Count)
    //        .ToArray();

    //    return new VectorIdentified(this.BoundaryConditionIds, hardcodedNodeDisplacements);
    //}

    //private MatrixIdentified BuildStructureStiffnessMatrix(
    //    List<UnsupportedStructureDisplacementId> degreeOfFreedomIds
    //)
    //{
    //    MatrixIdentified sMatrix = new(degreeOfFreedomIds);
    //    foreach (var element1D in element1ds)
    //    {
    //        var globalMatrixWithIdentifiers = element1D.GetGlobalStiffnessMatrixIdentified(
    //            unitSettings.ForceUnit,
    //            unitSettings.ForcePerLengthUnit,
    //            unitSettings.TorqueUnit
    //        );
    //        sMatrix.AddEntriesWithMatchingIdentifiers(globalMatrixWithIdentifiers);
    //    }

    //    return sMatrix;
    //}
}
