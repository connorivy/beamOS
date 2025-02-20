namespace BeamOs.StructuralAnalysis.Api.Endpoints;

/// <summary>
/// These attributes are only used in debug mode to generate the correct openapi schema.
/// Adding the Microsoft.AspNetCore.Mvc package in release mode messes with publishing for linux
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Parameter,
    AllowMultiple = false,
    Inherited = true
)]
internal class FromRouteAttribute
#if DEBUG
    : Microsoft.AspNetCore.Mvc.FromRouteAttribute
#else
    : Attribute
#endif
{ }

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Parameter,
    AllowMultiple = false,
    Inherited = true
)]
internal class FromQueryAttribute
#if DEBUG
    : Microsoft.AspNetCore.Mvc.FromQueryAttribute
#else
    : Attribute
#endif
{ }
