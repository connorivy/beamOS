namespace BeamOs.Common.Application.Interfaces;

public interface IMapper<TFrom, TTo>
{
    TTo Map(TFrom source);
}
