using BeamOs.Api.Common.Mappers;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Contracts.Common;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.Common.Mappers;

public abstract class AbstractMapper<TFrom, TTo> : IMapper<TFrom, TTo>
{
    public abstract TTo Map(TFrom source);

    public IEnumerable<TTo> Map(IEnumerable<TFrom> source)
    {
        foreach (var item in source)
        {
            yield return this.Map(item);
        }
    }
}
