using System.Collections;
using System.Reflection;
using BeamOs.Contracts.PhysicalModel.Common;

namespace BeamOS.IntegrationTests.DirectStiffnessMethod;

public static class ContractComparer
{
    public static void AssertContractsEqual(BeamOsContractBase first, BeamOsContractBase second)
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
            AssertContractsEqual(firstContract, secondContract);
        }

        if (first is IList firstList && second is IList secondList)
        {
            if (firstList.Count != secondList.Count)
            {
                throw new ArgumentException(
                    $"First is a list with count {firstList.Count} and second is a list with count {secondList.Count}"
                );
            }
            for (int i = 0; i < firstList.Count; i++)
            {
                AssertContractValuesAreEqual(firstList[i], secondList[i]);
            }
            return;
        }

        if (!Equals(first, second))
        {
            throw new ArgumentException(
                $"First info: \n{first.GetType()}\n{first}\n\nSecond Info: \n{second}"
            );
        }
    }
}