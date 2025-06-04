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
        bool isAxisAligned = IsAxisAligned(startNode, endNode);
        var angleTolerance = isAxisAligned
            ? modelProposalBuilder
                .ModelRepairOperationParameters
                .UnfavorableOperationAngleTolerance
                .Radians
            : modelProposalBuilder
                .ModelRepairOperationParameters
                .StandardOperationAngleTolerance
                .Radians;

        ExtendElementToNode(
            startNode,
            endNode,
            beamsAndBracesCloseToStart,
            modelProposalBuilder,
            tolerance,
            angleTolerance
        );
        ExtendElementToNode(
            endNode,
            startNode,
            beamsAndBracesCloseToEnd,
            modelProposalBuilder,
            tolerance,
            angleTolerance
        );
    }

    private static void ExtendElementToNode(
        Node startNode,
        Node endNode,
        IEnumerable<Element1d> beamsAndBracesCloseToStart,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance,
        double angleTolerance
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

            // check if the current startNode, endNode, and candidateStartNode are roughly collinear
            if (
                !ModelRepairRuleUtils.ArePointsRoughlyCollinear(
                    startNode.LocationPoint,
                    endNode.LocationPoint,
                    candidateStartNode.LocationPoint,
                    angleTolerance
                )
            )
            {
                continue;
            }

            modelProposalBuilder.MergeNodes(startNode, candidateStartNode);
            break;
        }
    }

    // Returns true if the element is axis-aligned (X, Y, or Z constant)
    private static bool IsAxisAligned(Node startNode, Node endNode)
    {
        double x0 = startNode.LocationPoint.X.Meters;
        double y0 = startNode.LocationPoint.Y.Meters;
        double z0 = startNode.LocationPoint.Z.Meters;
        double x1 = endNode.LocationPoint.X.Meters;
        double y1 = endNode.LocationPoint.Y.Meters;
        double z1 = endNode.LocationPoint.Z.Meters;
        return x0 == x1 || y0 == y1 || z0 == z1;
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
