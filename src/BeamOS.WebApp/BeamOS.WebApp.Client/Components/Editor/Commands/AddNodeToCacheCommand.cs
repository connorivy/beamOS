using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOS.WebApp.Client.Components.Editor.Commands;

public record struct AddNodeToCacheCommand(string ModelId, NodeResponse Node) : IClientCommand;

public record struct AddElement1dToCacheCommand(string ModelId, Element1DResponse Element1d)
    : IClientCommand;
