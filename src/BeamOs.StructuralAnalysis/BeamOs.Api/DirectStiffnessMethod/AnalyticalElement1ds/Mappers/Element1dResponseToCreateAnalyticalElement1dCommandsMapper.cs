using BeamOs.Api.Common;
using BeamOs.Api.Common.Mappers;
using BeamOs.Application.DirectStiffnessMethod.AnalyticalElement1ds.Commands;
using BeamOs.Contracts.PhysicalModel.Element1d;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Api.DirectStiffnessMethod.AnalyticalElement1ds.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToAngleMapper))]
public partial class Element1dResponseToCreateAnalyticalElement1dCommandMapper
    : AbstractMapper<Element1DResponse, CreateAnalyticalElement1dCommand>
{
    public override CreateAnalyticalElement1dCommand Map(Element1DResponse source) =>
        this.ToCommand(source);

    public partial CreateAnalyticalElement1dCommand ToCommand(Element1DResponse source);
}
