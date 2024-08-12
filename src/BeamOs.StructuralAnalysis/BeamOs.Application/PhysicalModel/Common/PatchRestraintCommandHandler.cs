using BeamOs.Domain.Common.ValueObjects;

namespace BeamOs.Application.PhysicalModel.Common;

public class PatchRestraintCommandHandler : ICommandHandlerSync<PatchRestraintCommand, Restraint>
{
    public Restraint Execute(PatchRestraintCommand command)
    {
        return new Restraint(
            command.PatchRequest.CanTranslateAlongX ?? command.Restraint.CanTranslateAlongX,
            command.PatchRequest.CanTranslateAlongY ?? command.Restraint.CanTranslateAlongY,
            command.PatchRequest.CanTranslateAlongZ ?? command.Restraint.CanTranslateAlongZ,
            command.PatchRequest.CanRotateAboutX ?? command.Restraint.CanRotateAboutX,
            command.PatchRequest.CanRotateAboutY ?? command.Restraint.CanRotateAboutY,
            command.PatchRequest.CanRotateAboutZ ?? command.Restraint.CanRotateAboutZ
        );
    }
}
