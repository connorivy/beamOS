namespace BeamOs.Common.Api;

public class UriProvider(string scheme) : IUriProvider
{
    public UriBuilder Identity { get; init; } = new(scheme, "localhost:7194");
    public UriBuilder StructuralAnalysis { get; init; } = new(scheme, "localhost:7193");
    public UriBuilder WebApp { get; init; } = new(scheme, "localhost:7111");
    public string BasePath { get; init; } = "localhost";
}

public interface IUriProvider
{
    UriBuilder Identity { get; }
    UriBuilder StructuralAnalysis { get; }
    UriBuilder WebApp { get; }
    string BasePath { get; }
}

//public class UriBuilder(string scheme, string host)
//{
//    public string Build(string? path, IEnumerable<KeyValuePair<string, StringValues>>? queryParams)
//    {
//        return UriHelper.BuildAbsolute(
//            scheme,
//            new HostString(host),
//            path: path != null ? new PathString(path) : default,
//            query: queryParams != null ? QueryString.Create(queryParams) : default
//        );
//    }

//    public Uri Build()

//    public string BasePath => UriHelper.BuildAbsolute(scheme, new HostString(host));

//    public string Host => host;
//}
