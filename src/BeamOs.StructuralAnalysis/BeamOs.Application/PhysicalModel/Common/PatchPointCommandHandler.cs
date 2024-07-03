using BeamOs.Api.Common.Mappers;
using BeamOs.Application.Common.Interfaces;
using BeamOs.Domain.Common.ValueObjects;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.Application.PhysicalModel.Common;

public class PatchPointCommandHandler : ICommandHandlerSync<PatchPointCommand, Point>
{
    public Point Execute(PatchPointCommand command)
    {
        LengthUnit lengthUnit = command.PatchRequest.LengthUnit.MapToLengthUnit();

        return new Point(
            command.PatchRequest.XCoordinate.HasValue
                ? new Length(command.PatchRequest.XCoordinate.Value, lengthUnit)
                : command.Point.XCoordinate,
            command.PatchRequest.YCoordinate.HasValue
                ? new Length(command.PatchRequest.YCoordinate.Value, lengthUnit)
                : command.Point.YCoordinate,
            command.PatchRequest.ZCoordinate.HasValue
                ? new Length(command.PatchRequest.ZCoordinate.Value, lengthUnit)
                : command.Point.ZCoordinate
        );
    }
}
