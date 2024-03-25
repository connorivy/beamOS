using BeamOs.Contracts.Common;

namespace BeamOs.Contracts.PhysicalModel.Model;

public record GetModelHydratedRequest(string ModelId, PreconfiguredUnits? Units = null);
