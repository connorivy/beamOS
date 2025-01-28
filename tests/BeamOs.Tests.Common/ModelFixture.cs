using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.CsSdk;
using BeamOs.StructuralAnalysis.CsSdk.Mappers;
using BeamOs.StructuralAnalysis.Domain.AnalyticalResults.ResultSetAggregate;
using UnitsNet;

namespace BeamOs.Tests.Common;

public abstract class ModelFixture : BeamOsModelBuilder, IModelFixture
{
    public abstract SourceInfo SourceInfo { get; }

    public IBeamOsEntityResponse MapToResponse()
    {
        BeamOsModelBuilderResponseMapper responseMapper = new(this.Id);
        return responseMapper.ToReponse(this);
    }
}

public enum FixtureSourceType
{
    Undefined = 0,
    ExampleProblem = 1,
    ExampleProblemElement = 2,
    Textbook = 3,
    SAP2000 = 4,
    Standalone = 5,
    ThirdPartyAnalysisSoftware = 6,
}

public record SourceInfo(
    string SourceName,
    FixtureSourceType SourceType,
    string? ModelName,
    string? ElementName = null,
    string? SourceLink = null
);

public interface IHasExpectedNodeResults
{
    public NodeResultFixture[] ExpectedNodeResults { get; }
}

public record NodeResultFixture
{
    public required int NodeId { get; init; }
    public required ResultSetId ResultSetId { get; init; }

    public Length? DisplacementAlongX { get; init; }
    public Length? DisplacementAlongY { get; init; }
    public Length? DisplacementAlongZ { get; init; }
    public Length LengthTolerance { get; init; } = new(.1, UnitsNet.Units.LengthUnit.Inch);

    public Angle? RotationAboutX { get; init; }
    public Angle? RotationAboutY { get; init; }
    public Angle? RotationAboutZ { get; init; }
    public Angle AngleTolerance { get; init; } = new(1, UnitsNet.Units.AngleUnit.Degree);

    public Force? ForceAlongX { get; init; }
    public Force? ForceAlongY { get; init; }
    public Force? ForceAlongZ { get; init; }
    public Force ForceTolerance { get; init; } = new(.1, UnitsNet.Units.ForceUnit.KilopoundForce);

    public Torque? TorqueAboutX { get; init; }
    public Torque? TorqueAboutY { get; init; }
    public Torque? TorqueAboutZ { get; init; }
    public Torque TorqueTolerance { get; init; } =
        new(5, UnitsNet.Units.TorqueUnit.KilopoundForceInch);
}
