using BeamOs.Api.Common.Mappers;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Node;

namespace BeamOS.WebApp.Client.Extensions;

public static class StructuralAnalysisContractExtensions
{
    public static NodeResponse InMeters(this NodeResponse element) =>
        element with
        {
            LocationPoint = element.LocationPoint.InMeters()
        };

    public static PointResponse InMeters(this PointResponse element) =>
        element with
        {
            XCoordinate = element.XCoordinate.InMeters(),
            YCoordinate = element.YCoordinate.InMeters(),
            ZCoordinate = element.ZCoordinate.InMeters(),
        };

    public static UnitValueDto InMeters(this UnitValueDto element) =>
        element.MapToLength().ToDto(UnitsNet.Units.LengthUnit.Meter);
}