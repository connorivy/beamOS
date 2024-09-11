namespace BeamOs.Common.Identity;

public interface IAuthStateProvider
{
    public Task Login(string accessToken);

    public Task Logout();

    public string? GetAccessToken();
}
