import type { ForceSystems, ForceUnits } from "convert-units/definitions/force";
import force from "convert-units/definitions/force"
import type { LengthSystems, LengthUnits } from "convert-units/definitions/length";
import length from "convert-units/definitions/length"
import { ForceUnit, getForceUnitFromPressureUnit, getForceUnitFromTorqueUnit, getLengthUnitFromPressureUnit, getLengthUnitFromTorqueUnit, LengthUnit } from "./type-extensions/UnitTypeContracts"
import configureMeasurements from "convert-units"

// Map from BeamOS LengthUnit enum to convert package unit strings
const lengthUnitToConvertUnit: Record<number, LengthUnits> = {
  [LengthUnit.Meter]: "m",
  [LengthUnit.Centimeter]: "cm",
  [LengthUnit.Millimeter]: "mm",
  [LengthUnit.Inch]: "in",
  [LengthUnit.Foot]: "ft",
}

const forceUnitToConvertUnit: Record<number, ForceUnits> = {
  [ForceUnit.Kilonewton]: "kN",
  [ForceUnit.Newton]: "N",
  [ForceUnit.PoundForce]: "lbf",
}

type Measures = 'length' | 'force'
type Systems = LengthSystems | ForceSystems
type Units = LengthUnits | ForceUnits

const convert = configureMeasurements<Measures, Systems, Units>({
  length,
  force,
});

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
      `Cannot convert from/to undefined unit. From: ${fromUnit.toString()}, To: ${toUnit.toString()}`,
    )
    throw new Error(`Cannot convert from ${fromUnit.toString()} to ${toUnit.toString()}`)
  }

  const fromConvertUnit = fromUnit in lengthUnitToConvertUnit ? lengthUnitToConvertUnit[fromUnit] : undefined
  const toConvertUnit = toUnit in lengthUnitToConvertUnit ? lengthUnitToConvertUnit[toUnit] : undefined

  if (!fromConvertUnit || !toConvertUnit) {
    console.error(
      `Unsupported length unit conversion: ${fromUnit.toString()} -> ${toUnit.toString()}`,
    )
    return value
  }

  return convert(value).from(fromConvertUnit).to(toConvertUnit)
}

export function convertForce(
  value: number,
  fromUnit: number,
  toUnit: number,
): number {
  // If units are the same, no conversion needed
  if (fromUnit === toUnit) {
    return value
  }

  // kips are not supported by convert-units, so convert to lbf first
  let additionalMult = 1
  if (fromUnit === ForceUnit.KilopoundForce)
  {
    fromUnit = ForceUnit.PoundForce
    additionalMult = 1000
  }
  if (toUnit === ForceUnit.KilopoundForce)
  {
    toUnit = ForceUnit.PoundForce
    additionalMult = 1 / 1000
  }

  const fromConvertUnit = fromUnit in forceUnitToConvertUnit ? forceUnitToConvertUnit[fromUnit] : undefined
  const toConvertUnit = toUnit in forceUnitToConvertUnit ? forceUnitToConvertUnit[toUnit] : undefined

  if (!fromConvertUnit || !toConvertUnit) {
    console.error(
      `Unsupported force unit conversion: ${fromUnit.toString()} -> ${toUnit.toString()}`,
    )
    return value
  }

  return convert(value).from(fromConvertUnit).to(toConvertUnit) * additionalMult
}

export function convertTorque(
  value: number,
  fromUnit: number,
  toUnit: number,
): number {
  // If units are the same, no conversion needed
  if (fromUnit === toUnit) {
    return value
  }

  const fromForceUnit = getForceUnitFromTorqueUnit(fromUnit)
  const toForceUnit = getForceUnitFromTorqueUnit(toUnit)

  let convertedValue = convertForce(value, fromForceUnit, toForceUnit)

  const fromLengthUnit = getLengthUnitFromTorqueUnit(fromUnit)
  const toLengthUnit = getLengthUnitFromTorqueUnit(toUnit)

  convertedValue = convertLength(convertedValue, fromLengthUnit, toLengthUnit)

  return convertedValue
}

export function convertPressure(
  value: number,
  fromUnit: number,
  toUnit: number,
): number {
  // If units are the same, no conversion needed
  if (fromUnit === toUnit) {
    return value
  }

  const fromForceUnit = getForceUnitFromPressureUnit(fromUnit)
  const toForceUnit = getForceUnitFromPressureUnit(toUnit)

  let convertedValue = convertForce(value, fromForceUnit, toForceUnit)

  const fromLengthUnit = getLengthUnitFromPressureUnit(fromUnit)
  const toLengthUnit = getLengthUnitFromPressureUnit(toUnit)

  const lengthConversionFactor = convertLength(1, fromLengthUnit, toLengthUnit)
  convertedValue /= lengthConversionFactor * lengthConversionFactor

  return convertedValue
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
