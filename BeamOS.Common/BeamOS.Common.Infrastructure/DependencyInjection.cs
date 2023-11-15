using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BeamOS.Common.Infrastructure;
public static class DependencyInjection
{
    public static void AddCommonInfrastructure(this ModelConfigurationBuilder configurationBuilder)
    {
        IEnumerable<Type> valueConverters = typeof(DependencyInjection).Assembly
            .GetTypes()
            .Where(t => t.IsClass
                && t.BaseType is not null
                && t.BaseType.IsGenericType
                && t.BaseType.GetGenericTypeDefinition() == typeof(ValueConverter<,>));

        foreach (var valueConverterType in valueConverters)
        {
            var genericArgs = valueConverterType.BaseType?.GetGenericArguments();
            if (genericArgs?.Length != 2)
            {
                throw new ArgumentException();
            }

            _ = configurationBuilder
                .Properties(genericArgs[0])
                .HaveConversion(valueConverterType);
        }
    }
}
