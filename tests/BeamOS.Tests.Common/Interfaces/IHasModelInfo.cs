namespace BeamOS.Tests.Common.Interfaces;

public interface IHasSourceInfo
{
    public SourceInfo SourceInfo { get; }
}

public enum FixtureSourceType
{
    Undefined = 0,
    Textbook = 1,
}

public record SourceInfo(
    string SourceName,
    FixtureSourceType SourceType,
    string? ModelName,
    string? ElementName = null
);
