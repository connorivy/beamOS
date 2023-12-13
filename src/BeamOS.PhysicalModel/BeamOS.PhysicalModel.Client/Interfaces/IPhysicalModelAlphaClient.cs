using BeamOS.PhysicalModel.Contracts.Model;
using BeamOS.PhysicalModel.Contracts.Node;
using Refit;

namespace BeamOS.PhysicalModel.Client.Interfaces;

public interface IPhysicalModelAlphaClient
{
    [Post("/api/nodes")]
    Task<NodeResponse> CreateNode(CreateNodeRequest request);

    [Get("/api/models/{id}?sendEntities=true")]
    Task<ModelResponse> GetModel(string id);
}
