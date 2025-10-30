import * as THREE from "three"
import { BeamOsMesh } from "../BeamOsMesh"
// import type { DiagramConsistentIntervalResponse } from "../EditorApi/EditorApiAlpha"
import type { DiagramConsistentIntervalResponse } from "../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/StructuralAnalysisApiClientV1"
import { BeamOsElement1d } from "./BeamOsElement1d"
import {
  BeamOsObjectTypes,
  objectTypeToString,
} from "../EditorApi/EditorApiAlphaExtensions"

export type DiagramEventMap = {
  moved: object
} & THREE.Object3DEventMap

export class BeamOsDiagram extends BeamOsMesh<
  THREE.BufferGeometry,
  THREE.Material,
  DiagramEventMap
> {
  public static beamOsObjectType = BeamOsObjectTypes.Other
  public beamOsUniqueId: string
  // private static DiagramHex: number = 0xff00ff;

  constructor(
    public beamOsId: number,
    intervals: DiagramConsistentIntervalResponse[],
    element1d: BeamOsElement1d,
    yAxisUp: boolean,
    maxValue: number,
  ) {
    super(
      beamOsId,
      BeamOsDiagram.beamOsObjectType,
      BeamOsDiagram.GetGeometry(intervals, element1d, maxValue),
      new THREE.MeshStandardMaterial({
        // color: BeamOsDiagram.DiagramHex,
        side: THREE.DoubleSide,
        vertexColors: true,
        // wireframe: true,
      }),
    )

    this.beamOsUniqueId =
      objectTypeToString(BeamOsElement1d.beamOsObjectType) + beamOsId.toString()

    this.position.set(
      element1d.startNode.position.x,
      element1d.startNode.position.y,
      element1d.startNode.position.z,
    )

    const deltaX = element1d.endNode.position.x - element1d.startNode.position.x
    const deltaY = element1d.endNode.position.y - element1d.startNode.position.y
    const deltaZ = element1d.endNode.position.z - element1d.startNode.position.z

    const currentAngle = new THREE.Vector3(1, 0, 0)
    const desiredAngle = new THREE.Vector3(deltaX, deltaY, deltaZ)

    const axis = currentAngle.clone().cross(desiredAngle).normalize()
    const x = currentAngle.angleTo(desiredAngle)

    this.setRotationFromAxisAngle(axis, x)

    // GetGeometry is assuming a yAxis is up (three js conventions).
    // Must rotate the geometry if that is the case
    if (!yAxisUp) {
      this.rotateOnAxis(currentAngle, Math.PI / 2)
    }
  }

  static GetGeometry(
    intervals: DiagramConsistentIntervalResponse[],
    element1d: BeamOsElement1d,
    maxValue: number,
  ): THREE.BufferGeometry {
    const maxValueMult = 1 / maxValue

    const point3dArr = new Array<number>()

    const worldStart = element1d.startNode.position
    const worldEnd = element1d.endNode.position
    const worldRange = worldEnd.distanceTo(worldStart)

    const localRange =
      intervals[intervals.length - 1].endLocation.value -
      intervals[0].startLocation.value

    const worldRangeScalingFactor = worldRange / localRange

    for (let i = 0; i < intervals.length - 1; i++) {
      const startLength = intervals[i].startLocation.value
      const endLength = intervals[i].endLocation.value
      const range = endLength - startLength

      for (let arrowIndex = 1; arrowIndex < 4; arrowIndex++) {
        const currentX = startLength + (arrowIndex / 3) * range
        const currentEval = this.EvalPolynomial(
          intervals[i].polynomialCoefficients,
          currentX,
        )

        const previousX = startLength + ((arrowIndex - 1) / 3) * range
        const prevEval = this.EvalPolynomial(
          intervals[i].polynomialCoefficients,
          previousX,
        )

        // previous bottom triangle
        point3dArr.push(previousX * worldRangeScalingFactor, 0, 0)
        point3dArr.push(
          previousX * worldRangeScalingFactor,
          prevEval * maxValueMult,
          0,
        )
        point3dArr.push(currentX * worldRangeScalingFactor, 0, 0)

        // previous top triangle
        point3dArr.push(currentX * worldRangeScalingFactor, 0, 0)
        point3dArr.push(
          previousX * worldRangeScalingFactor,
          prevEval * maxValueMult,
          0,
        )
        point3dArr.push(
          currentX * worldRangeScalingFactor,
          currentEval * maxValueMult,
          0,
        )
      }
    }

    const colorPoints = new Array<number>()
    for (
      let vertexIndex = 0;
      vertexIndex < point3dArr.length;
      vertexIndex += 3
    ) {
      const unity = point3dArr[vertexIndex + 1] / 0.5
      const color = this.GetColorFromUnity(unity)
      colorPoints.push(color.r, color.g, color.b)
    }

    const geometry = new THREE.BufferGeometry()
    geometry.setAttribute(
      "position",
      new THREE.BufferAttribute(new Float32Array(point3dArr), 3),
    )
    geometry.setAttribute(
      "color",
      new THREE.BufferAttribute(new Float32Array(colorPoints), 3),
    )

    return geometry
  }

  static GetHighestValue(
    intervals: DiagramConsistentIntervalResponse[],
  ): number {
    let highest = 0

    intervals.forEach(interval => {
      const startLength = interval.startLocation.value
      const endLength = interval.endLocation.value
      const range = endLength - startLength

      for (let i = 0; i < 15; i++) {
        const currentX = startLength + (i / 15) * range
        const polyEval = this.EvalPolynomial(
          interval.polynomialCoefficients,
          currentX,
        )
        highest = Math.max(highest, Math.abs(polyEval))
      }
    })

    return highest
  }

  static EvalPolynomial(coefficients: number[], xVal: number) {
    let result = 0
    for (let i = 0; i < coefficients.length; i++) {
      result += coefficients[i] * Math.pow(xVal, i)
    }

    return result
  }

  static GetColorFromUnity(unity: number): THREE.Color {
    const negativeColor = new THREE.Color(0, 0, 1)
    const neutralColor = new THREE.Color(1, 1, 1)
    const positiveColor = new THREE.Color(1, 0, 0)
    const result = new THREE.Color()

    // if (unity >= 1) {
    //     result.set(positiveColor);
    // } else if (unity >= 0) {
    //     result.lerpColors(neutralColor, positiveColor, unity);
    // } else if (unity >= -1) {
    //     result.lerpColors(neutralColor, negativeColor, unity);
    // } else {
    //     result.set(negativeColor);
    // }

    if (unity > 0) {
      result.set(positiveColor)
    } else if (unity == 0) {
      result.set(neutralColor)
    } else {
      result.set(negativeColor)
    }

    return result
  }
}
