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
public sealed class ExtendElement1dsInPlaneToNodeRule : BeamOrBraceVisitingRule
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Favorable;

    protected override void ApplyRuleForBeamOrBrace(
        Element1d element1D,
        Node startNode,
        Node endNode,
        IList<Node> nearbyStartNodes,
        IEnumerable<Element1d> beamsAndBracesCloseToStart,
        IEnumerable<Element1d> columnsCloseToStart,
        IList<Node> nearbyEndNodes,
        IEnumerable<Element1d> beamsAndBracesCloseToEnd,
        IEnumerable<Element1d> columnsCloseToEnd,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        var axisAlignmentTolerance =
            modelProposalBuilder.ModelRepairOperationParameters.GetAxisAlignmentTolerance(
                startNode,
                endNode
            );

        ExtendElementToNode(
            startNode,
            endNode,
            beamsAndBracesCloseToStart,
            modelProposalBuilder,
            tolerance,
            axisAlignmentTolerance
        );
        ExtendElementToNode(
            endNode,
            startNode,
            beamsAndBracesCloseToEnd,
            modelProposalBuilder,
            tolerance,
            axisAlignmentTolerance
        );
    }

    private static void ExtendElementToNode(
        Node startNode,
        Node endNode,
        IEnumerable<Element1d> beamsAndBracesCloseToStart,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance,
        AxisAlignmentTolerance axisAlignmentTolerance
    )
    {
        var elementsAndDistance = beamsAndBracesCloseToStart
            .Select(e => (e, ShortestDistanceTo(startNode, e, modelProposalBuilder)))
            .Where(tuple => tuple.Item2 < tolerance)
            .OrderBy(tuple => tuple.Item2)
            .ToList();

        foreach (var (candidateElement, distance) in elementsAndDistance)
        {
            var (candidateStartNode, candidateEndNode) = modelProposalBuilder.GetStartAndEndNodes(
                candidateElement
            );
            // Check if the candidate element is coplanar with the current element1D
            if (
                !ModelRepairRuleUtils.ArePointsRoughlyCoplanar(
                    startNode.LocationPoint.ToVector3InMeters(),
                    endNode.LocationPoint.ToVector3InMeters(),
                    candidateStartNode.LocationPoint.ToVector3InMeters(),
                    candidateEndNode.LocationPoint.ToVector3InMeters()
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
                    startNode.LocationPoint,
                    endNode.LocationPoint,
                    candidateStartNode.LocationPoint,
                    xAxisLengthTolerance,
                    yAxisLengthTolerance,
                    zAxisLengthTolerance,
                    modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                )
                || ModelRepairRuleUtils.CanLineEndpointBeExtendedToPointWithinTolerance(
                    endNode.LocationPoint,
                    startNode.LocationPoint,
                    candidateStartNode.LocationPoint,
                    xAxisLengthTolerance,
                    yAxisLengthTolerance,
                    zAxisLengthTolerance,
                    modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                );
            bool mergeWithCandidateEndNode =
                ModelRepairRuleUtils.CanLineEndpointBeExtendedToPointWithinTolerance(
                    startNode.LocationPoint,
                    endNode.LocationPoint,
                    candidateEndNode.LocationPoint,
                    xAxisLengthTolerance,
                    yAxisLengthTolerance,
                    zAxisLengthTolerance,
                    modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                )
                || ModelRepairRuleUtils.CanLineEndpointBeExtendedToPointWithinTolerance(
                    endNode.LocationPoint,
                    startNode.LocationPoint,
                    candidateEndNode.LocationPoint,
                    xAxisLengthTolerance,
                    yAxisLengthTolerance,
                    zAxisLengthTolerance,
                    modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                );

            if (!mergeWithCandidateStartNode && !mergeWithCandidateEndNode)
            {
                continue; // neither end is collinear, skip this candidate
            }

            if (mergeWithCandidateStartNode)
            {
                modelProposalBuilder.MergeNodes(startNode, candidateStartNode);
            }
            if (mergeWithCandidateEndNode)
            {
                modelProposalBuilder.MergeNodes(startNode, candidateEndNode);
            }
            break;
        }
    }

    private static Length ShortestDistanceTo(
        Node node,
        Element1d element1d,
        ModelProposalBuilder modelProposalBuilder
    )
    {
        var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(element1d);
        return node.LocationPoint.ShortestDistanceToLine(
            startNode.LocationPoint,
            endNode.LocationPoint
        );
    }
}
