using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Rules;

/// <summary>
/// Rule: Extend Element1ds to join nodes if both lie in the same plane and can both be extended
/// within tolerances to meet each other
///
/// Sketch:
///
///   o---------o-o  (Element1d A)
///                \
///                 o
///                  \
///                   o (Element1d B)
///
/// </summary>
public sealed class ExtendCoplanarElement1dsToJoinNodes : BeamOrBraceVisitingRule
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Standard;

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

        ExtendElementsToJoinNodes(
            startNode,
            endNode,
            beamsAndBracesCloseToStart,
            modelProposalBuilder,
            tolerance,
            axisAlignmentTolerance
        );
        ExtendElementsToJoinNodes(
            endNode,
            startNode,
            beamsAndBracesCloseToEnd,
            modelProposalBuilder,
            tolerance,
            axisAlignmentTolerance
        );
    }

    private static void ExtendElementsToJoinNodes(
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
                    startNode.LocationPoint,
                    endNode.LocationPoint,
                    candidateStartNode.LocationPoint,
                    candidateEndNode.LocationPoint
                )
            )
            {
                continue;
            }

            // check if the current element and the candidate element can be extended to meet
            // each other within the specified tolerances
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
