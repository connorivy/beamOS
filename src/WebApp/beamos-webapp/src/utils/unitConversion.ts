import convert from "convert"
import { LengthUnit } from "./type-extensions/UnitTypeContracts"

// Map from BeamOS LengthUnit enum to convert package unit strings
const lengthUnitToConvertUnit: Record<number, "m" | "cm" | "mm" | "in" | "ft"> = {
  [LengthUnit.Meter]: "m",
  [LengthUnit.Centimeter]: "cm",
  [LengthUnit.Millimeter]: "mm",
  [LengthUnit.Inch]: "in",
  [LengthUnit.Foot]: "ft",
}

/**
 * Convert a length value from one unit to another
 * @param value - The numeric value to convert
 * @param fromUnit - The BeamOS LengthUnit enum value of the source unit
 * @param toUnit - The BeamOS LengthUnit enum value of the target unit
 * @returns The converted value
 */
export function convertLength(
  value: number,
  fromUnit: number,
  toUnit: number,
): number {
  // If units are the same, no conversion needed
  if (fromUnit === toUnit) {
    return value
  }

  // Handle undefined units
  if (fromUnit === LengthUnit.Undefined || toUnit === LengthUnit.Undefined) {
    console.warn(
      `Cannot convert from/to undefined unit. From: ${fromUnit}, To: ${toUnit}`,
    )
    return value
  }

  const fromConvertUnit = lengthUnitToConvertUnit[fromUnit]
  const toConvertUnit = lengthUnitToConvertUnit[toUnit]

  if (!fromConvertUnit || !toConvertUnit) {
    console.error(
      `Unsupported length unit conversion: ${fromUnit} -> ${toUnit}`,
    )
    return value
  }

  return convert(value, fromConvertUnit).to(toConvertUnit)
}

/**
 * Convert a point's coordinates from its unit to a target unit
 * @param point - The point with x, y, z coordinates and a lengthUnit
 * @param targetUnit - The BeamOS LengthUnit enum value of the target unit
 * @returns A new point with converted coordinates in the target unit
 */
export function convertPoint(
  point: { x: number; y: number; z: number; lengthUnit: number },
  targetUnit: number,
): { x: number; y: number; z: number; lengthUnit: number } {
  return {
    x: convertLength(point.x, point.lengthUnit, targetUnit),
    y: convertLength(point.y, point.lengthUnit, targetUnit),
    z: convertLength(point.z, point.lengthUnit, targetUnit),
    lengthUnit: targetUnit,
  }
}
