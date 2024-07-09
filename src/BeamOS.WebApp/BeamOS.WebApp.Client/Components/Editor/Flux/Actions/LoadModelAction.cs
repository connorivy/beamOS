using BeamOs.Contracts.PhysicalModel.Model;

namespace BeamOS.WebApp.Client.Components.Editor.Flux.Actions;

public record struct LoadModelAction(string ModelId);

public record struct LoadModelActionResult(ModelResponseHydrated ModelResponse);
