using BeamOs.CsSdk.Mappers.UnitValueDtoMappers;
using BeamOs.StructuralAnalysis.Contracts.Common;
using UnitsNet;

namespace BeamOs.CodeGen.TestModelBuilderGenerator.Extensions;

internal static class DoubleExtensions
{
    public static double Convert(
        this double value,
        LengthUnitContract fromUnit,
        LengthUnitContract toUnit
    )
    {
        return new Length(value, fromUnit.MapEnumToLengthUnit()).As(toUnit.MapEnumToLengthUnit());
    }
}
