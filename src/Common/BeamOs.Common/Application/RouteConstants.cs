namespace BeamOs.Common.Api;

public class RouteConstants
{
    public const string ModelRoutePrefixWithoutTrailingSlash = "models/{modelId:Guid}";
    public const string ModelRoutePrefixWithTrailingSlash =
        ModelRoutePrefixWithoutTrailingSlash + "/";
    public const string ModelResults = ModelRoutePrefixWithTrailingSlash + "results/";
    public const string LoadCombinations = ModelResults + "load-combinations/";
}
