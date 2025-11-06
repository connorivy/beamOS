using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

internal abstract class Element1dVisitingRule(ModelRepairContext context) : IModelRepairRule
{
    protected ModelRepairContext Context => context;

    public void Apply()
    {
        var tolerance = this.Context.ModelRepairOperationParameters.GetTolerance(this.RuleType);
        foreach (Element1d element in this.Context.ModelProposalBuilder.Element1ds)
        {
            var (startNode, endNode) = this.Context.ModelProposalBuilder.GetStartAndEndNodes(
                element,
                out _
            );
            var startNodeLocation = startNode.GetLocationPoint(
                this.Context.ModelProposalBuilder.Element1dStore,
                this.Context.ModelProposalBuilder.NodeStore
            );
            var endNodeLocation = endNode.GetLocationPoint(
                this.Context.ModelProposalBuilder.Element1dStore,
                this.Context.ModelProposalBuilder.NodeStore
            );

            var nearbyStartNodes = this
                .Context.ModelProposalBuilder.Octree.FindNodesWithin(
                    startNodeLocation,
                    tolerance.Meters,
                    startNode.Id
                )
                .Select(this.Context.ModelProposalBuilder.NodeStore.ApplyExistingProposal)
                .Distinct()
                .Where(node => node.Id != startNode.Id)
                .ToList();
            var nearbyEndNodes = this
                .Context.ModelProposalBuilder.Octree.FindNodesWithin(
                    endNodeLocation,
                    tolerance.Meters,
                    endNode.Id
                )
                .Select(this.Context.ModelProposalBuilder.NodeStore.ApplyExistingProposal)
                .Distinct()
                .Where(node => node.Id != endNode.Id)
                .ToList();

            var nearbyElement1dsAtStart = Element1dSpatialHelper.FindElement1dsWithin(
                this.Context.ModelProposalBuilder.Element1ds,
                this.Context.ModelProposalBuilder,
                startNodeLocation,
                tolerance.Meters,
                startNode.Id
            );
            var nearbyElement1dsAtEnd = Element1dSpatialHelper.FindElement1dsWithin(
                this.Context.ModelProposalBuilder.Element1ds,
                this.Context.ModelProposalBuilder,
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
                nearbyElement1dsAtStart.Where(e => !IsColumn(e, this.Context.ModelProposalBuilder)),
                nearbyElement1dsAtStart.Where(e => IsColumn(e, this.Context.ModelProposalBuilder)),
                nearbyEndNodes.OfType<Node>().ToList(),
                nearbyEndNodes.OfType<InternalNode>().ToList(),
                nearbyElement1dsAtEnd.Where(e => !IsColumn(e, this.Context.ModelProposalBuilder)),
                nearbyElement1dsAtEnd.Where(e => IsColumn(e, this.Context.ModelProposalBuilder)),
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

internal abstract class BeamOrBraceVisitingRule(ModelRepairContext context)
    : Element1dVisitingRule(context)
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
        Length tolerance
    )
    {
        if (IsColumn(startNodeLocation, endNodeLocation, this.Context.ModelProposalBuilder))
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
        Length tolerance
    );
}

internal abstract class ColumnVisitingRule(ModelRepairContext context)
    : Element1dVisitingRule(context)
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
        Length tolerance
    )
    {
        if (!IsColumn(startNodeLocation, endNodeLocation, this.Context.ModelProposalBuilder))
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
        Length tolerance
    );
}
