export const LengthUnit = {
  Undefined: 0,
  Centimeter: 1,
  Foot: 2,
  Inch: 3,
  Meter: 4,
  Millimeter: 5,
}

export const AreaUnit = {
  Undefined: 0,
  SquareCentimeter: 1,
  SquareFoot: 2,
  SquareInch: 3,
  SquareMeter: 4,
  SquareMillimeter: 5,
}

export const VolumeUnit = {
  Undefined: 0,
  CubicCentimeter: 1,
  CubicFoot: 2,
  CubicInch: 3,
  CubicMeter: 4,
  CubicMillimeter: 5,
}

export const AreaMomentOfInertiaUnit = {
  Undefined: 0,
  CentimeterToTheFourth: 1,
  FootToTheFourth: 2,
  InchToTheFourth: 3,
  MeterToTheFourth: 4,
  MillimeterToTheFourth: 5,
}

export const ForceUnit = {
  Undefined: 0,
  Kilonewton: 1,
  KilopoundForce: 2,
  Newton: 3,
  PoundForce: 4,
}

export const AngleUnit = {
  Undefined: 0,
  Degree: 1,
  Radian: 2,
}

export const TorqueUnit = {
  Undefined: 0,
  KilonewtonCentimeter: 7,
  KilonewtonMeter: 8,
  KilonewtonMillimeter: 9,
  KilopoundForceFoot: 10,
  KilopoundForceInch: 11,
  NewtonCentimeter: 17,
  NewtonMeter: 18,
  NewtonMillimeter: 19,
  PoundForceFoot: 21,
  PoundForceInch: 22,
}

export const ForcePerLengthUnit = {
  Undefined: 0,
  KilonewtonPerCentimeter: 7,
  KilonewtonPerMeter: 8,
  KilonewtonPerMillimeter: 9,
  KilopoundForcePerFoot: 10,
  KilopoundForcePerInch: 11,
  NewtonPerCentimeter: 17,
  NewtonPerMeter: 18,
  NewtonPerMillimeter: 19,
  PoundForcePerFoot: 21,
  PoundForcePerInch: 22,
}

export const PressureUnit = {
  Undefined: 0,
  KilonewtonPerSquareCentimeter: 7,
  KilonewtonPerSquareMeter: 8,
  KilonewtonPerSquareMillimeter: 9,
  KilopoundForcePerSquareFoot: 10,
  KilopoundForcePerSquareInch: 11,
  NewtonPerSquareCentimeter: 17,
  NewtonPerSquareMeter: 18,
  NewtonPerSquareMillimeter: 19,
  PoundForcePerSquareFoot: 21,
  PoundForcePerSquareInch: 22,
}

export const RatioUnit = {
  Undefined: 0,
  DecimalFraction: 1,
  Percent: 2,
}

export function getUnitName(
  unitObj: Record<string, number>,
  value: number,
): string | undefined {
  return Object.keys(unitObj).find(key => unitObj[key] === value)
}

export function getAreaUnit(lengthUnit: number): number {
  const mapping: Record<number, number> = {
    [LengthUnit.Centimeter]: AreaUnit.SquareCentimeter,
    [LengthUnit.Foot]: AreaUnit.SquareFoot,
    [LengthUnit.Inch]: AreaUnit.SquareInch,
    [LengthUnit.Meter]: AreaUnit.SquareMeter,
    [LengthUnit.Millimeter]: AreaUnit.SquareMillimeter,
  }

  const areaUnit = mapping[lengthUnit]
  if (!areaUnit) {
    throw new Error(`Unsupported length unit: ${lengthUnit.toString()}`)
  }
  return areaUnit
}

export function getTorqueUnit(lengthUnit: number, forceUnit: number): number {
  switch (lengthUnit) {
    case LengthUnit.Millimeter:
      switch (forceUnit) {
        case ForceUnit.Kilonewton:
          return TorqueUnit.KilonewtonMillimeter
        case ForceUnit.Newton:
          return TorqueUnit.NewtonMillimeter
        default:
          throw new Error(`Unsupported force unit: ${forceUnit.toString()}`)
      }
    case LengthUnit.Centimeter:
      switch (forceUnit) {
        case ForceUnit.Kilonewton:
          return TorqueUnit.KilonewtonCentimeter
        case ForceUnit.Newton:
          return TorqueUnit.NewtonCentimeter
        default:
          throw new Error(`Unsupported force unit: ${forceUnit.toString()}`)
      }
    case LengthUnit.Meter:
      switch (forceUnit) {
        case ForceUnit.Kilonewton:
          return TorqueUnit.KilonewtonMeter
        case ForceUnit.Newton:
          return TorqueUnit.NewtonMeter
        default:
          throw new Error(`Unsupported force unit: ${forceUnit.toString()}`)
      }
    case LengthUnit.Inch:
      switch (forceUnit) {
        case ForceUnit.KilopoundForce:
          return TorqueUnit.KilopoundForceInch
        case ForceUnit.PoundForce:
          return TorqueUnit.PoundForceInch
        default:
          throw new Error(`Unsupported force unit: ${forceUnit.toString()}`)
      }
    case LengthUnit.Foot:
      switch (forceUnit) {
        case ForceUnit.KilopoundForce:
          return TorqueUnit.KilopoundForceFoot
        case ForceUnit.PoundForce:
          return TorqueUnit.PoundForceFoot
        default:
          throw new Error(`Unsupported force unit: ${forceUnit.toString()}`)
      }
    default:
      throw new Error(`Unsupported length unit: ${lengthUnit.toString()}`)
  }
}

export function getPressureUnit(forceUnit: number, lengthUnit: number): number {
  switch (forceUnit) {
    case ForceUnit.Kilonewton:
      switch (lengthUnit) {
        case LengthUnit.Centimeter:
          return PressureUnit.KilonewtonPerSquareCentimeter
        case LengthUnit.Meter:
          return PressureUnit.KilonewtonPerSquareMeter
        case LengthUnit.Millimeter:
          return PressureUnit.KilonewtonPerSquareMillimeter
        default:
          throw new Error(`Unsupported area unit: ${lengthUnit.toString()}`)
      }
    case ForceUnit.KilopoundForce:
      switch (lengthUnit) {
        case LengthUnit.Foot:
          return PressureUnit.KilopoundForcePerSquareFoot
        case LengthUnit.Inch:
          return PressureUnit.KilopoundForcePerSquareInch
        default:
          throw new Error(`Unsupported area unit: ${lengthUnit.toString()}`)
      }
    case ForceUnit.Newton:
      switch (lengthUnit) {
        case LengthUnit.Centimeter:
          return PressureUnit.NewtonPerSquareCentimeter
        case LengthUnit.Meter:
          return PressureUnit.NewtonPerSquareMeter
        case LengthUnit.Millimeter:
          return PressureUnit.NewtonPerSquareMillimeter
        case LengthUnit.Foot:
          return PressureUnit.PoundForcePerSquareFoot
        case LengthUnit.Inch:
          return PressureUnit.PoundForcePerSquareInch
        default:
          throw new Error(`Unsupported area unit: ${lengthUnit.toString()}`)
      }
    case ForceUnit.PoundForce:
      switch (lengthUnit) {
        case LengthUnit.Foot:
          return PressureUnit.PoundForcePerSquareFoot
        case LengthUnit.Inch:
          return PressureUnit.PoundForcePerSquareInch
        default:
          throw new Error(`Unsupported area unit: ${lengthUnit.toString()}`)
      }
    default:
      throw new Error(`Unsupported force unit: ${forceUnit.toString()}`)
  }
}

export function getForceUnitFromTorqueUnit(torqueUnit: number): number {
  switch (torqueUnit) {
    case TorqueUnit.KilonewtonMillimeter:
    case TorqueUnit.KilonewtonCentimeter:
    case TorqueUnit.KilonewtonMeter:
      return ForceUnit.Kilonewton
    case TorqueUnit.NewtonMillimeter:
    case TorqueUnit.NewtonCentimeter:
    case TorqueUnit.NewtonMeter:
      return ForceUnit.Newton
    case TorqueUnit.KilopoundForceInch:
    case TorqueUnit.KilopoundForceFoot:
      return ForceUnit.PoundForce
    case TorqueUnit.PoundForceInch:
    case TorqueUnit.PoundForceFoot:
      return ForceUnit.PoundForce
    default:
      throw new Error(`Unsupported torque unit: ${torqueUnit.toString()}`)
  }
}

export function getLengthUnitFromTorqueUnit(torqueUnit: number): number {
  switch (torqueUnit) {
    case TorqueUnit.KilonewtonMillimeter:
    case TorqueUnit.NewtonMillimeter:
      return LengthUnit.Millimeter
    case TorqueUnit.KilonewtonCentimeter:
    case TorqueUnit.NewtonCentimeter:
      return LengthUnit.Centimeter
    case TorqueUnit.KilonewtonMeter:
    case TorqueUnit.NewtonMeter:
      return LengthUnit.Meter
    case TorqueUnit.KilopoundForceInch:
    case TorqueUnit.PoundForceInch:
      return LengthUnit.Inch
    case TorqueUnit.PoundForceFoot:
    case TorqueUnit.KilopoundForceFoot:
      return LengthUnit.Foot
    default:
      throw new Error(`Unsupported torque unit: ${torqueUnit.toString()}`)
  }
}

export function getForceUnitFromPressureUnit(pressureUnit: number): number {
  switch (pressureUnit) {
    case PressureUnit.KilonewtonPerSquareMillimeter: 
    case PressureUnit.KilonewtonPerSquareCentimeter: 
    case PressureUnit.KilonewtonPerSquareMeter:
      return ForceUnit.Kilonewton
    case PressureUnit.NewtonPerSquareMillimeter: 
    case PressureUnit.NewtonPerSquareCentimeter: 
    case PressureUnit.NewtonPerSquareMeter:
      return ForceUnit.Newton
    case PressureUnit.KilopoundForcePerSquareInch:
    case PressureUnit.KilopoundForcePerSquareFoot:
      return ForceUnit.PoundForce
    case PressureUnit.PoundForcePerSquareInch:
    case PressureUnit.PoundForcePerSquareFoot:
      return ForceUnit.PoundForce
    default:
      throw new Error(`Unsupported pressure unit: ${pressureUnit.toString()}`)
  }
}

export function getLengthUnitFromPressureUnit(pressureUnit: number): number {
  switch (pressureUnit) {
    case PressureUnit.KilonewtonPerSquareMillimeter: 
    case PressureUnit.NewtonPerSquareMillimeter: 
      return LengthUnit.Millimeter
    case PressureUnit.KilonewtonPerSquareCentimeter: 
    case PressureUnit.NewtonPerSquareCentimeter: 
      return LengthUnit.Centimeter
    case PressureUnit.KilonewtonPerSquareMeter:
    case PressureUnit.NewtonPerSquareMeter:
      return LengthUnit.Meter
    case PressureUnit.KilopoundForcePerSquareInch:
    case PressureUnit.PoundForcePerSquareInch:
      return LengthUnit.Inch
    case PressureUnit.PoundForcePerSquareFoot:
    case PressureUnit.KilopoundForcePerSquareFoot:
      return LengthUnit.Foot
    default:
      throw new Error(`Unsupported pressure unit: ${pressureUnit.toString()}`)
  }
}