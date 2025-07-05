using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Rules;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;
using Microsoft.Extensions.Logging;

namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public class ModelRepairer(
    ModelRepairOperationParameters modelRepairOperationParameters,
    ILogger logger
)
{
    // private readonly IList<Node> nodes;
    // private readonly IList<Element1d> element1ds;
    private static readonly List<IModelRepairRule> rules =
    [
        new AlignBeamsIntoPlaneOfColumns(),
        new ExtendCoplanarElement1dsToJoinNodes(),
        new Element1dExtendOrShortenRule(),
        new ExtendElement1dsInPlaneToNodeRule(),
        new NodeMergeRule(),
        new NodeSnapToElement1dRule(),
    ];

    public ModelProposal ProposeRepairs(Model model)
    {
        ModelProposalElement1dStore element1dStore = new(model.Element1ds);
        ModelProposalNodeStore nodeStore = new(model.Nodes, model.InternalNodes);

        Octree octree = new(model.Id, model.Nodes[0].LocationPoint, 10.0);
        foreach (Node node in model.Nodes)
        {
            octree.Add(node);
        }
        foreach (var internalNode in model.InternalNodes)
        {
            octree.Add(internalNode, element1dStore, nodeStore);
        }
        ModelProposalBuilder modelProposal = new(
            model.Id,
            "Repair Proposal",
            "Proposed repairs for model connectivity",
            model.Settings,
            octree,
            modelRepairOperationParameters,
            nodeStore,
            element1dStore
        );

        new RemoveOrphanedNodeRule().Apply(modelProposal, Length.Zero);

        for (int i = 0; i < 3; i++)
        {
            foreach (var rule in rules)
            {
                if (rule.GetType() == typeof(ExtendElement1dsInPlaneToNodeRule) && i == 1)
                {
                    ;
                }
                try
                {
                    rule.Apply(
                        modelProposal,
                        modelRepairOperationParameters.GetToleranceForRule(rule) * (i + 1) / 3.0
                    );
                }
                catch (Exception ex)
                {
                    // Log the exception and continue with the next rule
                    logger.LogError(
                        ex,
                        "Error applying rule {RuleName} on iteration {Iteration}",
                        rule.GetType().Name,
                        i
                    );
                }
            }
        }
        return modelProposal.Build();
    }

    public static IEnumerable<IModelRepairRule> GetRules(ModelRepairContext context)
    {
        yield return new AlignBeamsIntoPlaneOfColumns();
        yield return new ExtendCoplanarElement1dsToJoinNodes();
        yield return new Element1dExtendOrShortenRule();
        yield return new ExtendElement1dsInPlaneToNodeRule();
        yield return new NodeMergeRule();
        yield return new NodeSnapToElement1dRule();
    }
}
