using BeamOs.StructuralAnalysis.Domain.PhysicalModel.NodeAggregate;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.Nodes;

public interface INodeRepository
{
    void Add(Node entity);
    Task<Node> Update(PatchNodeCommand patchCommand);
    Task<List<Node>> GetAll();
}
