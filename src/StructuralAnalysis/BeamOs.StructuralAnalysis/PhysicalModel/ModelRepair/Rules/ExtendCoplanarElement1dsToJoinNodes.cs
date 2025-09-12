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
internal sealed class ExtendCoplanarElement1dsToJoinNodes(ModelRepairContext context)
    : BeamOrBraceVisitingRule(context)
{
    public override ModelRepairRuleType RuleType => ModelRepairRuleType.Standard;

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
        var axisAlignmentTolerance =
            this.Context.ModelProposalBuilder.ModelRepairOperationParameters.GetAxisAlignmentTolerance(
                startNodeLocation,
                endNodeLocation
            );

        if (startNode is Node startNodeAsNode)
        {
            ExtendElementsToJoinNodes(
                startNodeAsNode,
                startNodeLocation,
                endNodeLocation,
                beamsAndBracesCloseToStart,
                this.Context.ModelProposalBuilder,
                tolerance,
                axisAlignmentTolerance
            );
        }
        if (endNode is Node endNodeAsNode)
        {
            ExtendElementsToJoinNodes(
                endNodeAsNode,
                endNodeLocation,
                startNodeLocation,
                beamsAndBracesCloseToEnd,
                this.Context.ModelProposalBuilder,
                tolerance,
                axisAlignmentTolerance
            );
        }
    }

    private static void ExtendElementsToJoinNodes(
        Node startNode,
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

            // check if the current element and the candidate element can be extended to meet
            // each other within the specified tolerances
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

            if (
                ModelRepairRuleUtils.TryFindApproximateIntersection(
                    startNodeLocation,
                    endNodeLocation,
                    candidateStartNode.GetLocationPoint(
                        modelProposalBuilder.Element1dStore,
                        modelProposalBuilder.NodeStore
                    ),
                    candidateEndNode.GetLocationPoint(
                        modelProposalBuilder.Element1dStore,
                        modelProposalBuilder.NodeStore
                    ),
                    out var intersection
                )
            )
            {
                var intersectionPoint = new Point(
                    new(intersection.X, LengthUnit.Meter),
                    new(intersection.Y, LengthUnit.Meter),
                    new(intersection.Z, LengthUnit.Meter)
                );
                var startNodeDist = startNodeLocation.CalculateDistance(
                    intersectionPoint.X,
                    intersectionPoint.Y,
                    intersectionPoint.Z
                );
                var candidateStartNodeDist = candidateStartNodeLocation.CalculateDistance(
                    intersectionPoint.X,
                    intersectionPoint.Y,
                    intersectionPoint.Z
                );
                var candidateEndNodeDist = candidateEndNodeLocation.CalculateDistance(
                    intersectionPoint.X,
                    intersectionPoint.Y,
                    intersectionPoint.Z
                );

                var (candidateNodeToKeep, candidateNodeToKeepLocation) =
                    candidateStartNodeDist < candidateEndNodeDist
                        ? (candidateStartNode, candidateStartNodeLocation)
                        : (candidateEndNode, candidateEndNodeLocation);

                var candidateAxisAlignmentTolerance =
                    modelProposalBuilder.ModelRepairOperationParameters.GetAxisAlignmentTolerance(
                        candidateStartNodeLocation,
                        candidateEndNodeLocation
                    );

                if (
                    ModelRepairRuleUtils.PointWithinTolerances(
                        startNodeLocation,
                        intersection,
                        xAxisLengthTolerance,
                        yAxisLengthTolerance,
                        zAxisLengthTolerance,
                        modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                    )
                    && ModelRepairRuleUtils.PointWithinTolerances(
                        candidateNodeToKeepLocation,
                        intersection,
                        modelProposalBuilder.ModelRepairOperationParameters.GetLengthTolerance(
                            candidateAxisAlignmentTolerance.X
                        ),
                        modelProposalBuilder.ModelRepairOperationParameters.GetLengthTolerance(
                            candidateAxisAlignmentTolerance.Y
                        ),
                        modelProposalBuilder.ModelRepairOperationParameters.GetLengthTolerance(
                            candidateAxisAlignmentTolerance.Z
                        ),
                        modelProposalBuilder.ModelRepairOperationParameters.VeryRelaxedTolerance
                    )
                )
                {
                    modelProposalBuilder.NodeStore.AddNodeProposal(
                        new NodeProposal(startNode, modelProposalBuilder.Id, intersectionPoint)
                    );
                    modelProposalBuilder.MergeNodes(candidateNodeToKeep, startNode);
                }
                break;
            }
        }
    }

    private static Length ShortestDistanceTo(
        Point nodeLocation,
        Element1d element1d,
        ModelProposalBuilder modelProposalBuilder
    )
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
}
