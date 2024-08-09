using BeamOs.Contracts.PhysicalModel.Common;
using BeamOs.Contracts.PhysicalModel.Element1d;
using BeamOs.Contracts.PhysicalModel.Node;
using BeamOs.WebApp.Client.Events.Interfaces;

namespace BeamOs.WebApp.Client.Components.Components.Editor.Commands;

public record struct AddNodeToCacheCommand(string ModelId, NodeResponse Node) : IClientCommand;

public record struct AddElement1dToCacheCommand(string ModelId, Element1DResponse Element1d)
    : IClientCommand;

public record struct AddEntityContractToCacheCommand(
    string ModelId,
    BeamOsEntityContractBase Entity
) : IClientCommand;
