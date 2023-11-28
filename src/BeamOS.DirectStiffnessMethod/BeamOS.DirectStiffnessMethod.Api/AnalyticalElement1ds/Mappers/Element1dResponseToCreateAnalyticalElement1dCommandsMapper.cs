using BeamOS.Common.Api;
using BeamOS.Common.Api.Mappers;
using BeamOS.DirectStiffnessMethod.Application.AnalyticalElement1ds.Commands;
using BeamOS.PhysicalModel.Contracts.Element1D;
using Riok.Mapperly.Abstractions;

namespace BeamOS.DirectStiffnessMethod.Api.AnalyticalElement1ds.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitValueDtoToAngleMapper))]
public partial class Element1dResponseToCreateAnalyticalElement1dCommandMapper : AbstractMapper<Element1DResponse, CreateAnalyticalElement1dCommand>
{
    public override CreateAnalyticalElement1dCommand Map(Element1DResponse source) => this.ToCommand(source);
    public partial CreateAnalyticalElement1dCommand ToCommand(Element1DResponse source);
}
