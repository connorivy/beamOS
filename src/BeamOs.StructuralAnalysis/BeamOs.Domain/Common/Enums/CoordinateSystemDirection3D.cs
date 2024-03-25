namespace BeamOs.Domain.Common.Enums;

public enum CoordinateSystemDirection3D
{
    Undefined = 0,
    AlongX = 1,
    AlongY = 2,
    AlongZ = 3,
    AboutX = 4,
    AboutY = 5,
    AboutZ = 6,
}

public static class CoordinateSystemDirection3dExtensions
{
    public static bool IsLinearDirection(this CoordinateSystemDirection3D coord)
    {
        if (
            coord
            is CoordinateSystemDirection3D.AlongX
                or CoordinateSystemDirection3D.AlongY
                or CoordinateSystemDirection3D.AlongZ
        )
        {
            return true;
        }
        return false;
    }
}
