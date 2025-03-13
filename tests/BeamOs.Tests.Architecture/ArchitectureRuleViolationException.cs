namespace BeamOs.Tests.Architecture;

public class ArchitectureRuleViolationException : Exception
{
    public ArchitectureRuleViolationException(string message)
        : base(message)
    {
    }
}
