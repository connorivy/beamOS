namespace BeamOs.Common.Identity;

public interface IAccountService
{
    public Task<AuthenticationResponse> LoginWithCredentials(string email, string password);
    public Task<AuthenticationResponse> LoginWithProvider(string authProvider);
    public Task Logout();
}
