using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOS.WebApp.Client.Components.Editor.Flux.Events;

public record struct AddNodeAction(string CanvasId, NodeResponse Node) : IClientAction;

public record struct AddElement1dAction(string CanvasId, Element1DResponse Element1d)
    : IClientAction;
