using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.CsSdk.Mappers;
using BeamOs.StructuralAnalysis.Sdk;
using UnitsNet;

namespace BeamOs.Tests.Common;

public abstract class ModelFixture : BeamOsModelBuilder, IModelFixture
{
    public abstract SourceInfo SourceInfo { get; }

    public virtual ModelResponse MapToResponse()
    {
        BeamOsModelBuilderResponseMapper responseMapper = new(this.Id);
        return responseMapper.ToReponse(this);
    }

    public virtual ValueTask InitializeAsync() => ValueTask.CompletedTask;
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

public interface IHasDsmElement1dResults
{
    public DsmElement1dResultFixture[] ExpectedDsmElement1dResults { get; }
}

public interface IHasExpectedDisplacementVector
{
    public double[] ExpectedDisplacementVector { get; }
}

public interface IHasExpectedReactionVector
{
    public double[] ExpectedReactionVector { get; }
}

public interface IHasExpectedDiagramResults
{
    public DiagramResultFixture[] ExpectedDiagramResults { get; }
}

public interface IHasStructuralStiffnessMatrix
{
    public double[,] ExpectedStructuralStiffnessMatrix { get; }
}

public record NodeResultFixture
{
    public required int NodeId { get; init; }
    public required int ResultSetId { get; init; }

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

public record DsmElement1dResultFixture
{
    public required int ElementId { get; init; }
    public required int ResultSetId { get; init; }
    public double[,]? ExpectedRotationMatrix { get; init; }
    public double[,]? ExpectedTransformationMatrix { get; init; }
    public double[,]? ExpectedLocalStiffnessMatrix { get; init; }
    public double[,]? ExpectedGlobalStiffnessMatrix { get; init; }
    public double[]? ExpectedLocalFixedEndForces { get; init; }
    public double[]? ExpectedGlobalFixedEndForces { get; init; }
    public double[]? ExpectedLocalEndDisplacements { get; init; }
    public double[]? ExpectedGlobalEndDisplacements { get; init; }
    public double[]? ExpectedLocalEndForces { get; init; }
    public double[]? ExpectedGlobalEndForces { get; init; }
}

public record DiagramResultFixture
{
    public required int NodeId { get; init; }
    public required int ResultSetId { get; init; }

    public Force? MinShear { get; init; }
    public Force? MaxShear { get; init; }
    public Torque? MinMoment { get; init; }
    public Torque? MaxMoment { get; init; }
    public Length? MaxDeflection { get; init; }
    public Length? MinDeflection { get; init; }
}
