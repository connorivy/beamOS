using System.Buffers;
using BeamOs.StructuralAnalysis.Domain.Common.Extensions;
using CommunityToolkit.HighPerformance.Buffers;
using CSparse.Double;
using CSparse.Storage;
using MathNet.Numerics.LinearAlgebra;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams;

internal sealed class DeflectedShapeShapeFunctionCalculator
{
    //private static double N1(double x, double l) =>
    //    1 - 3 * Math.Pow(x / l, 2) + 2 * Math.Pow(x / l, 3);

    //private static double N2(double x, double l) => x * Math.Pow(1 - x / l, 2);

    //private static double N3(double x, double l) => 3 * Math.Pow(x / l, 2) - 2 * Math.Pow(x / l, 3);

    //private static double N4(double x, double l) => -Math.Pow(x, 2) / l * (1 - x / l);

    //private static double[,] Nx(double x, double l) =>
    //    new double[6, 12]
    //    {
    //        { N1(x, l), 0, 0, 0, 0, 0, N2(x, l), 0, 0, 0, 0, 0 },
    //        { 0, N1(x, l), 0, 0, 0, N2(x, l), 0, N3(x, l), 0, 0, 0, N4(x, l) },
    //        { 0, 0, N1(x, l), 0, N2(x, l), 0, 0, 0, N3(x, l), 0, N4(x, l), 0 },
    //        { 0, 0, 0, N1(x, l), 0, 0, 0, 0, 0, N2(x, l), 0, 0 },
    //        { 0, 0, N1(x, l), 0, N2(x, l), 0, 0, 0, N3(x, l), 0, N4(x, l), 0 },
    //        { 0, N1(x, l), 0, 0, 0, N2(x, l), 0, N3(x, l), 0, 0, 0, N4(x, l) },
    //    };
    private static double N1(double x, double l) => 1 - x / l;

    private static double N2(double x, double l) => x / l;

    private static double N3(double x, double l) =>
        1 - 3 * Math.Pow(x / l, 2) + 2 * Math.Pow(x / l, 3);

    private static double N4(double x, double l) => x * Math.Pow(1 - x / l, 2);

    private static double N5(double x, double l) => 3 * Math.Pow(x / l, 2) - 2 * Math.Pow(x / l, 3);

    private static double N6(double x, double l) => -Math.Pow(x, 2) / l * (1 - x / l);

    private static double[,] Nx(double x, double l) =>
        new double[3, 12]
        {
            { N1(x, l), 0, 0, 0, 0, 0, N2(x, l), 0, 0, 0, 0, 0 },
            { 0, N3(x, l), 0, 0, 0, N4(x, l), 0, N5(x, l), 0, 0, 0, N6(x, l) },
            { 0, 0, N3(x, l), 0, -N4(x, l), 0, 0, 0, N5(x, l), 0, -N6(x, l), 0 },
        };

    /// <summary>
    /// { N1(x,l), 0,       0,       0, 0,        0,       N2(x,l), 0,       0,       0, 0,        0        },
    /// { 0,       N3(x,l), 0,       0, 0,        N4(x,l), 0,       N5(x,l), 0,       0, 0,        N6(x, l) },
    /// { 0,       0,       N3(x,l), 0, -N4(x,l), 0,       0,       0,       N5(x,l), 0, -N6(x,l), 0        },
    /// </summary>
    /// <param name="x"></param>
    /// <param name="l"></param>
    /// <returns></returns>
    private static void NxColumnMajor(double x, double l, Span<double> destinationSpan)
    {
        destinationSpan.Slice(0, 3).Fill(N1(x, l), 0, 0);
        destinationSpan.Slice(3, 3).Fill(0, N3(x, l), 0);
        destinationSpan.Slice(6, 3).Fill(0, 0, N3(x, l));
        destinationSpan.Slice(9, 3).Fill(0, 0, 0);
        destinationSpan.Slice(12, 3).Fill(0, 0, -N4(x, l));
        destinationSpan.Slice(15, 3).Fill(0, N4(x, l), 0);
        destinationSpan.Slice(18, 3).Fill(N2(x, l), 0, 0);
        destinationSpan.Slice(21, 3).Fill(0, N5(x, l), 0);
        destinationSpan.Slice(24, 3).Fill(0, 0, N5(x, l));
        destinationSpan.Slice(27, 3).Fill(0, 0, 0);
        destinationSpan.Slice(30, 3).Fill(0, 0, -N6(x, l));
        destinationSpan.Slice(33, 3).Fill(0, N6(x, l), 0);
    }

    public static double[] Solve(
        double x,
        double l,
        Vector<double> displacementVector,
        Matrix<double> elementRotationMatrixTranspose
    )
    {
        Matrix<double> N = Matrix<double>.Build.DenseOfArray(Nx(x, l));

        return (elementRotationMatrixTranspose * N.Multiply(displacementVector)).AsArray();
    }

    public static void Solve(
        double x,
        double l,
        Span<double> displacementVector,
        DenseColumnMajorStorage<double> elementRotationMatrixTranspose,
        Span<double> destinationSpan
    )
    {
        if (destinationSpan.Length != 3)
        {
            throw new ArgumentException("The length of the destination span must be equal to 12.");
        }

        double[] shapeMatrixArr = ArrayPool<double>.Shared.Rent(36);
        Span<double> tempBuffer = stackalloc double[3];
        try
        {
            NxColumnMajor(x, l, shapeMatrixArr);
            DenseMatrix shapeMatrix = new(3, 12, shapeMatrixArr);
            shapeMatrix.Multiply(displacementVector, tempBuffer);
            elementRotationMatrixTranspose.Multiply(tempBuffer, destinationSpan);
        }
        finally
        {
            ArrayPool<double>.Shared.Return(shapeMatrixArr);
        }
    }
}
