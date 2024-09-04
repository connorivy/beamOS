namespace BeamOs.ApiClient.Builders;

public interface IModelFixtureInDb
{
    Task Create(IApiAlphaClient client);
    string RuntimeIdToDbId(FixtureId id);
}

public interface IModelFixture
{
    Guid ModelGuid { get; }
}

public interface IHasFixtureId
{
    FixtureId Id { get; }
}
