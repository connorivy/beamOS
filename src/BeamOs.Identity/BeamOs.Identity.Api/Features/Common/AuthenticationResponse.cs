namespace BeamOs.Identity.Api.Features.Common;

public record AuthenticationResponse(string AccessToken, string RefreshToken);
