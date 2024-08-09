using BeamOs.Api.Common.Mappers;
using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.Common;
using BeamOs.Contracts.PhysicalModel.Node;

namespace BeamOs.WebApp.Client.Components.Extensions;

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
        element.MapToLength().MapToContract(UnitsNet.Units.LengthUnit.Meter);
}
