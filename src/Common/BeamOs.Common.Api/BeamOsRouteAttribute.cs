namespace BeamOs.Common.Api;

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
