import * as THREE from "three"
import { BufferGeometryUtils } from "three/examples/jsm/Addons.js"
import { BeamOsMesh } from "../BeamOsMesh"
import type { ShearDiagramResponse } from "../../../../../../../codeGen/BeamOs.CodeGen.EditorApi/EditorApiAlpha"
import { BeamOsPointLoad } from "./BeamOsPointLoad"
import { BeamOsElement1d } from "./BeamOsElement1d"
import { BeamOsObjectTypes } from "../EditorApi/EditorApiAlphaExtensions"

export type DistributedLoadEventMap = {
  moved: object
} & THREE.Object3DEventMap

export class BeamOsDistributedLoad extends BeamOsMesh<
  THREE.BufferGeometry,
  THREE.Material,
  DistributedLoadEventMap
> {
  public static beamOsObjectType: number = BeamOsObjectTypes.DistributedLoad

  private static DistributedLoadHex = 0x00ff00

  constructor(
    public beamOsId: number,
    shearDiagramResponse: ShearDiagramResponse,
    element1d: BeamOsElement1d,
  ) {
    super(
      beamOsId,
      BeamOsDistributedLoad.beamOsObjectType,
      BeamOsDistributedLoad.GetGeometry(shearDiagramResponse, element1d),
      new THREE.MeshLambertMaterial({
        color: BeamOsDistributedLoad.DistributedLoadHex,
      }),
    )

    this.position.set(
      element1d.startNode.position.x +
        BeamOsElement1d.lineThickness *
          shearDiagramResponse.globalShearDirection.x,
      element1d.startNode.position.y +
        BeamOsElement1d.lineThickness *
          shearDiagramResponse.globalShearDirection.y,
      element1d.startNode.position.z +
        BeamOsElement1d.lineThickness *
          shearDiagramResponse.globalShearDirection.z,
    )

    const currentAngle = new THREE.Vector3(0, 0, 1)
    const desiredAngle = new THREE.Vector3(
      shearDiagramResponse.globalShearDirection.x,
      shearDiagramResponse.globalShearDirection.y,
      shearDiagramResponse.globalShearDirection.z,
    )

    const axis = currentAngle.clone().cross(desiredAngle).normalize()
    const x = currentAngle.angleTo(desiredAngle)

    this.setRotationFromAxisAngle(axis, -x)
  }

  static GetGeometry(
    diagram: ShearDiagramResponse,
    element1d: BeamOsElement1d,
  ): THREE.BufferGeometry {
    const geometries: THREE.BufferGeometry[] = []
    // let highestValue = this.GetHighestValue(diagram);
    const deltaX = element1d.endNode.position.x - element1d.startNode.position.x
    const deltaY = element1d.endNode.position.y - element1d.startNode.position.y
    const deltaZ = element1d.endNode.position.z - element1d.startNode.position.z
    const distance = Math.sqrt(
      deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ,
    )

    for (const i of diagram.intervals) {
      const startLength = i.startLocation.value
      const endLength = i.endLocation.value
      const range = endLength - startLength

      for (let arrowIndex = 0; arrowIndex < 3; arrowIndex++) {
        const currentX = startLength + (arrowIndex / 3) * range
        // let polyEval = this.EvalPolynomial(
        //     diagram.intervals[i].polynomialCoefficients,
        //     currentX
        // );
        // let pointAlongBeam = new THREE.Vector3(
        //     element1d.startNode.position.x +
        //         (deltaX * currentX) / distance,
        //     element1d.startNode.position.y +
        //         (deltaY * currentX) / distance,
        //     element1d.startNode.position.z +
        //         (deltaZ * currentX) / distance
        // );

        const newGeo = BeamOsPointLoad.GetGeometry()
          // .scale(polyEval, polyEval, polyEval)
          .translate(
            (deltaX * currentX) / distance,
            (deltaY * currentX) / distance,
            (deltaZ * currentX) / distance,
          )
        // .translate(
        //     pointAlongBeam.x +
        //         BeamOsElement1d.lineThickness *
        //             diagram.globalShearDirection.x,
        //     pointAlongBeam.y +
        //         BeamOsElement1d.lineThickness *
        //             diagram.globalShearDirection.y,
        //     pointAlongBeam.z +
        //         BeamOsElement1d.lineThickness *
        //             diagram.globalShearDirection.z
        // );
        // .lookAt(pointAlongBeam.clone());

        geometries.push(newGeo)
      }
    }

    return BufferGeometryUtils.mergeGeometries(geometries)
  }

  static GetHighestValue(diagram: ShearDiagramResponse): number {
    let highest = 0

    diagram.intervals.forEach(interval => {
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
}
