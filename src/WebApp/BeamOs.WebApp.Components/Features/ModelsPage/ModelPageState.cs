using System.Diagnostics.CodeAnalysis;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Model;
using Fluxor;

namespace BeamOs.WebApp.Components.Features.ModelsPage;

[FeatureState]
public record ModelPageState
{
    [SetsRequiredMembers]
    public ModelPageState()
        : this([]) { }

    [SetsRequiredMembers]
    public ModelPageState(IReadOnlyCollection<ModelInfoResponse> modelResponses)
    {
        this.ModelResponses = modelResponses;
    }

    public required IReadOnlyCollection<ModelInfoResponse> ModelResponses { get; init; }
}
