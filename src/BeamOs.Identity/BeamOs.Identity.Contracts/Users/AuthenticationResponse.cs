namespace BeamOs.Identity.Contracts.Users;

public record AuthenticationResponse(string AccessToken, string RefreshToken);
