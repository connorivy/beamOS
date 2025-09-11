using BeamOs.Common.Contracts;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.LoadCombinations;

namespace BeamOs.StructuralAnalysis.Application.PhysicalModel.LoadCombinations;

internal readonly struct CreateLoadCombinationCommand : IModelResourceRequest<LoadCombinationData>
{
    public Guid ModelId { get; init; }
    public LoadCombinationData Body { get; init; }
    public Dictionary<int, double> LoadCaseFactors => this.Body.LoadCaseFactors;
}
