namespace BeamOs.CodeGen.Apis.Generator.Extensions;

public static class TypeExtensions
{
    public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        var interfaceTypes = givenType.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        {
            return true;
        }

        if (givenType.BaseType is not null)
        {
            return givenType.BaseType.IsAssignableToGenericType(genericType);
        }

        return false;
    }

    public static bool TryGetImplementedGenericType(
        this Type givenType,
        Type genericType,
        out Type? implementedType
    )
    {
        implementedType = null;
        var interfaceTypes = givenType.GetInterfaces();

        foreach (var it in interfaceTypes)
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
            {
                implementedType = it;
                return true;
            }
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        {
            implementedType = genericType;
            return true;
        }

        if (givenType.BaseType is not null)
        {
            return givenType.BaseType.IsAssignableToGenericType(genericType);
        }

        return false;
    }
}
