namespace BeamOs.Api.Common.Interfaces;

public interface IMapper<TFrom, TTo>
{
    TTo Map(TFrom source);
}
