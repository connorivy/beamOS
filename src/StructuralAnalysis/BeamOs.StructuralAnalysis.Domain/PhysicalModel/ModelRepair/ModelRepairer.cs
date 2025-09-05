using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelAggregate;
using BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair.Constraints;
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
    // private static readonly List<IModelRepairRule> rules =
    // [
    //     new AlignBeamsIntoPlaneOfColumns(),
    //     new ExtendCoplanarElement1dsToJoinNodes(),
    //     new Element1dExtendOrShortenRule(),
    //     new ExtendElement1dsInPlaneToNodeRule(),
    //     new NodeMergeRule(),
    //     new NodeSnapToElement1dRule(),
    // ];

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

        ElementConstraintManager elementConstraintManager = new(nodeStore, element1dStore);
        elementConstraintManager.AddConstraints(model, modelRepairOperationParameters);

        ModelProposalBuilder modelProposal = new(
            model.Id,
            "Repair Proposal",
            "Proposed repairs for model connectivity",
            model.Settings,
            octree,
            modelRepairOperationParameters,
            nodeStore,
            element1dStore,
            elementConstraintManager
        );
        var context = new ModelRepairContext()
        {
            NodeStore = nodeStore,
            Element1dStore = element1dStore,
            ModelProposalBuilder = modelProposal,
            ModelRepairOperationParameters = modelRepairOperationParameters,
        };
        new RemoveOrphanedNodeRule(context).Apply();

        for (int i = 0; i < 3; i++)
        {
            var newRepairOptions = modelRepairOperationParameters with
            {
                VeryStrictTolerance =
                    modelRepairOperationParameters.VeryStrictTolerance * (i + 1) / 3.0,
                StrictTolerance = modelRepairOperationParameters.StrictTolerance * (i + 1) / 3.0,
                StandardTolerance =
                    modelRepairOperationParameters.StandardTolerance * (i + 1) / 3.0,
                RelaxedTolerance = modelRepairOperationParameters.RelaxedTolerance * (i + 1) / 3.0,
                VeryRelaxedTolerance =
                    modelRepairOperationParameters.VeryRelaxedTolerance * (i + 1) / 3.0,
            };
            context = context with { ModelRepairOperationParameters = newRepairOptions };
            foreach (var rule in GetRules(context))
            {
                try
                {
                    rule.Apply();
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
        yield return new AlignBeamsIntoPlaneOfColumns(context);
        yield return new ExtendCoplanarElement1dsToJoinNodes(context);
        yield return new Element1dExtendOrShortenRule(context);
        yield return new ExtendElement1dsInPlaneToNodeRule(context);
        yield return new NodeMergeRule(context);
        yield return new NodeSnapToElement1dRule(context);
    }
}
