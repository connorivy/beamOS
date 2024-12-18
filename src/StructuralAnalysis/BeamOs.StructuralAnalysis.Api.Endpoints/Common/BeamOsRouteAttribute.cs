using BeamOs.Common.Api;

namespace BeamOs.StructuralAnalysis.Api.Endpoints.Common;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BeamOsRouteAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BeamOsEndpointTypeAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class BeamOsRequiredAuthorizationLevelAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}
