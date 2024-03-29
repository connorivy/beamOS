using BeamOs.Api.Common.Interfaces;

namespace BeamOs.Api.Common;

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
