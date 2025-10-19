import * as THREE from "three"
import type { Restraint } from "../../../../../../../codeGen/BeamOs.CodeGen.EditorApi/EditorApiAlpha"
import {
  BeamOsObjectTypes,
  RestraintContractUtils,
  RestraintType,
} from "../EditorApi/EditorApiAlphaExtensions"
import type { BeamOsElement1d } from "./BeamOsElement1d"
import { BeamOsNode } from "./BeamOsNode"
import { BeamOsNodeBase } from "./BeamOsNodeBase"

export class BeamOsInternalNode extends BeamOsNodeBase {
  public static beamOsObjectType = BeamOsObjectTypes.InternalNode
  // public beamOsObjectType: string = BeamOsNode.beamOsObjectType;
  public static nodeRadius = 0.1
  private onElementMovedFunc: (_event: unknown) => void

  private _restraint: Restraint

  constructor(
    beamOsid: number,
    private element1d: BeamOsElement1d,
    public ratioAlongElement1d: number,
    restraint: Restraint,
    yAxisUp: boolean,
    objectType: number = BeamOsInternalNode.beamOsObjectType,
  ) {
    const restraintType = RestraintContractUtils.GetRestraintType(restraint)
    super(
      beamOsid,
      objectType,
      BeamOsNode.GetGeometry(restraintType),
      new THREE.MeshLambertMaterial({ color: BeamOsNode.nodeHex }),
    )
    this._restraint = restraint
    this.setMeshPositionFromCoordinates()
    this.onElementMovedFunc = this.onElementMoved.bind(this)
    element1d.addEventListener("moved", this.onElementMovedFunc)

    // GetGeometry is assuming a yAxis is up (three js conventions).
    // Must rotate the geometry if that is the case
    if (!yAxisUp) {
      this.rotateOnAxis(new THREE.Vector3(1, 0, 0), Math.PI / 2)
    }
  }

  set restraint(value) {
    this._restraint = value
    this.updateGeometryFromRestraint()
  }

  get restraint() {
    return this._restraint
  }

  public GetPosition(): THREE.Vector3 {
    // get the ratio between the start and end of the element1d
    const element1dStart = this.element1d.startNode.GetPosition()
    const element1dEnd = this.element1d.endNode.GetPosition()
    const position = new THREE.Vector3()
    position.x =
      element1dStart.x +
      (element1dEnd.x - element1dStart.x) * this.ratioAlongElement1d
    position.y =
      element1dStart.y +
      (element1dEnd.y - element1dStart.y) * this.ratioAlongElement1d
    position.z =
      element1dStart.z +
      (element1dEnd.z - element1dStart.z) * this.ratioAlongElement1d
    return position
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onElementMoved(_event: unknown) {
    this.setMeshPositionFromCoordinates()
    this.dispatchEvent({ type: "moved" })
  }

  public setMeshPositionFromCoordinates() {
    this.position.copy(this.GetPosition())
    this.geometry.attributes.position.needsUpdate = true
  }

  public firePositionChangedEvent() {
    this.dispatchEvent({ type: "moved" })
  }

  static GetGeometry(restraint: RestraintType): THREE.BufferGeometry {
    if (restraint == RestraintType.Pinned) {
      return new THREE.ConeGeometry(0.1, 0.2)
    } else if (restraint == RestraintType.Fixed) {
      const boxSideLength = BeamOsNode.nodeRadius * 2
      return new THREE.BoxGeometry(boxSideLength, boxSideLength, boxSideLength)
    } else {
      return new THREE.SphereGeometry(BeamOsNode.nodeRadius)
    }
  }

  // Add this method to your BeamOsNode class
  updateGeometryFromRestraint() {
    // Dispose of the old geometry to prevent memory leaks
    // if (this.geometry) {
    this.geometry.dispose()
    // }

    const restraintType = RestraintContractUtils.GetRestraintType(
      this.restraint,
    )
    this.geometry = BeamOsNode.GetGeometry(restraintType)

    // Update the mesh
    this.geometry.attributes.position.needsUpdate = true
  }
}

export class BeamOsInternalNodeProposal extends BeamOsInternalNode {
  public static beamOsObjectType = BeamOsObjectTypes.InternalNodeProposal

  constructor(
    public existingNodeId: number | undefined,
    beamOsid: number,
    element1d: BeamOsElement1d,
    ratio: number,
    restraint: Restraint,
    yAxisUp: boolean,
  ) {
    super(
      beamOsid,
      element1d,
      ratio,
      restraint,
      yAxisUp,
      BeamOsObjectTypes.InternalNodeProposal,
    )
  }

  public IsExisting() {
    return this.existingNodeId != undefined
  }
}
