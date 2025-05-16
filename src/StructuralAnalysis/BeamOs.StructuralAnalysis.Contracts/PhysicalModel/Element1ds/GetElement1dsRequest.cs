namespace BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Element1ds;

public record GetElement1dsRequest(string ModelId, string[]? Element1dIds = null) { }
