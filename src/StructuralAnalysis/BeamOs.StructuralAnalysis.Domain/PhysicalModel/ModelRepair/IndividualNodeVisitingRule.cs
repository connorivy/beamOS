using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public abstract class IndividualNodeVisitingRule : IModelRepairRule
{
    public void Apply(ModelProposalBuilder modelProposalBuilder, Length tolerance)
    {
        HashSet<NodeId> visitedNodeIds = [];
        foreach (Element1d element in modelProposalBuilder.Element1ds)
        {
            var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(element, out _);

            if (visitedNodeIds.Add(startNode.Id))
            {
                this.ApplyToSingleNode(modelProposalBuilder, tolerance, element, startNode);
            }
            if (visitedNodeIds.Add(endNode.Id))
            {
                this.ApplyToSingleNode(modelProposalBuilder, tolerance, element, endNode);
            }
        }
    }

    private void ApplyToSingleNode(
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance,
        Element1d element,
        Node node
    )
    {
        List<Node> nearbyNodes = modelProposalBuilder
            .Octree.FindNodesWithin(node.LocationPoint, tolerance.Meters, node.Id)
            .Select(modelProposalBuilder.ApplyExistingProposal)
            .Distinct()
            .Where(n => n.Id != node.Id)
            .ToList();

        List<Element1d> nearbyElement1ds = Element1dSpatialHelper.FindElement1dsWithin(
            modelProposalBuilder.Element1ds,
            modelProposalBuilder,
            node.LocationPoint,
            tolerance.Meters,
            node.Id
        );

        this.ApplyToSingleNode(
            element,
            node,
            nearbyNodes,
            nearbyElement1ds,
            modelProposalBuilder,
            tolerance
        );
    }

    protected abstract void ApplyToSingleNode(
        Element1d element,
        Node node,
        IList<Node> nearbyNodes,
        IList<Element1d> nearbyElement1ds,
        ModelProposalBuilder modelProposalBuilder,
        Length tolerance
    );
}

public abstract class BeamAndBraceVisitingRule : IModelRepairRule
{
    public void Apply(ModelProposalBuilder modelProposal, Length tolerance)
    {
        foreach (Element1d element in modelProposal.Element1ds)
        {
            var (startNode, endNode) = modelProposal.GetStartAndEndNodes(element, out _);

            var nearbyStartNodes = modelProposal
                .Octree.FindNodesWithin(startNode.LocationPoint, tolerance.Meters, startNode.Id)
                .Select(modelProposal.ApplyExistingProposal)
                .Distinct()
                .Where(node => node.Id != startNode.Id)
                .ToList();
            var nearbyEndNodes = modelProposal
                .Octree.FindNodesWithin(endNode.LocationPoint, tolerance.Meters, endNode.Id)
                .Select(modelProposal.ApplyExistingProposal)
                .Distinct()
                .Where(node => node.Id != endNode.Id)
                .ToList();

            var nearbyElement1dsAtStart = Element1dSpatialHelper.FindElement1dsWithin(
                modelProposal.Element1ds,
                modelProposal,
                startNode.LocationPoint,
                tolerance.Meters,
                startNode.Id
            );
            var nearbyElement1dsAtEnd = Element1dSpatialHelper.FindElement1dsWithin(
                modelProposal.Element1ds,
                modelProposal,
                endNode.LocationPoint,
                tolerance.Meters,
                endNode.Id
            );

            this.ApplyToBothElementNodes(
                element,
                startNode,
                endNode,
                nearbyStartNodes,
                nearbyElement1dsAtStart.Where(e => !IsColumn(e, modelProposal)),
                nearbyElement1dsAtStart.Where(e => IsColumn(e, modelProposal)),
                nearbyEndNodes,
                nearbyElement1dsAtEnd.Where(e => !IsColumn(e, modelProposal)),
                nearbyElement1dsAtEnd.Where(e => IsColumn(e, modelProposal)),
                modelProposal,
                tolerance
            );
        }
    }

    protected abstract void ApplyToBothElementNodes(
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
    );

    protected static bool IsColumn(Element1d element, ModelProposalBuilder modelProposalBuilder)
    {
        // Use the up axis to determine if the element is a column:
        // If yAxisUp: column is vertical in Y (start.Y != end.Y, and X/Z nearly constant)
        // If !yAxisUp: column is vertical in Z (start.Z != end.Z, and X/Y nearly constant)
        bool yAxisUp = modelProposalBuilder.Settings?.YAxisUp ?? true;
        var (startNode, endNode) = modelProposalBuilder.GetStartAndEndNodes(element);
        var end = endNode.LocationPoint;
        var start = startNode.LocationPoint;
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
