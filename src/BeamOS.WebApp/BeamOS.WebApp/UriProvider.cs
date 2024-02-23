namespace BeamOS.WebApp;

public class UriProvider : IUriProvider
{
    public string IdentityServerBaseUri { get; } = "https://localhost:7194";
}

public interface IUriProvider
{
    string IdentityServerBaseUri { get; }
}
