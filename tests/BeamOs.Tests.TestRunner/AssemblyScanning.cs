using System.Reflection;
using BeamOs.Api.IntegrationTests;
using BeamOS.Tests.Common.SolvedProblems.Fixtures;
using BeamOS.Tests.Common.Traits;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BeamOs.Tests.TestRunner;

public static class AssemblyScanning
{
    public static IEnumerable<Assembly> TestAssemblies()
    {
        yield return typeof(IAssemblyMarkerApiIntegrationTests).Assembly;
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
                object? testClassInstance = null;
                foreach (var methodInfo in exportedType.GetMethods())
                {
                    TestInfo[] testInfo = AssemblyScanning
                        .GetTestInfoFromMethod(methodInfo, exportedType)
                        .ToArray();
                    foreach (var test in testInfo)
                    {
                        yield return test;
                    }
                }
            }
        }
    }

    public static IEnumerable<TestInfo> GetTestInfoFromMethod(MethodInfo methodInfo, Type classType)
    {
        DataAttribute? dataAttribute = methodInfo.GetCustomAttribute<DataAttribute>();
        if (dataAttribute is null)
        {
            yield break;
        }

        object[][] data = dataAttribute.GetData(methodInfo).ToArray();

        foreach (var dataItem in data)
        {
            if (dataItem.Length == 0)
            {
                continue;
            }

            if (dataItem.Length > 1)
            {
                continue;
            }

            if (dataItem[0] is FixtureBase fixtureBase)
            {
                yield return new TestInfo(
                    classType,
                    dataItem,
                    methodInfo,
                    methodInfo
                        .GetCustomAttributes<TestBaseAttribute>()
                        .Concat(classType.GetCustomAttributes<TestBaseAttribute>())
                        .Concat(fixtureBase.GetType().GetCustomAttributes<TestBaseAttribute>())
                        .GroupBy(attr => attr.TraitName)
                        .ToDictionary(g => g.Key, g => g.Select(attr => attr.TraitValue).ToArray()),
                    fixtureBase.ModelId.ToGuid(),
                    fixtureBase.Id
                );
            }
        }
    }

    public static IServiceCollection RegisterTestClasses(this IServiceCollection services)
    {
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
                        services.AddScoped(exportedType);
                        break;
                    }
                }
            }
        }
        return services;
    }
}
