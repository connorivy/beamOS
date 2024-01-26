using System.Collections;
using System.Reflection;

namespace BeamOS.PhysicalModel.Contracts.Common;

public abstract record BeamOsContractBase
{
    public bool EqualByNonIdValues(BeamOsContractBase other)
    {
        if (this == null && other == null)
        {
            return true;
        }
        if (this == null || other == null)
        {
            return false;
        }

        foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
        {
            if (propertyInfo.Name.Contains("Id"))
            {
                continue;
            }

            if (propertyInfo.CanRead)
            {
                object firstValue = propertyInfo.GetValue(this, null);
                object secondValue = propertyInfo.GetValue(other, null);

                if (!ContractComparer.NonIdValuesAreEqual(firstValue, secondValue))
                {
                    return false;
                }
            }
        }
        return true;
    }
}

public static class ContractComparer
{
    public static bool NonIdValuesAreEqual(object? first, object? second)
    {
        if (first == null && second == null)
        {
            return true;
        }
        if (first == null || second == null)
        {
            return false;
        }
        Type firstType = first.GetType();
        if (second.GetType() != firstType)
        {
            return false; // Or throw an exception
        }

        if (
            first is BeamOsContractBase firstContract
            && second is BeamOsContractBase secondContract
        )
        {
            return firstContract.EqualByNonIdValues(secondContract);
        }

        if (first is IList firstList && second is IList secondList)
        {
            if (firstList.Count != secondList.Count)
            {
                return false;
            }
            for (int i = 0; i < firstList.Count; i++)
            {
                if (!NonIdValuesAreEqual(firstList[i], secondList[i]))
                {
                    return false;
                }
            }
            return true;
        }

        return object.Equals(first, second);
    }
}
