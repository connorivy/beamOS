using System.Text;
using ArchUnitNET.Fluent;

namespace BeamOs.Tests.Architecture;

internal static class IArchRuleExtensions
{
    public static void Check(this IArchRule rule, ArchUnitNET.Domain.Architecture architecture)
    {
        var violations = rule.Evaluate(architecture).Where(e => !e.Passed).ToList();
        if (violations.Count > 0)
        {
            var message = new StringBuilder();
            message.AppendLine($"Rule '{rule.Description}' violated:");
            foreach (var violation in violations)
            {
                message.AppendLine($"- {violation.Description}");
            }
            throw new ArchitectureRuleViolationException(message.ToString());
        }
    }
}
