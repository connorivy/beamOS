using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.Components.Components.Editor.Commands;

public record struct LoadModelCommand(string CanvasId, string ModelId) : IClientCommand;
