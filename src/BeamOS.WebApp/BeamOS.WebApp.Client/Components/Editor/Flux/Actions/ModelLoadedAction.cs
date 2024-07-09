namespace BeamOS.WebApp.Client.Components.Editor.Flux.Actions;

public record struct ModelLoadedAction(string ModelId);

public record struct ModelUnloadedAction(string ModelId);
