using BeamOs.StructuralAnalysis.Domain.Common;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public abstract class Element1dVisitingRule : IModelRepairRule
{
    public void Apply(ModelProposalBuilder modelProposal, Length tolerance)
    {
        foreach (Element1d element in modelProposal.Element1ds)
        {
            var (startNode, endNode) = modelProposal.GetStartAndEndNodes(element, out _);
            var startNodeLocation = startNode.GetLocationPoint(
                modelProposal.Element1dStore,
                modelProposal.NodeStore
            );
            var endNodeLocation = endNode.GetLocationPoint(
                modelProposal.Element1dStore,
                modelProposal.NodeStore
            );

            var nearbyStartNodes = modelProposal
                .Octree.FindNodeIdsWithin(startNodeLocation, tolerance.Meters, startNode.Id)
                .Select(modelProposal.NodeStore.ApplyExistingProposal)
                .Distinct()
                .Where(node => node.Id != startNode.Id)
                .ToList();
            var nearbyEndNodes = modelProposal
                .Octree.FindNodeIdsWithin(endNodeLocation, tolerance.Meters, endNode.Id)
                .Select(modelProposal.NodeStore.ApplyExistingProposal)
                .Distinct()
                .Where(node => node.Id != endNode.Id)
                .ToList();

            var nearbyElement1dsAtStart = Element1dSpatialHelper.FindElement1dsWithin(
                modelProposal.Element1ds,
                modelProposal,
                startNodeLocation,
                tolerance.Meters,
                startNode.Id
            );
            var nearbyElement1dsAtEnd = Element1dSpatialHelper.FindElement1dsWithin(
                modelProposal.Element1ds,
                modelProposal,
                endNodeLocation,
                tolerance.Meters,
                endNode.Id
            );

            this.ApplyRule(
                element,
                startNode,
                endNode,
                startNodeLocation,
                endNodeLocation,
                nearbyStartNodes.OfType<Node>().ToList(),
                nearbyStartNodes.OfType<InternalNode>().ToList(),
                nearbyElement1dsAtStart.Where(e => !IsColumn(e, modelProposal)),
                nearbyElement1dsAtStart.Where(e => IsColumn(e, modelProposal)),
                nearbyEndNodes.OfType<Node>().ToList(),
                nearbyEndNodes.OfType<InternalNode>().ToList(),
                nearbyElement1dsAtEnd.Where(e => !IsColumn(e, modelProposal)),
                nearbyElement1dsAtEnd.Where(e => IsColumn(e, modelProposal)),
                modelProposal,
                tolerance
            );
        }
    }

    public abstract ModelRepairRuleType RuleType { get; }

    protected abstract void ApplyRule(
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
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    );

    protected static bool IsColumn(Element1d element, ModelProposalBuilder modelProposalBuilder)
    {
        var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(element);
        var startNodeLocation = startNode.GetLocationPoint(
            modelProposalBuilder.Element1dStore,
            modelProposalBuilder.NodeStore
        );
        var endNodeLocation = endNode.GetLocationPoint(
            modelProposalBuilder.Element1dStore,
            modelProposalBuilder.NodeStore
        );
        return IsColumn(startNodeLocation, endNodeLocation, modelProposalBuilder);
    }

    protected static bool IsColumn(
        Node startNode,
        Node endNode,
        ModelProposalBuilder modelProposalBuilder
    ) => IsColumn(startNode.LocationPoint, endNode.LocationPoint, modelProposalBuilder);

    protected static bool IsColumn(
        Point start,
        Point end,
        ModelProposalBuilder modelProposalBuilder
    )
    {
        // Use the up axis to determine if the element is a column:
        // If yAxisUp: column is vertical in Y (start.Y != end.Y, and X/Z nearly constant)
        // If !yAxisUp: column is vertical in Z (start.Z != end.Z, and X/Y nearly constant)
        bool yAxisUp = modelProposalBuilder.Settings?.YAxisUp ?? true;
        double tol = 1e-6; // meters, for planarity check
        if (yAxisUp)
        {
            // Check if X and Z are nearly constant, and Y is not
            bool xConst = Math.Abs(start.X.Meters - end.X.Meters) < tol;
            bool zConst = Math.Abs(start.Z.Meters - end.Z.Meters) < tol;
            bool yDiff = Math.Abs(start.Y.Meters - end.Y.Meters) > tol;
            return xConst && zConst && yDiff;
        }
        else
        {
            // Check if X and Y are nearly constant, and Z is not
            bool xConst = Math.Abs(start.X.Meters - end.X.Meters) < tol;
            bool yConst = Math.Abs(start.Y.Meters - end.Y.Meters) < tol;
            bool zDiff = Math.Abs(start.Z.Meters - end.Z.Meters) > tol;
            return xConst && yConst && zDiff;
        }
    }
}

public abstract class BeamOrBraceVisitingRule : Element1dVisitingRule
{
    protected override void ApplyRule(
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
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        if (IsColumn(startNodeLocation, endNodeLocation, modelProposalBuilder))
        {
            // If the element is a column, skip this rule
            return;
        }
        this.ApplyRuleForBeamOrBrace(
            element1D,
            startNode,
            endNode,
            startNodeLocation,
            endNodeLocation,
            nearbyStartNodes,
            nearbyStartInternalNodes,
            beamsAndBracesCloseToStart,
            columnsCloseToStart,
            nearbyEndNodes,
            nearbyEndInternalNodes,
            beamsAndBracesCloseToEnd,
            columnsCloseToEnd,
            modelProposalBuilder,
            tolerance
        );
    }

    protected abstract void ApplyRuleForBeamOrBrace(
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
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    );
}

public abstract class ColumnVisitingRule : Element1dVisitingRule
{
    protected override void ApplyRule(
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
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    )
    {
        if (!IsColumn(startNodeLocation, endNodeLocation, modelProposalBuilder))
        {
            // If the element is a not a column, skip this rule
            return;
        }
        this.ApplyRuleForColumn(
            element1D,
            startNode,
            endNode,
            startNodeLocation,
            endNodeLocation,
            nearbyStartNodes,
            nearbyStartInternalNodes,
            beamsAndBracesCloseToStart,
            columnsCloseToStart,
            nearbyEndNodes,
            nearbyEndInternalNodes,
            beamsAndBracesCloseToEnd,
            columnsCloseToEnd,
            modelProposalBuilder,
            tolerance
        );
    }

    protected abstract void ApplyRuleForColumn(
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
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    );
}
