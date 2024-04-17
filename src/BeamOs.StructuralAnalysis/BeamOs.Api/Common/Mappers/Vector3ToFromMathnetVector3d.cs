using BeamOs.Contracts.Common;
using MathNet.Spatial.Euclidean;

namespace BeamOs.Api.Common.Mappers;

public static class Vector3ToFromMathnetVector3d
{
    public static Vector3D MapVector3(this Vector3 vector3)
    {
        return new(vector3.X, vector3.Y, vector3.Z);
    }

    public static Vector3 MapMathnetVector(this Vector3D vector)
    {
        return new(vector.X, vector.Y, vector.Z);
    }
}
