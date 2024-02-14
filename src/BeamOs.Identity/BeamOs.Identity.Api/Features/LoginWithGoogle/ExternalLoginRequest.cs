namespace BeamOs.Identity.Api.Features.LoginWithGoogle;

public record ExternalLoginRequest(
    string ReturnUrl,
    string Provider,
    string __RequestVerificationToken
);
