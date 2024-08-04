namespace BeamOs.Application.Common.Queries;

public record GetResourceByIdWithPropertiesQuery(Guid Id, string[]? Properties = null);

public record GetModelResourcesByIdsQuery(Guid ModelId, HashSet<Guid>? ResourceIds);
