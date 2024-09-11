using System.Diagnostics.CodeAnalysis;
using BeamOs.Contracts.PhysicalModel.Model;
using Fluxor;

namespace BeamOs.WebApp.Client.Components.Features.ModelsPage;

[FeatureState]
public record ModelPageState
{
    [SetsRequiredMembers]
    public ModelPageState()
        : this([]) { }

    [SetsRequiredMembers]
    public ModelPageState(IReadOnlyCollection<ModelResponse> modelResponses)
    {
        this.ModelResponses = modelResponses;
    }

    public required IReadOnlyCollection<ModelResponse> ModelResponses { get; init; }
}
