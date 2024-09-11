using Xunit;

namespace BeamOS.Tests.Common;

public static class Asserter
{
    public static event EventHandler<ComparedObjectEventArgs<double>>? DoublesAssertedEqual;
    public static event EventHandler<ComparedObjectEventArgs2>? AssertedEqual2;
    public static event EventHandler<ComparedObjectEventArgs<double[]>>? DoubleArrayAssertedEqual;
    public static event EventHandler<
        ComparedObjectEventArgs<double[,]>
    >? Double2dArrayAssertedEqual;

    public static void AssertEqual(
        string beamOsObjectId,
        string comparedValueName,
        double expected,
        double actual,
        int numDigits = 3,
        ICollection<string>? comparedValueNameCollection = null
    )
    {
        AssertedEqual2?.Invoke(
            typeof(Asserter),
            new()
            {
                BeamOsObjectId = beamOsObjectId,
                ComparedObjectPropertyName = comparedValueName,
                ExpectedValue = expected,
                CalculatedValue = actual,
                ComparedValueNameCollection = comparedValueNameCollection
            }
        );

        Assert.Equal(expected, actual, numDigits);
    }

    public static void AssertEqual(
        string beamOsObjectId,
        string comparedValueName,
        double[] expected,
        double[] actual,
        int numDigits = 3,
        ICollection<string>? comparedValueNameCollection = null
    )
    {
        AssertedEqual2?.Invoke(
            typeof(Asserter),
            new()
            {
                BeamOsObjectId = beamOsObjectId,
                ComparedObjectPropertyName = comparedValueName,
                ExpectedValue = expected,
                CalculatedValue = actual,
                ComparedValueNameCollection = comparedValueNameCollection
            }
        );

        if (actual.Length != expected.Length)
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (actual.Length == 0)
        {
            return;
        }

        for (int col = 0; col < actual.Length; col++)
        {
            Assert.Equal(expected[col], actual[col], numDigits);
        }
    }

    public static void AssertEqual(
        string beamOsObjectId,
        string comparedValueName,
        double?[] expected,
        double?[] actual,
        int numDigits = 3,
        ICollection<string>? comparedValueNameCollection = null
    )
    {
        AssertedEqual2?.Invoke(
            typeof(Asserter),
            new()
            {
                BeamOsObjectId = beamOsObjectId,
                ComparedObjectPropertyName = comparedValueName,
                ExpectedValue = expected,
                CalculatedValue = actual,
                ComparedValueNameCollection = comparedValueNameCollection
            }
        );

        if (actual.Length != expected.Length)
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (actual.Length == 0)
        {
            return;
        }

        for (int col = 0; col < actual.Length; col++)
        {
            double? actualValue = actual[col];
            double? expectedValue = expected[col];
            if (expectedValue is null)
            {
                continue;
            }
            if (actualValue is null)
            {
                throw new Exception(
                    $"Expected value is {expectedValue} and the actual value is null"
                );
            }
            Assert.Equal(expectedValue.Value, actualValue.Value, numDigits);
        }
    }

    public static void AssertEqual(
        string beamOsObjectId,
        string comparedValueName,
        double[,] expected,
        double[,] actual,
        int numDigits = 3,
        ICollection<string>? comparedValueNameCollection = null
    )
    {
        AssertedEqual2?.Invoke(
            typeof(Asserter),
            new()
            {
                BeamOsObjectId = beamOsObjectId,
                ComparedObjectPropertyName = comparedValueName,
                ExpectedValue = expected,
                CalculatedValue = actual,
                ComparedValueNameCollection = comparedValueNameCollection
            }
        );

        int numRows = actual.GetLength(0);
        int numCols = actual.GetLength(1);
        if (numRows != expected.GetLength(0) || numCols != expected.GetLength(1))
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (numRows == 0 || numCols == 0)
        {
            return;
        }

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                Assert.Equal(expected[row, col], actual[row, col], numDigits);
            }
        }
    }

    public static void AssertEqual(
        string beamOsObjectId,
        string comparedValueName,
        double?[,] expected,
        double?[,] actual,
        int numDigits = 3,
        ICollection<string>? comparedValueNameCollection = null
    )
    {
        AssertedEqual2?.Invoke(
            typeof(Asserter),
            new()
            {
                BeamOsObjectId = beamOsObjectId,
                ComparedObjectPropertyName = comparedValueName,
                ExpectedValue = expected,
                CalculatedValue = actual,
                ComparedValueNameCollection = comparedValueNameCollection
            }
        );

        int numRows = actual.GetLength(0);
        int numCols = actual.GetLength(1);
        if (numRows != expected.GetLength(0) || numCols != expected.GetLength(1))
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (numRows == 0 || numCols == 0)
        {
            return;
        }

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                double? actualValue = actual[row, col];
                double? expectedValue = expected[row, col];
                if (expectedValue is null)
                {
                    continue;
                }
                if (actualValue is null)
                {
                    throw new Exception(
                        $"Expected value is {expectedValue} and the actual value is null"
                    );
                }

                Assert.Equal(expectedValue.Value, actualValue.Value, numDigits);
            }
        }
    }

    public static void AssertEqual(
        string comparedValueName,
        double expected,
        double actual,
        int numDigits
    )
    {
        DoublesAssertedEqual?.Invoke(typeof(Asserter), new(expected, actual, comparedValueName));
        Assert.Equal(expected, actual, numDigits);
    }

    public static void AssertEqual(
        string comparedValueName,
        double[] expected,
        double[] calculated,
        int numDigits = 3
    )
    {
        DoubleArrayAssertedEqual?.Invoke(
            typeof(Asserter),
            new(expected, calculated, comparedValueName)
        );

        if (calculated.Length != expected.Length)
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (calculated.Length == 0)
        {
            return;
        }

        for (int col = 0; col < calculated.Length; col++)
        {
            Assert.Equal(expected[col], calculated[col], numDigits);
        }
    }

    public static void AssertEqual(
        string comparedValueName,
        double[,] expected,
        double[,] calculated,
        int numDigits = 3
    )
    {
        Double2dArrayAssertedEqual?.Invoke(
            typeof(Asserter),
            new(expected, calculated, comparedValueName)
        );

        int numRows = calculated.GetLength(0);
        int numCols = calculated.GetLength(1);
        if (numRows != expected.GetLength(0) || numCols != expected.GetLength(1))
        {
            throw new Exception("Calculated and expected values have different lengths");
        }

        if (numRows == 0 || numCols == 0)
        {
            return;
        }

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                Assert.Equal(expected[row, col], calculated[row, col], numDigits);
            }
        }
    }
}

public class ComparedObjectEventArgs<T>(T expected, T calculated, string comparedObjectName)
    : EventArgs
{
    public T Expected { get; } = expected;
    public T Calculated { get; } = calculated;
    public string ComparedObjectName { get; } = comparedObjectName;
}

public record ComparedObjectEventArgs2
{
    //public required BeamOsObjectType BeamOsObjectType { get; init; }
    public required string BeamOsObjectId { get; init; }
    public required string ComparedObjectPropertyName { get; init; }
    public required object ExpectedValue { get; init; }
    public required object CalculatedValue { get; init; }
    public ICollection<string>? ComparedValueNameCollection { get; init; }
}

public enum BeamOsObjectType
{
    Undefined = 0,
    Model = 1,
    Element1d = 2,
    Node = 3,
}
