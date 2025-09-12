namespace BeamOs.Common.Contracts;

/// <summary>
/// These attributes are only used in debug mode to generate the correct openapi schema.
/// Adding the Microsoft.AspNetCore.Mvc package in release mode messes with publishing for linux
/// </summary>
[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Parameter,
    AllowMultiple = false,
    Inherited = true
)]
public class FromRouteAttribute
#if CODEGEN
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
public class FromQueryAttribute
#if CODEGEN
    : Microsoft.AspNetCore.Mvc.FromQueryAttribute
#else
    : Attribute
#endif
{ }

[AttributeUsage(
    AttributeTargets.Property | AttributeTargets.Parameter,
    AllowMultiple = false,
    Inherited = true
)]
public class FromBodyAttribute
#if CODEGEN
    : Microsoft.AspNetCore.Mvc.FromBodyAttribute
#else
    : Attribute
#endif
{ }
