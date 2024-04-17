using UnitsNet;

namespace BeamOs.Infrastructure.Data.Models;

internal sealed class NodeReadModel
{
    public Guid Id { get; private set; }
    public Guid ModelId { get; private set; }

    public Length LocationPoint_XCoordinate { get; private set; }
    public Length LocationPoint_YCoordinate { get; private set; }
    public Length LocationPoint_ZCoordinate { get; private set; }

    public bool Restraint_CanRotateAboutX { get; private set; }
    public bool Restraint_CanRotateAboutY { get; private set; }
    public bool Restraint_CanRotateAboutZ { get; private set; }
    public bool Restraint_CanTranslateAlongX { get; private set; }
    public bool Restraint_CanTranslateAlongY { get; private set; }
    public bool Restraint_CanTranslateAlongZ { get; private set; }
}
