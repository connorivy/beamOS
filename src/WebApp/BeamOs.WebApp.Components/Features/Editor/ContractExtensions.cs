using BeamOs.StructuralAnalysis.Contracts.Common;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Models;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.Nodes;
using BeamOs.StructuralAnalysis.Contracts.PhysicalModel.PointLoads;

namespace BeamOs.WebApp.Components.Features.Editor;

internal static class ContractExtensions
{
    public static ModelResponse ToEditorUnits(this ModelResponse element) =>
        element with
        {
            Nodes = element.Nodes?.Select(e => e.ToEditorUnits()).ToList(),
            PointLoads = element.PointLoads?.Select(e => e.ToEditorUnits()).ToList(),
        };

    public static NodeResponse ToEditorUnits(this NodeResponse element) =>
        element with
        {
            LocationPoint = element.LocationPoint.InMeters(),
        };

    public static PointLoadResponse ToEditorUnits(this PointLoadResponse element) =>
        element with
        {
            Force = new(element.Force.ToUnitsNet().Kilonewtons, ForceUnitContract.Kilonewton),
        };

    public static Point InMeters(this Point element)
    {
        LengthUnit lengthUnit = element.LengthUnit.ToUnitsNet();
        return new()
        {
            X = new Length(element.X, lengthUnit).Meters,
            Y = new Length(element.Y, lengthUnit).Meters,
            Z = new Length(element.Z, lengthUnit).Meters,
            LengthUnit = LengthUnitContract.Meter,
        };
    }
}
