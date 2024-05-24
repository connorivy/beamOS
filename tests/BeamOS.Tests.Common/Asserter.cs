using Xunit;

namespace BeamOS.Tests.Common;

public static class Asserter
{
    public static event EventHandler<ComparedObjectEventArgs<double>>? DoublesAssertedEqual;
    public static event EventHandler<ComparedObjectEventArgs<double[]>>? DoubleArrayAssertedEqual;
    public static event EventHandler<
        ComparedObjectEventArgs<double[,]>
    >? Double2dArrayAssertedEqual;

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
