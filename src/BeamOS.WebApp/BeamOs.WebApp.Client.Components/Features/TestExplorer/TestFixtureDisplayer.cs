using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Fixtures.Mappers;
using BeamOs.WebApp.Client.Components.Components.Editor.CommandHandlers;

namespace BeamOs.WebApp.Client.Components.Features.TestExplorer;

public class TestFixtureDisplayer
{
    private readonly Dictionary<Type, IFixtureMapper> fixtureTypeToFixtureMapperDict = [];
    private readonly AddEntityContractToEditorCommandHandler addEntityContractToEditorCommandHandler;

    public TestFixtureDisplayer(
        IEnumerable<IFixtureMapper> fixtureMappers,
        AddEntityContractToEditorCommandHandler addEntityContractToEditorCommandHandler
    )
    {
        this.addEntityContractToEditorCommandHandler = addEntityContractToEditorCommandHandler;

        foreach (IFixtureMapper fixture in fixtureMappers)
        {
            if (
                DependencyInjection.GetInterfaceType(fixture.GetType(), typeof(IFixtureMapper<>))
                is not Type fixtureMapper
            )
            {
                throw new Exception(
                    "Fixture mapper instance does not also implement generic fixture mapper interface"
                );
            }

            this.fixtureTypeToFixtureMapperDict[fixtureMapper.GetGenericArguments()[0]] = fixture;
        }
    }

    public async Task Display(FixtureBase2 fixture, string canvasId)
    {
        if (fixture is Element1dFixture2 element1DFixture)
        {
            // todo: this is a hack. make proper, individual, test fixture displayers
            fixture = element1DFixture.Model.Value;
        }

        var fixtureToContractMapper = this.GetFixtureMapperForType(fixture.GetType());

        var contract = fixtureToContractMapper.Map(fixture);

        await this.addEntityContractToEditorCommandHandler.ExecuteAsync(new(canvasId, contract));
    }

    private IFixtureMapper GetFixtureMapperForType(Type fixtureType)
    {
        if (this.fixtureTypeToFixtureMapperDict.TryGetValue(fixtureType, out var fixtureMapper))
        {
            return fixtureMapper;
        }

        if (fixtureType == typeof(object))
        {
            throw new Exception("Unable to find mapper for fixture");
        }

        return GetFixtureMapperForType(fixtureType.BaseType);
    }
}
