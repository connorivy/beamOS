using BeamOs.ApiClient;
using BeamOs.ApiClient.Builders;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.SolvedProblems;
using UnitsNet;

namespace BeamOs.Api.IntegrationTests;

public class UnitTest1 : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly ApiAlphaClient apiClient;
    private readonly Dictionary<string, IModelFixtureInDb> modelIdToModelFixtureDict = [];

    public UnitTest1(CustomWebApplicationFactory<Program> webApplicationFactory)
    {
        var httpClient = webApplicationFactory.CreateClient();
        this.apiClient = new ApiAlphaClient(httpClient);
    }

    public async Task InitializeAsync()
    {
        //AllModelFixtures allSolved = new();
        //foreach (var fixture in allSolved.GetItems())
        //{
        //    var dbModelFixture = new ModelFixtureInDb(fixture);
        //    await dbModelFixture.Create(this.apiClient);
        //    this.modelIdToModelFixtureDict.Add(fixture.Id.ToString(), dbModelFixture);
        //    await this.apiClient.RunDirectStiffnessMethodAsync(fixture.Id.ToString());
        //}

        AllCreateModelRequestBuilders allModelBuilders = new();
        foreach (var modelBuilder in allModelBuilders.GetItems())
        {
            await modelBuilder.InitializeAsync();
            await modelBuilder.Create(this.apiClient);
            this.modelIdToModelFixtureDict.Add(modelBuilder.Id.ToString(), modelBuilder);
            await this.apiClient.RunDirectStiffnessMethodAsync(modelBuilder.Id.ToString());
        }
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    //[SkippableTheory]
    //[ClassData(typeof(AllModelFixtures))]
    //public async Task ModelResponses_WhenRetrievedFromDb_ShouldEqualExpectedValues(
    //    ModelFixture2 modelFixture
    //)
    //{
    //    var dbModelFixture = this.modelIdToModelFixtureDict[modelFixture.Id.ToString()];

    //    var modelResponse = await this.apiClient.GetModelAsync(modelFixture.Id.ToString(), null);
    //    var expectedModelResponse = dbModelFixture.ToResponse();

    //    ContractComparer.AssertContractsEqual(modelResponse, expectedModelResponse);
    //}

    [Theory]
    [ClassData(typeof(AllCreateModelRequestBuildersFilter<IHasExpectedNodeDisplacementResults>))]
    public async Task NodeDisplacementResults_ForSampleProblems_ShouldResultInExpectedValues(
        IHasExpectedNodeDisplacementResults modelFixture
    )
    {
        var dbModelFixture = this.modelIdToModelFixtureDict[modelFixture.Id.ToString()];

        foreach (var expectedNodeDisplacementResult in modelFixture.ExpectedNodeDisplacementResults)
        {
            var dbId = dbModelFixture.RuntimeIdToDbId(expectedNodeDisplacementResult.NodeId);
            var nodeResults = await this.apiClient.GetSingleNodeResultAsync(dbId);

            // for now we don't support multiple results for a node (i.e. load combinations)
            var result = nodeResults.First();

            this.AssertQuantitesEqual(
                expectedNodeDisplacementResult.DisplacementAlongX,
                result.Displacements.DisplacementAlongX.Value,
                modelFixture.Settings.UnitSettings.LengthUnit.MapToLengthUnit(),
                expectedNodeDisplacementResult.LengthTolerance
            );
            this.AssertQuantitesEqual(
                expectedNodeDisplacementResult.DisplacementAlongY,
                result.Displacements.DisplacementAlongY.Value,
                modelFixture.Settings.UnitSettings.LengthUnit.MapToLengthUnit(),
                expectedNodeDisplacementResult.LengthTolerance
            );
            this.AssertQuantitesEqual(
                expectedNodeDisplacementResult.DisplacementAlongZ,
                result.Displacements.DisplacementAlongZ.Value,
                modelFixture.Settings.UnitSettings.LengthUnit.MapToLengthUnit(),
                expectedNodeDisplacementResult.LengthTolerance
            );
            this.AssertQuantitesEqual(
                expectedNodeDisplacementResult.RotationAboutX,
                result.Displacements.RotationAboutX.Value,
                modelFixture.Settings.UnitSettings.AngleUnit.MapToAngleUnit(),
                expectedNodeDisplacementResult.AngleTolerance
            );
            this.AssertQuantitesEqual(
                expectedNodeDisplacementResult.RotationAboutY,
                result.Displacements.RotationAboutY.Value,
                modelFixture.Settings.UnitSettings.AngleUnit.MapToAngleUnit(),
                expectedNodeDisplacementResult.AngleTolerance
            );
            this.AssertQuantitesEqual(
                expectedNodeDisplacementResult.RotationAboutZ,
                result.Displacements.RotationAboutZ.Value,
                modelFixture.Settings.UnitSettings.AngleUnit.MapToAngleUnit(),
                expectedNodeDisplacementResult.AngleTolerance
            );
        }
    }

    private void AssertQuantitesEqual<TUnit, TUnitType>(
        TUnit? expected,
        double calculated,
        TUnitType unitType,
        TUnit tolerance
    )
        where TUnit : struct, IQuantity<TUnitType>
        where TUnitType : Enum
    {
        if (expected is null)
        {
            return;
        }

        Assert.Equal(expected.Value.As(unitType), calculated, tolerance.As(unitType));
    }
}
