namespace BeamOS.PhysicalModel.Api.Common.Interfaces;
public interface IMapper<TFrom, TTo>
{
    TTo Map(TFrom from);
}
