using BeamOs.CodeGen.Apis.EditorApi;
using BeamOS.Tests.Common.Interfaces;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;

public partial class DsmElement1dFixture(Element1dFixture fixture)
    : FixtureBase,
        ITestFixtureDisplayable
{
    public Element1dFixture Fixture { get; } = fixture;
    public override Guid Id => this.Fixture.Id;

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
    public SourceInfo SourceInfo => this.Fixture.SourceInfo;

    public Task Display(IEditorApiAlpha editorApiAlpha) => this.Fixture.Display(editorApiAlpha);
}
