namespace BeamOS.PhysicalModel.Contracts.Model;

public record GetElementsInModelRequest(string ModelId, List<string>? ChildIds);
