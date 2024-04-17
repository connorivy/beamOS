namespace BeamOs.Application.Common.Interfaces;

public interface IMapper<TFrom, TTo>
{
    TTo Map(TFrom source);
}
