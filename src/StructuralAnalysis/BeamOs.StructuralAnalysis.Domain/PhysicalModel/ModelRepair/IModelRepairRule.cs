namespace BeamOs.StructuralAnalysis.Domain.PhysicalModel.ModelRepair;

public interface IModelRepairRule
{
    public void Apply();

    public ModelRepairRuleType RuleType { get; }
}

public enum ModelRepairRuleType
{
    Undefined = 0,
    Unfavorable,
    Standard,
    Favorable,
}
