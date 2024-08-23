using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.PhysicalModel.Node;
using UnitsNet;

namespace BeamOs.WebApp.Client.Components.Extensions;

public static class StructuralAnalysisContractExtensions
{
    public static NodeResponse InMeters(this NodeResponse element) =>
        element with
        {
            LocationPoint = element.LocationPoint.InMeters()
        };

    public static Point InMeters(this Point element) =>
        element with
        {
            XCoordinate = new Length(
                element.XCoordinate,
                UnitsNetMappers.MapToLengthUnit(element.LengthUnit)
            ).Meters,
            YCoordinate = new Length(
                element.YCoordinate,
                UnitsNetMappers.MapToLengthUnit(element.LengthUnit)
            ).Meters,
            ZCoordinate = new Length(
                element.ZCoordinate,
                UnitsNetMappers.MapToLengthUnit(element.LengthUnit)
            ).Meters,
            LengthUnit = "Meters"
        };
}
