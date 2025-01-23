using BeamOs.Common.Contracts;

namespace BeamOs.StructuralAnalysis.Contracts.Common;

public record ModelEntityResponse(int Id, Guid ModelId) : IModelEntity;
