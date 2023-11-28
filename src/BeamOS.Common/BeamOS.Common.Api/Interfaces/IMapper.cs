namespace BeamOS.Common.Api.Interfaces;
public interface IMapper<TFrom, TTo>
{
    TTo Map(TFrom source);
}
