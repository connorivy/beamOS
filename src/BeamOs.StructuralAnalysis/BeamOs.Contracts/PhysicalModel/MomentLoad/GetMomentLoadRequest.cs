namespace BeamOs.Contracts.PhysicalModel.MomentLoad;

public record GetMomentLoadRequest(string ModelId, string[]? MomentLoadIds = null);
