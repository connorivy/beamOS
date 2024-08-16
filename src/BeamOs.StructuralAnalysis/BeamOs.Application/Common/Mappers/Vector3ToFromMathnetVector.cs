using BeamOs.Contracts.Common;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Spatial.Euclidean;

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

    public static Vector3D MapMathnetSpatialVector(this Vector<double> vector)
    {
        return new(vector[0], vector[1], vector[2]);
    }

    public static Vector3D MapVector3d(this Vector3 vector3)
    {
        return new(vector3.X, vector3.Y, vector3.Z);
    }

    public static Vector3 MapMathnetVector(this Vector3D vector)
    {
        return new(vector.X, vector.Y, vector.Z);
    }

    public static Vector<double> MapMathnetNumericsVector(this Vector3D vector)
    {
        return DenseVector.OfArray([vector.X, vector.Y, vector.Z]);
    }
}
