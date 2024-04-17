using UnitsNet;

namespace BeamOs.Infrastructure.Data.Interfaces;

internal interface IFlattenedLocationPoint
{
    public Length LocationPoint_XCoordinate { get; }
    public Length LocationPoint_YCoordinate { get; }
    public Length LocationPoint_ZCoordinate { get; }
}
