using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Model;
using BeamOS.Tests.Common.Fixtures;
using BeamOS.Tests.Common.Fixtures.Mappers;
using Riok.Mapperly.Abstractions;

namespace BeamOS.Tests.Common.SolvedProblems.Fixtures.Mappers.ToResponse;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class Element1dFixtureToResponseMapperStatic
{
    public static partial Element1DResponse ToContract(this Element1dFixture2 fixture);
}

public class Element1dFixtureToResponseMapper : IFixtureMapper<Element1dFixture2, Element1DResponse>
{
    public Element1DResponse Map(Element1dFixture2 source) => source.ToContract();

    public BeamOsEntityContractBase Map(FixtureBase2 source) => this.Map((Element1dFixture2)source);

    BeamOsEntityContractBase IFixtureMapper<Element1dFixture2>.Map(Element1dFixture2 source) =>
        this.Map(source);
}

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class ModelFixtureToResponseMapperStatic
{
    public static partial ModelResponse ToContract(this ModelFixture2 fixture);
}

public class ModelFixtureToResponseMapper : IFixtureMapper<ModelFixture2, ModelResponse>
{
    public ModelResponse Map(ModelFixture2 source) => source.ToContract();

    public BeamOsEntityContractBase Map(FixtureBase2 source) => this.Map((ModelFixture2)source);

    BeamOsEntityContractBase IFixtureMapper<ModelFixture2>.Map(ModelFixture2 source) =>
        this.Map(source);
}
