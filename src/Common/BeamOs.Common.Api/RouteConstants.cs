namespace BeamOs.Common.Api;

public class RouteConstants
{
    public const string ModelRoutePrefixWithTrailingSlash = "models/{modelId:Guid}/";
    public const string ModelResults = ModelRoutePrefixWithTrailingSlash + "results/";
    public const string LoadCombinations = ModelResults + "load-combinations/";
}
