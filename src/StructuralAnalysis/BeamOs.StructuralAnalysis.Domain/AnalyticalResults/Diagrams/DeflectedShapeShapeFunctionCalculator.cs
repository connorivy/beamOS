using MathNet.Numerics.LinearAlgebra;

namespace BeamOs.StructuralAnalysis.Domain.AnalyticalResults.Diagrams;

public sealed class DeflectedShapeShapeFunctionCalculator
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

    //public static double[] Solve(double x, double l, double[] elementDisplacementVector)
    //{
    //    Vector<double> displacementVector = Vector<double>
    //        .Build
    //        .DenseOfArray(elementDisplacementVector);

    //    return Solve(x, l, displacementVector);
    //}

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
}
