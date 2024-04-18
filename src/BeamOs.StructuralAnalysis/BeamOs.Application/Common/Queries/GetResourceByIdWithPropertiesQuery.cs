namespace BeamOs.Application.Common.Queries;

public record GetResourceByIdWithPropertiesQuery(Guid Id, HashSet<string>? Properties);
