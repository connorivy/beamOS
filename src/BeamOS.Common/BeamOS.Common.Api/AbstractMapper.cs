using BeamOS.Common.Api.Interfaces;

namespace BeamOS.Common.Api;

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
