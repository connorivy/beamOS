using BeamOs.Common.Application.Interfaces;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOS.Tests.Common.Fixtures.Mappers;

public interface IFixtureMapper<TFrom, TTo> : IMapper<TFrom, TTo>, IFixtureMapper<TFrom>
    where TFrom : FixtureBase2
    where TTo : BeamOsEntityContractBase { }

public interface IFixtureMapper<TFrom> : IFixtureMapper
{
    BeamOsEntityContractBase Map(TFrom source);
}

public interface IFixtureMapper
{
    BeamOsEntityContractBase Map(FixtureBase2 source);
}
