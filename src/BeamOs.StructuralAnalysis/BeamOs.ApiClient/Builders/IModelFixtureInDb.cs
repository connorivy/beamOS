namespace BeamOs.ApiClient.Builders;

public interface IModelFixtureInDb
{
    Task Create(ApiAlphaClient client);
    string RuntimeIdToDbId(FixtureId id);
}

public interface IModelFixture
{
    Guid ModelGuid { get; }
}
