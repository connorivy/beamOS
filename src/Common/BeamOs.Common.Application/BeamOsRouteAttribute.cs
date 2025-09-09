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

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class BeamOsTagAttribute(string value) : Attribute
{
    public string Value { get; } = value;
}

public static class BeamOsTags
{
    public const string AI = nameof(AI);
}
