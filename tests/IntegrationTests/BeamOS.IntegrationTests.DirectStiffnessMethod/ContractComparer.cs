using System.Collections;
using System.Reflection;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod;

public static class ContractComparer
{
    public static void AssertContractsEqual(BeamOsContractBase first, BeamOsContractBase second)
    {
        RoundAllDoubleValues(first, 2);
        RoundAllDoubleValues(second, 2);
        AssertContractsEqualRecursive(first, second);
    }

    private static void AssertContractsEqualRecursive(
        BeamOsContractBase first,
        BeamOsContractBase second
    )
    {
        foreach (
            PropertyInfo propertyInfo in first
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        )
        {
            object? firstValue = propertyInfo.GetValue(first, null);
            object? secondValue = propertyInfo.GetValue(second, null);

            AssertContractValuesAreEqual(firstValue, secondValue);
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

    private static void AssertContractValuesAreEqual(object? first, object? second)
    {
        if (first == null && second == null)
        {
            return;
        }
        if (first == null || second == null)
        {
            throw new ArgumentNullException($"Value of first: {first}, \nValue of second {second}");
        }
        if (first.GetType() != second.GetType())
        {
            throw new ArgumentException(
                $"Value and type of first: {first} {first.GetType()}, Value and type of second {second} {second.GetType()}"
            );
        }

        if (
            first is BeamOsContractBase firstContract
            && second is BeamOsContractBase secondContract
        )
        {
            AssertContractsEqualRecursive(firstContract, secondContract);
            return;
        }

        if (first is IList firstList && second is IList secondList)
        {
            AssertListsEqual(firstList, secondList);
            return;
        }

        if (!Equals(first, second))
        {
            throw new ArgumentException(
                $"First info: \n{first.GetType()}\n{first}\n\nSecond Info: \n{second}"
            );
        }
    }

    private static void AssertListsEqual(IList firstList, IList secondList)
    {
        if (firstList.Count != secondList.Count)
        {
            throw new ArgumentException(
                $"First is a list with count {firstList.Count} and second is a list with count {secondList.Count}"
            );
        }
        HashSet<int> firstSet = [];
        HashSet<int> secondSet = [];
        foreach (var item in firstList)
        {
            firstSet.Add(item.GetHashCode());
        }
        foreach (var item in secondList)
        {
            secondSet.Add(item.GetHashCode());
        }
        foreach (int key in firstSet)
        {
            if (!secondSet.Contains(key))
            {
                throw new Exception(
                    $"First list contains an item with a hashcode equal to {key}, but the second list does not."
                );
            }
        }
    }
}
