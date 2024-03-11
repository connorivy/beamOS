namespace BeamOS.WebApp.Client;

public static class Constants
{
    public const string ASSEMBLY_NAME = "AssemblyName";
    public const string PHYSICAL_MODEL_API_BASE_URI = "PhysicalModelApiBaseUriString";
    public const string DSM_API_BASE_URI = "DSMApiBaseUriString";
    public const string ANALYSIS_API_BASE_URI = "AnalysisApiBaseUriString";

    // WARNING : strings must match CommonApiConstants. For some reason the client doesn't build
    // when referencing the BeamOs.Common.Api project
    public const string ACCESS_TOKEN_GUID = "717270ec-bcc4-4f97-ae66-b09ca36771a1";
    public const string REFRESH_TOKEN_GUID = "33fbdb4d-5ebc-48f5-b3c8-e2af398efa24";
}
