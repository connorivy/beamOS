using BeamOS.PhysicalModel.Contracts.Node;
using Refit;

namespace BeamOS.PhysicalModel.Client.Interfaces;

public interface IPhysicalModelAlphaClient
{
    [Post("/api/nodes")]
    Task<NodeResponse> CreateNode(CreateNodeRequest request);
}
