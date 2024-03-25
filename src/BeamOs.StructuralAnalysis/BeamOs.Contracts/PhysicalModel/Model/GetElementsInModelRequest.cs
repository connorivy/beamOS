namespace BeamOs.Contracts.PhysicalModel.Model;

public record GetElementsInModelRequest(string ModelId, List<string>? ChildIds);
