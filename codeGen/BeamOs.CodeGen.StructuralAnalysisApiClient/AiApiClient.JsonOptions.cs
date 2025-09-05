namespace BeamOs.CodeGen.AiApiClient;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Trimming",
    "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
    Justification = "Okay to supress because we are using the source generated jsonSerializerOptions"
)]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "AOT",
    "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
    Justification = "Okay to supress because we are using the source generated jsonSerializerOptions"
)]
public partial class AiApiClient { }
