using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Rules;

/// <summary>
/// Rule: Extend an Element1d to join a node of another Element1d if both lie in the same plane.
///
/// Sketch:
///
///   o---------o   (Element1d A)
///             \
///              o
///               \
///                o (Element1d B)
///
/// If A and B are coplanar and nearly collinear (within tolerance),
/// extend A or B so their endpoints coincide at a node.
///
/// - If the element is axis-aligned (X, Y, or Z constant), use strict tolerance.
/// - If not, allow greater rotational tolerance for extension.
/// </summary>
public sealed class ExtendElement1dsInPlaneToNodeRule(ModelRepairContext context)
    : BeamOrBraceVisitingRule(context)
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Favorable;

    protected override void ApplyRuleForBeamOrBrace(
        Element1d element1D,
        NodeDefinition startNode,
        NodeDefinition endNode,
        Point startNodeLocation,
        Point endNodeLocation,
        IList<Node> nearbyStartNodes,
        IList<InternalNode> nearbyStartInternalNodes,
        IEnumerable<Element1d> beamsAndBracesCloseToStart,
        IEnumerable<Element1d> columnsCloseToStart,
        IList<Node> nearbyEndNodes,
        IList<InternalNode> nearbyEndInternalNodes,
        IEnumerable<Element1d> beamsAndBracesCloseToEnd,
        IEnumerable<Element1d> columnsCloseToEnd,
        Length tolerance
    )
    {
        var modelProposalBuilder = this.Context.ModelProposalBuilder;
        var axisAlignmentTolerance =
            modelProposalBuilder.ModelRepairOperationParameters.GetAxisAlignmentTolerance(
                startNodeLocation,
                endNodeLocation
            );

        ExtendElementToNode(
            startNode,
            endNode,
            startNodeLocation,
            endNodeLocation,
            beamsAndBracesCloseToStart,
            modelProposalBuilder,
            tolerance,
            axisAlignmentTolerance
        );
        ExtendElementToNode(
            endNode,
            startNode,
            endNodeLocation,
            startNodeLocation,
            beamsAndBracesCloseToEnd,
            modelProposalBuilder,
            tolerance,
            axisAlignmentTolerance
        );
    }

    private static void ExtendElementToNode(
        NodeDefinition startNode,
        NodeDefinition endNode,
        Point startNodeLocation,
        Point endNodeLocation,
        IEnumerable<Element1d> beamsAndBracesCloseToStart,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance,
        AxisAlignmentTolerance axisAlignmentTolerance
    )
    {
        var elementsAndDistance = beamsAndBracesCloseToStart
            .Select(e => (e, ShortestDistanceTo(startNodeLocation, e, modelProposalBuilder)))
            .Where(tuple => tuple.Item2 < tolerance)
            .OrderBy(tuple => tuple.Item2)
            .ToList();

        foreach (var (candidateElement, distance) in elementsAndDistance)
        {
            var (candidateStartNode, candidateEndNode) = modelProposalBuilder.GetStartAndEndNodes(
                candidateElement
            );
            var candidateStartNodeLocation = candidateStartNode.GetLocationPoint(
                modelProposalBuilder.Element1dStore,
                modelProposalBuilder.NodeStore
            );
            var candidateEndNodeLocation = candidateEndNode.GetLocationPoint(
                modelProposalBuilder.Element1dStore,
                modelProposalBuilder.NodeStore
            );
            // Check if the candidate element is coplanar with the current element1D
            if (
                !ModelRepairRuleUtils.ArePointsRoughlyCoplanar(
                    startNodeLocation.ToVector3InMeters(),
                    endNodeLocation.ToVector3InMeters(),
                    candidateStartNodeLocation.ToVector3InMeters(),
                    candidateEndNodeLocation.ToVector3InMeters()
                )
            )
            {
                continue;
            }

            // check if the current startNode, endNode, and candidateStartNode are roughly collinear
            var xAxisLengthTolerance =
                modelProposalBuilder.ModelRepairOperationParameters.GetLengthTolerance(
                    axisAlignmentTolerance.X
                );
            var yAxisLengthTolerance =
                modelProposalBuilder.ModelRepairOperationParameters.GetLengthTolerance(
                    axisAlignmentTolerance.Y
                );
            var zAxisLengthTolerance =
                modelProposalBuilder.ModelRepairOperationParameters.GetLengthTolerance(
                    axisAlignmentTolerance.Z
                );

            bool mergeWithCandidateStartNode =
                ModelRepairRuleUtils.CanLineEndpointBeExtendedToPointWithinTolerance(
                    startNodeLocation,
                    endNodeLocation,
                    candidateStartNodeLocation,
                    xAxisLengthTolerance,
                    yAxisLengthTolerance,
                    zAxisLengthTolerance,
                    modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                )
                || ModelRepairRuleUtils.CanLineEndpointBeExtendedToPointWithinTolerance(
                    endNodeLocation,
                    startNodeLocation,
                    candidateStartNodeLocation,
                    xAxisLengthTolerance,
                    yAxisLengthTolerance,
                    zAxisLengthTolerance,
                    modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                );
            bool mergeWithCandidateEndNode =
                ModelRepairRuleUtils.CanLineEndpointBeExtendedToPointWithinTolerance(
                    startNodeLocation,
                    endNodeLocation,
                    candidateEndNodeLocation,
                    xAxisLengthTolerance,
                    yAxisLengthTolerance,
                    zAxisLengthTolerance,
                    modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                )
                || ModelRepairRuleUtils.CanLineEndpointBeExtendedToPointWithinTolerance(
                    endNodeLocation,
                    startNodeLocation,
                    candidateEndNodeLocation,
                    xAxisLengthTolerance,
                    yAxisLengthTolerance,
                    zAxisLengthTolerance,
                    modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                );

            if (!mergeWithCandidateStartNode && !mergeWithCandidateEndNode)
            {
                continue; // neither end is collinear, skip this candidate
            }

            if (
                mergeWithCandidateStartNode
                && !candidateStartNode.DependsOnNode(
                    startNode.Id,
                    modelProposalBuilder.Element1dStore,
                    modelProposalBuilder.NodeStore
                )
            )
            {
                modelProposalBuilder.MergeNodes(startNode, candidateStartNode);
            }
            if (
                mergeWithCandidateEndNode
                && !candidateEndNode.DependsOnNode(
                    startNode.Id,
                    modelProposalBuilder.Element1dStore,
                    modelProposalBuilder.NodeStore
                )
            )
            {
                modelProposalBuilder.MergeNodes(startNode, candidateEndNode);
            }
            break;
        }
    }

    private static Length ShortestDistanceTo(
        Point nodeLocation,
        Element1d element1d,
        ModelProposalBuilder modelProposalBuilder
    )
    {
        if (element1d.Id.Id == 134)
        {
            ;
        }
        try
        {
            var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(element1d);
            return nodeLocation.ShortestDistanceToLine(
                startNode.GetLocationPoint(
                    modelProposalBuilder.Element1dStore,
                    modelProposalBuilder.NodeStore
                ),
                endNode.GetLocationPoint(
                    modelProposalBuilder.Element1dStore,
                    modelProposalBuilder.NodeStore
                )
            );
        }
        catch (Exception ex)
        {
            ;
            throw;
        }
    }
}
