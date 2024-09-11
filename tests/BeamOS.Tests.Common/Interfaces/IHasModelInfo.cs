namespace BeamOS.Tests.Common.Interfaces;

public interface IHasSourceInfo
{
    public SourceInfo SourceInfo { get; }
}

public enum FixtureSourceType
{
    Undefined = 0,
    ExampleProblem = 1,
    ExampleProblemElement = 2,
    Textbook = 3,
    SAP2000 = 4,
    Standalone = 5
}

public record SourceInfo(
    string SourceName,
    FixtureSourceType SourceType,
    string? ModelName,
    string? ElementName = null,
    string? SourceLink = null
);
