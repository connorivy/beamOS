using System.Collections;
using System.Reflection;
using BeamOs.Tests.Common;
using BeamOs.Tests.StructuralAnalysis.Integration;
using BeamOs.Tests.StructuralAnalysis.Unit;

namespace BeamOs.Tests.Runtime.TestRunner;

public class TestInfoCollector
{
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

    private static IEnumerable<Assembly> TestAssemblies()
    {
        yield return typeof(IAssemblyMarkerStructuralIntegrationTests).Assembly;
        yield return typeof(IAssemblyMarkerStructuralUnitTests).Assembly;
    }

    public static IEnumerable<TestInfo> GetTestInfoFromMethod(MethodInfo methodInfo, Type classType)
    {
        if (
            methodInfo.GetCustomAttribute<TestAttribute>() is null
            || methodInfo.GetCustomAttribute<SkipInFrontEndAttribute>() is not null
        )
        {
            yield break;
        }

        IEnumerable<MethodDataSourceAttribute> dataAttributes =
            methodInfo.GetCustomAttributes<MethodDataSourceAttribute>();
        if (dataAttributes.Any())
        {
            foreach (var dataAttribute in dataAttributes)
            {
                MethodInfo? dataMethod = dataAttribute.ClassProvidingDataSource.GetMethod(
                    dataAttribute.MethodNameProvidingDataSource,
                    BindingFlags.Public | BindingFlags.Static
                );

                object? data = dataMethod.Invoke(null, []);

                if (data is not IEnumerable enumerableData)
                {
                    continue;
                }

                foreach (var dataItem in enumerableData)
                {
                    yield return new TestInfo()
                    {
                        TestClassType = classType,
                        TestData = [dataItem],
                        MethodInfo = methodInfo,
                    };
                    //    classType,
                    //    dataItem,
                    //    methodInfo,
                    //    methodInfo
                    //        .GetCustomAttributes<TestBaseAttribute>()
                    //        .Concat(classType.GetCustomAttributes<TestBaseAttribute>())
                    //        .Concat(dataItem[0].GetType().GetCustomAttributes<TestBaseAttribute>())
                    //        .GroupBy(attr => attr.TraitName)
                    //        .ToDictionary(
                    //            g => g.Key,
                    //            g => g.Select(attr => attr.TraitValue).First()
                    //        )
                    //);
                }
            }
        }
        //else
        //{
        //    yield return new TestInfo(
        //        classType,
        //        null,
        //        methodInfo,
        //        methodInfo
        //            .GetCustomAttributes<TestBaseAttribute>()
        //            .Concat(classType.GetCustomAttributes<TestBaseAttribute>())
        //            .GroupBy(attr => attr.TraitName)
        //            .ToDictionary(g => g.Key, g => g.Select(attr => attr.TraitValue).First())
        //    );
        //}
    }
}
