import type { DiagramConsistentIntervalResponse } from "../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { convertLength } from "../../../utils/unitConversion"
import { LengthUnit } from "../../../utils/type-extensions/UnitTypeContracts"

export type Length = {
  value: number
  unit: number
}

// Evaluate polynomial at location
function evaluateAtLocation(
  interval: DiagramConsistentIntervalResponse,
  value: number,
): number {
  return interval.polynomialCoefficients.reduce(
    (acc: number, coeff: number, idx: number) =>
      acc + coeff * Math.pow(value, idx),
    0,
  )
}

// TypeScript translation of C# GetValueAtLocation
export function getValueAtLocation(
  intervals: DiagramConsistentIntervalResponse[],
  location: Length,
  equalityTolerance: Length,
): {
  leftValue: number
  rightValue: number
  isBetweenConsistantIntervals: boolean
} {
  for (let i = 0; i < intervals.length; i++) {
    const interval = intervals[i]
    const locationAsMeters = convertLength(
      location.value,
      location.unit,
      LengthUnit.Meter,
    )
    const startLocationAsMeters = convertLength(
      interval.startLocation.value,
      interval.startLocation.unit,
      LengthUnit.Meter,
    )
    const endLocationAsMeters = convertLength(
      interval.endLocation.value,
      interval.endLocation.unit,
      LengthUnit.Meter,
    )
    const equalityToleranceAsMeters = convertLength(
      equalityTolerance.value,
      equalityTolerance.unit,
      LengthUnit.Meter,
    )

    if (locationAsMeters < startLocationAsMeters) {
      continue
    }
    // not needed for ordered intervals
    if (locationAsMeters > endLocationAsMeters) {
      continue
    }
    const left = evaluateAtLocation(interval, locationAsMeters)
    let right: number | null = null
    if (
      i < intervals.length - 1 &&
      Math.abs(locationAsMeters - endLocationAsMeters) <=
        equalityToleranceAsMeters
    ) {
      const rightInterval = intervals[i + 1]
      const rightIntervalEndAsMeters = convertLength(
        rightInterval.endLocation.value,
        rightInterval.endLocation.unit,
        LengthUnit.Meter,
      )
      const rightIntervalStartAsMeters = convertLength(
        rightInterval.startLocation.value,
        rightInterval.startLocation.unit,
        LengthUnit.Meter,
      )
      if (
        Math.abs(rightIntervalStartAsMeters - rightIntervalEndAsMeters) >
        equalityToleranceAsMeters
      )
        right = evaluateAtLocation(rightInterval, locationAsMeters)
    }

    const isBetweenConsistantIntervals = right !== null
    return {
      leftValue: left,
      rightValue: right ?? left,
      isBetweenConsistantIntervals,
    }
  }
  throw new Error("Out of bounds, I guess??")
}
