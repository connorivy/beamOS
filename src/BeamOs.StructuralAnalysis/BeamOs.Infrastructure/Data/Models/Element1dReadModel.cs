using UnitsNet;

namespace BeamOs.Infrastructure.Data.Models;

internal class Element1dReadModel
{
    public Guid Id { get; private set; }
    public Guid ModelId { get; private set; }
    public Guid StartNodeId { get; private set; }
    public NodeReadModel StartNode { get; set; }
    public Guid EndNodeId { get; private set; }
    public NodeReadModel EndNode { get; set; }
    public Guid MaterialId { get; private set; }
    public Guid SectionProfileId { get; private set; }

    /// <summary>
    /// counter-clockwise rotation in radians when looking in the negative (local) x direction
    /// </summary>
    public Angle SectionProfileRotation { get; set; }
}
