using BeamOS.PhysicalModel.Contracts.Element1D;
using BeamOS.PhysicalModel.Contracts.Node;

namespace BeamOS.PhysicalModel.Contracts.Model;
public record HydratedModelResponse(
    string Id,
    string Name,
    string Description,
    ModelSettingsResponse Settings,
    List<NodeResponse> NodeIds,
    List<Element1DResponse> Element1DIds,
    List<string> MaterialIds,
    List<string> SectionProfileIds);
