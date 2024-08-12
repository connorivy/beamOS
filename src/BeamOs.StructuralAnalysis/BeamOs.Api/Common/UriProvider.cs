using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;

namespace BeamOs.Api.Common;

public class UriProvider(string scheme) : IUriProvider
{
    public UriBuilder DirectStiffnessMethod { get; } = new(scheme, "localhost:7110");
    public UriBuilder Identity { get; } = new(scheme, "localhost:7194");
    public UriBuilder PhysicalModel { get; } = new(scheme, "localhost:7193");
    public UriBuilder WebApp { get; } = new(scheme, "localhost:7111");
    public string BasePath { get; } = "localhost";
}

public interface IUriProvider
{
    UriBuilder DirectStiffnessMethod { get; }
    UriBuilder Identity { get; }
    UriBuilder PhysicalModel { get; }
    UriBuilder WebApp { get; }
    string BasePath { get; }
}

public class UriBuilder(string scheme, string host)
{
    public string Build(string? path, IEnumerable<KeyValuePair<string, StringValues>>? queryParams)
    {
        return UriHelper.BuildAbsolute(
            scheme,
            new HostString(host),
            path: path != null ? new PathString(path) : default,
            query: queryParams != null ? QueryString.Create(queryParams) : default
        );
    }

    public string BasePath => UriHelper.BuildAbsolute(scheme, new HostString(host));

    public string Host => host;
}
