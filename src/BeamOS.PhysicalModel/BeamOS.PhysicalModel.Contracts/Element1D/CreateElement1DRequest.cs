namespace BeamOS.PhysicalModel.Contracts.Element1D;
public record CreateElement1DRequest(
    string ModelId,
    string StartNodeId,
    string EndNodeId,
    string MaterialId,
    string SectionProfileId);
