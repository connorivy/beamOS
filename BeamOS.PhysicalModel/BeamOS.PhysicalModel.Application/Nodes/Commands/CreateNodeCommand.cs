using BeamOS.PhysicalModel.Domain.AnalyticalNodeAggregate;
using FastEndpoints;
using UnitsNet.Units;

namespace BeamOS.PhysicalModel.Application.Nodes.Commands;

public record CreateNodeCommand(
    GuidBasedIdDto ModelId,
    double XCoordinate,
    double YCoordinate,
    double ZCoordinate,
    LengthUnit LengthUnit,
    RestraintsCommand? Restraint = null) : ICommand<AnalyticalNode>;

public record GuidBasedIdDto(Guid ModelId)
{
    public GuidBasedIdDto(string modelId) : this(Guid.Parse(modelId))
    {

    }
};
public record RestraintsCommand(
    bool CanTranslateAlongX,
    bool CanTranslateAlongY,
    bool CanTranslateAlongZ,
    bool CanRotateAboutX,
    bool CanRotateAboutY,
    bool CanRotateAboutZ);
