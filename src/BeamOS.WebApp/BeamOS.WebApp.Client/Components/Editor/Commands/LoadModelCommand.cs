using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOS.WebApp.Client.Components.Editor.Commands;

public record struct LoadModelCommand(string CanvasId, string ModelId) : IClientAction;
