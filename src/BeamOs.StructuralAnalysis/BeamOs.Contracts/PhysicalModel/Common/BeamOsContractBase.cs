using System.Collections;

namespace BeamOs.Contracts.PhysicalModel.Common;

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

        var thisType = this.GetType();
        if (other.GetType() != thisType)
        {
            return false;
        }

        foreach (var propertyInfo in thisType.GetProperties())
        {
            if (propertyInfo.Name.EndsWith("Id"))
            {
                continue;
            }

            if (propertyInfo.CanRead)
            {
                var firstValue = propertyInfo.GetValue(this, null);
                var secondValue = propertyInfo.GetValue(other, null);

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
        var firstType = first.GetType();
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
            for (var i = 0; i < firstList.Count; i++)
            {
                if (!NonIdValuesAreEqual(firstList[i], secondList[i]))
                {
                    return false;
                }
            }
            return true;
        }

        return Equals(first, second);
    }
}
