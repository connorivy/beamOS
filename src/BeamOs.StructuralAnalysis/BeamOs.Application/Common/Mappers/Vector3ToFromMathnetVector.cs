using BeamOs.Contracts.Common;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace BeamOs.Application.Common.Mappers;

public static class Vector3ToFromMathnetVector
{
    public static Vector<double> MapVector3(this Vector3 vector3)
    {
        return DenseVector.OfArray([vector3.X, vector3.Y, vector3.Z]);
    }

    public static Vector3 MapMathnetVector(this Vector<double> vector)
    {
        return new(vector[0], vector[1], vector[2]);
    }
}
