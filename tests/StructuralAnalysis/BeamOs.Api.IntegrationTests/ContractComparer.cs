using System.Collections;
using System.Reflection;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOs.Api.IntegrationTests;

public static class ContractComparer
{
    public static void AssertContractsEqual(BeamOsContractBase expected, BeamOsContractBase actual)
    {
        RoundAllDoubleValues(expected, 2);
        RoundAllDoubleValues(actual, 2);
        AssertContractsEqualRecursive(expected, actual);
    }

    public static void AssertContractsEqual(IList expected, IList actual)
    {
        RoundAllDoubleValues(expected, 2);
        RoundAllDoubleValues(actual, 2);
        AssertContractValuesAreEqual(expected, actual, 2);
    }

    private static void AssertContractsEqualRecursive(
        BeamOsContractBase expected,
        BeamOsContractBase actual
    )
    {
        if (expected.GetType() != actual.GetType())
        {
            throw new ArgumentException(
                $"Type of expected contract, {expected.GetType()}, does not match type of actual contract, {actual.GetType()}"
            );
        }

        foreach (
            PropertyInfo propertyInfo in expected
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        )
        {
            object? expectedValue = propertyInfo.GetValue(expected, null);
            object? actualValue = propertyInfo.GetValue(actual, null);

            AssertContractValuesAreEqual(expectedValue, actualValue);
        }
    }

    private static readonly HashSet<Type> simpleTypes =
    [
        typeof(string),
        typeof(decimal),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(Guid)
    ];

    private static void RoundAllDoubleValues(object? entity, int numDecimals)
    {
        if (entity == null)
        {
            return;
        }

        //var type = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
        Type entityType = entity.GetType();

        if (entityType.IsPrimitive || entityType.IsEnum || simpleTypes.Contains(entityType))
        {
            return;
        }
        else if (entity is IEnumerable entityEnumerable)
        {
            foreach (object? item in entityEnumerable)
            {
                RoundAllDoubleValues(item, numDecimals);
            }
            return;
        }

        foreach (
            var propInfo in entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
        )
        {
            var prop = propInfo.GetValue(entity, null);

            if (prop is double propDouble)
            {
                propInfo.SetValue(entity, Math.Round(propDouble, numDecimals), null);
            }

            RoundAllDoubleValues(prop, numDecimals);
        }
    }

    private static void AssertContractValuesAreEqual(
        object? expected,
        object? actual,
        int numDecimals = 2
    )
    {
        if (expected == null && actual == null)
        {
            return;
        }
        if (expected == null || actual == null)
        {
            throw new ArgumentNullException(
                $"Value of expected: {expected}, \nValue of actual {actual}"
            );
        }
        if (expected.GetType() != actual.GetType())
        {
            throw new ArgumentException(
                $"Value and type of expected: {expected} {expected.GetType()}, Value and type of actual {actual} {actual.GetType()}"
            );
        }

        if (
            expected is BeamOsContractBase expectedContract
            && actual is BeamOsContractBase actualContract
        )
        {
            AssertContractsEqualRecursive(expectedContract, actualContract);
            return;
        }

        if (expected is IList expectedList && actual is IList actualList)
        {
            AssertListsEqual(expectedList, actualList);
            return;
        }

        if (expected is double expectedDouble && actual is double actualDouble)
        {
            Assert.Equal(expectedDouble, actualDouble, numDecimals);
            return;
        }

        if (!Equals(expected, actual))
        {
            throw new ArgumentException(
                $"expected info: \n{expected.GetType()}\n{expected}\n\nactual Info: \n{actual}"
            );
        }
    }

    private static void AssertListsEqual(IList expectedList, IList actualList)
    {
        if (expectedList.Count != actualList.Count)
        {
            throw new ArgumentException(
                $"expected is a list with count {expectedList.Count} and actual is a list with count {actualList.Count}"
            );
        }
        HashSet<int> expectedSet = [];
        HashSet<int> actualSet = [];
        foreach (var item in expectedList)
        {
            expectedSet.Add(item.GetHashCode());
        }
        foreach (var item in actualList)
        {
            actualSet.Add(item.GetHashCode());
        }
        foreach (int key in expectedSet)
        {
            if (!actualSet.Contains(key))
            {
                throw new Exception(
                    $"expected list contains an item with a hashcode equal to {key}, but the actual list does not."
                );
            }
        }
    }
}
