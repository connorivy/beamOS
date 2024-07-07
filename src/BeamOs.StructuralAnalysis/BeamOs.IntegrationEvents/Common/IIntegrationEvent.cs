using System.Collections;
using System.Reflection;

namespace BeamOs.IntegrationEvents.Common;

public interface IIntegrationEvent
{
    private static readonly HashSet<Type> simpleTypes =
    [
        typeof(string),
        typeof(decimal),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(Guid)
    ];

    public bool AlmostEquals(IIntegrationEvent other, int numDigits = 8)
    {
        return AlmostEquals(this, other, numDigits);
    }

    private static bool AlmostEquals(object? first, object? second, int numDigits)
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
            return false;
        }

        if (first is double firstDouble && second is double secondDouble)
        {
            return Math.Round(firstDouble, numDigits) == Math.Round(secondDouble, numDigits);
        }

        if (first is float firstFloat && second is float secondFloat)
        {
            return Math.Round(firstFloat, numDigits) == Math.Round(secondFloat, numDigits);
        }

        if (firstType.IsEnum || simpleTypes.Contains(firstType))
        {
            return first.Equals(second);
        }

        var publicProps = firstType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        if (publicProps.Length != 0)
        {
            foreach (var propInfo in publicProps)
            {
                var firstProp = propInfo.GetValue(first, null);
                var secondProp = propInfo.GetValue(second, null);

                if (!AlmostEquals(firstProp, secondProp, numDigits))
                {
                    return false;
                }
            }

            return true;
        }

        if (first is IList firstList && second is IList secondList)
        {
            if (firstList.Count != secondList.Count)
            {
                return false;
            }
            for (var i = 0; i < firstList.Count; i++)
            {
                if (!AlmostEquals(firstList[i], secondList[i], numDigits))
                {
                    return false;
                }
            }
            return true;
        }

        return Equals(first, second);
    }
}
