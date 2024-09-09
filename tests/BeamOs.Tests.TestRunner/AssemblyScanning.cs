using System.Reflection;
using BeamOs.Api.IntegrationTests;
using BeamOs.Api.UnitTests;
using BeamOs.ApiClient;
using BeamOS.Tests.Common.Traits;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;
using Xunit.Sdk;

namespace BeamOs.Tests.TestRunner;

public static class AssemblyScanning
{
    public static IEnumerable<Assembly> TestAssemblies()
    {
        yield return typeof(IAssemblyMarkerApiIntegrationTests).Assembly;
        yield return typeof(IAssemblyMarkerDomainIntegrationTests).Assembly;
        yield return typeof(IAssemblyMarkerDomainUnitTests).Assembly;
    }

    public static IEnumerable<Assembly> GetTestingAssemblies()
    {
        return TestAssemblies().Where(AssemblyContainsTests);
    }

    public static bool AssemblyContainsTests(Assembly assembly)
    {
        foreach (
            var exportedType in assembly
                .GetExportedTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract)
        )
        {
            foreach (var methodInfo in exportedType.GetMethods())
            {
                if (methodInfo.GetCustomAttribute<DataAttribute>() is not null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static IEnumerable<TestInfo> GetAllTestInfo()
    {
        foreach (var testAssembly in TestAssemblies())
        {
            foreach (
                var exportedType in testAssembly
                    .GetExportedTypes()
                    .Where(t => !t.IsInterface && !t.IsAbstract)
            )
            {
                foreach (var methodInfo in exportedType.GetMethods())
                {
                    foreach (var test in GetTestInfoFromMethod(methodInfo, exportedType))
                    {
                        yield return test;
                    }
                }
            }
        }
    }

    public static IEnumerable<TestInfo> GetTestInfoFromMethod(MethodInfo methodInfo, Type classType)
    {
        if (methodInfo.GetCustomAttribute<FactAttribute>() is null)
        {
            yield break;
        }

        IEnumerable<DataAttribute> dataAttributes = methodInfo.GetCustomAttributes<DataAttribute>();
        if (dataAttributes.Any())
        {
            foreach (var dataAttribute in dataAttributes)
            {
                object[][] data = dataAttribute.GetData(methodInfo).ToArray();

                foreach (var dataItem in data)
                {
                    if (dataItem.Length == 0)
                    {
                        continue;
                    }

                    yield return new TestInfo(
                        classType,
                        dataItem,
                        methodInfo,
                        methodInfo
                            .GetCustomAttributes<TestBaseAttribute>()
                            .Concat(classType.GetCustomAttributes<TestBaseAttribute>())
                            .Concat(dataItem[0].GetType().GetCustomAttributes<TestBaseAttribute>())
                            .GroupBy(attr => attr.TraitName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(attr => attr.TraitValue).First()
                            )
                    );
                }
            }
        }
        else
        {
            yield return new TestInfo(
                classType,
                null,
                methodInfo,
                methodInfo
                    .GetCustomAttributes<TestBaseAttribute>()
                    .Concat(classType.GetCustomAttributes<TestBaseAttribute>())
                    .GroupBy(attr => attr.TraitName)
                    .ToDictionary(g => g.Key, g => g.Select(attr => attr.TraitValue).First())
            );
        }
    }

    public static IServiceCollection RegisterTestClasses(this IServiceCollection services)
    {
        _ = services.AddScoped<UnitTest1>(
            sp => UnitTest1.Create(sp.GetRequiredService<IApiAlphaClient>())
        );
        foreach (var assembly in TestAssemblies().Where(AssemblyContainsTests))
        {
            foreach (
                var exportedType in assembly
                    .GetExportedTypes()
                    .Where(t => !t.IsInterface && !t.IsAbstract)
            )
            {
                foreach (var methodInfo in exportedType.GetMethods())
                {
                    if (GetTestInfoFromMethod(methodInfo, exportedType).Any())
                    {
                        services.TryAddScoped(exportedType);
                        break;
                    }
                }
            }
        }
        return services;
    }
}
