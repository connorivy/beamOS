namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public interface IModelRepairRule
{
    public void Apply(ModelProposalBuilder modelProposal, Length tolerance);

    // public void ApplyToBothElementNodes(
    //     Element1d element1D,
    //     Node startNode,
    //     Node endNode,
    //     IList<Node> nearbyStartNodes,
    //     IList<Element1d> element1DsCloseToStart,
    //     IList<Node> nearbyEndNodes,
    //     IList<Element1d> element1DsCloseToEnd,
    //     ModelProposalBuilder modelProposal,
    //     Length tolerance
    // );

    // public void ApplyToSingleElementNode(
    //     Element1d element1D,
    //     Node node,
    //     IList<Node> nearbyStartNodes,
    //     IList<Element1d> element1DsCloseToStart,
    //     ModelProposalBuilder modelProposal,
    //     Length tolerance
    // );
}
