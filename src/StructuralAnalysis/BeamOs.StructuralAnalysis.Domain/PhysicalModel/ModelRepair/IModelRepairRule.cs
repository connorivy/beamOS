using BeamOs.StructuralAnalysis.Domain.PhysicalModel.Element1dAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public interface IModelRepairRule
{
    public void Apply(
        Element1d element1D,
        Node startNode,
        Node endNode,
        IList<Node> nearbyStartNodes,
        IList<Element1d> element1DsCloseToStart,
        IList<Node> nearbyEndNodes,
        IList<Element1d> element1DsCloseToEnd,
        ModelProposalBuilder modelProposal,
        double tolerance
    );
}
