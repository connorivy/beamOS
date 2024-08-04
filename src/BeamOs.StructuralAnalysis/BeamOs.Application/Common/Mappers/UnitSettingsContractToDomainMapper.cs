using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.Common;
using BeamOs.Domain.Common.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.Common.Mappers;

[Mapper]
[UseStaticMapper(typeof(UnitsNetMappers))]
public static partial class UnitSettingsContractToDomainMapper
{
    public static partial UnitSettings ToDomain(this UnitSettingsDtoVerbose from);
}
