using BeamOs.Application.Common.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Node;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoad;
using UnitsNet;
using UnitsNet.Units;

namespace BeamOs.WebApp.Components.Features.Editors.ReadOnlyEditor;

internal static class ContractExtensions
{
    public static NodeResponse ToEditorUnits(this NodeResponse element) =>
        element with
        {
            LocationPoint = element.LocationPoint.InMeters()
        };

    public static PointLoadResponse ToEditorUnits(this PointLoadResponse element) =>
        element with
        {
            Force = new(element.Force.MapToForce().Kilonewtons, ForceUnitContract.Kilonewton)
        };

    public static StructuralAnalysis.Contracts.Common.Point InMeters(
        this StructuralAnalysis.Contracts.Common.Point element
    )
    {
        LengthUnit lengthUnit = element.LengthUnit.MapToLengthUnit();
        return new()
        {
            X = new Length(element.X, lengthUnit).Meters,
            Y = new Length(element.Y, lengthUnit).Meters,
            Z = new Length(element.Z, lengthUnit).Meters,
            LengthUnit = StructuralAnalysis.Contracts.Common.LengthUnitContract.Meter
        };
    }
}
