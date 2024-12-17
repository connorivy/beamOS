using System.Diagnostics.CodeAnalysis;
using BeamOs.Domain.DirectStiffnessMethod;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToDomain;

namespace BeamOs.Domain.IntegrationTests.DirectStiffnessMethod.Common.Fixtures;

public partial class DsmElement1dFixture : Element1dFixture2
{
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

    public DsmElement1d ToDomain() => new(((Element1dFixture2)this).ToDomain());

    [SetsRequiredMembers]
    public DsmElement1dFixture(Element1dFixture2 element1DFixture)
    {
        this.Id = element1DFixture.Id;
        this.EndNode = element1DFixture.EndNode;
        this.StartNode = element1DFixture.StartNode;
        this.Model = element1DFixture.Model;
        this.SectionProfileRotation = element1DFixture.SectionProfileRotation;
        this.Material = element1DFixture.Material;
        this.SectionProfile = element1DFixture.SectionProfile;
        this.ElementName = element1DFixture.ElementName;
    }
}
