namespace BeamOs.Contracts.Common;

public record IdRequestWithProperties(string Id, string[]? Properties = null);

public record ModelIdRequestWithProperties(string ModelId, string[]? Properties = null);

public record GetNodeResultsRequest(string ModelId, string[]? NodeIds = null);
