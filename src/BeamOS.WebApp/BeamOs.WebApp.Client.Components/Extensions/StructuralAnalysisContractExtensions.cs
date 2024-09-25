using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.Contracts.PhysicalModel.Node;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.WebApp.Client.Components.Extensions;

public static class StructuralAnalysisContractExtensions
{
    public static NodeResponse InMeters(this NodeResponse element) =>
        element with
        {
            LocationPoint = element.LocationPoint.InMeters()
        };

    public static Point InMeters(this Point element)
    {
        LengthUnit lengthUnit = element.LengthUnit.MapToLengthUnit();
        return element with
        {
            XCoordinate = new Length(element.XCoordinate, lengthUnit).Meters,
            YCoordinate = new Length(element.YCoordinate, lengthUnit).Meters,
            ZCoordinate = new Length(element.ZCoordinate, lengthUnit).Meters,
            LengthUnit = Contracts.Common.LengthUnitContract.Meter
        };
    }
}
