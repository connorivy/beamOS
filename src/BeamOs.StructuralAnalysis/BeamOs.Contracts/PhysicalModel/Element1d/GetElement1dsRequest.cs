namespace BeamOs.Contracts.PhysicalModel.Element1d;

public record GetElement1dsRequest(string ModelId, string[]? Element1dIds = null) { }
