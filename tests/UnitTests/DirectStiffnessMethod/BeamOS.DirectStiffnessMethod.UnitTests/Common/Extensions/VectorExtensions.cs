using System.Globalization;
using MathNet.Numerics.LinearAlgebra;
using Xunit.Sdk;

namespace BeamOS.DirectStiffnessMethod.Domain.UnitTests.Common.Extensions;
public static class VectorExtensions
{
    private static Vector<double> GetRounded(this Vector<double> vector, int numDigits = 3) =>
     vector.Map(m =>
     {
         m = Math.Round(m, numDigits);
         if (m == double.NegativeZero)
         {
             m = default;
         }

         return m;
     });
    public static void AssertAlmostEqual(this Vector<double> calculatedVector, Vector<double> expectedVector, int numDigits = 3)
    {
        var calculated = calculatedVector.GetRounded(numDigits);
        var expected = expectedVector.GetRounded(numDigits);

        var exception = Record.Exception(() => Assert.Equal(expected, calculated));

        Assert.False(exception is EqualException, VectorUnequalExceptionMessage(calculated, expected));
    }
    public static string VectorUnequalExceptionMessage(Vector<double> calculatedVector, Vector<double> expectedVector)
    {
        var diff = calculatedVector - expectedVector;
        return string.Format(CultureInfo.CurrentCulture, "{1}{0}Expected: {2}{0}Actual:   {3}{0}Diff:    {4}{0}",
          Environment.NewLine,
          "Assert.Equal() Failure",
          expectedVector.ToString(12, 12) ?? "(null)",
          calculatedVector.ToString(12, 12) ?? "(null)",
          diff.ToString(12, 12) ?? "(null)");
    }
}
