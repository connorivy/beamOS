namespace BeamOs.StructuralAnalysis.Api.Shared.Common;

public enum UserAuthorizationLevel
{
    None = 0,
    Authenticated = 1,
    ModelReviewer = 2,
    ModelContributor = 3,
    ModelOwner = 4
}
