namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public interface IModelRepairRule
{
    public void Apply(ModelProposalBuilder modelProposal, Length tolerance);

    public ModelRepairRuleType RuleType { get; }
}

public enum ModelRepairRuleType
{
    Undefined = 0,
    Unfavorable,
    Standard,
    Favorable,
}
