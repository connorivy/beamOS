using BeamOs.Application.Common.Commands;
using BeamOs.Domain.Common.ValueObjects;
using Riok.Mapperly.Abstractions;

namespace BeamOs.Application.PhysicalModel.Models.Mappers;

[Mapper]
public static partial class UnitSettingsCommandToDomainObject
{
    public static partial UnitSettings ToDomainObject(this UnitSettingsCommand command);
}
