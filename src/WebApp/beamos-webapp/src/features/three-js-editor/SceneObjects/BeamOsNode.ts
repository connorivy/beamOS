import * as THREE from "three"
import type { Restraint } from "../../../../../../../codeGen/BeamOs.CodeGen.EditorApi/EditorApiAlpha"
import {
  BeamOsObjectTypes,
  RestraintContractUtils,
  RestraintType,
} from "../EditorApi/EditorApiAlphaExtensions"
import type { BeamOsObjectType } from "../EditorApi/EditorEventsApi"
import { BeamOsNodeBase } from "./BeamOsNodeBase"

export type NodeEventMap = {
  moved: object
} & THREE.Object3DEventMap

export class BeamOsNode extends BeamOsNodeBase {
  public static beamOsObjectType: BeamOsObjectType = BeamOsObjectTypes.Node
  // public beamOsObjectType: string = BeamOsNode.beamOsObjectType;
  public static nodeHex = 0x00ff00
  public static nodeRadius = 0.1

  private _restraint: Restraint

  constructor(
    beamOsid: number,
    public xCoordinate: number,
    public yCoordinate: number,
    public zCoordinate: number,
    restraint: Restraint,
    yAxisUp: boolean,
    objectType: BeamOsObjectType = BeamOsNode.beamOsObjectType,
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

    // GetGeometry is assuming a yAxis is up (three js conventions).
    // Must rotate the geometry if that is the case
    if (!yAxisUp) {
      this.rotateOnAxis(new THREE.Vector3(1, 0, 0), Math.PI / 2)
    }
  }

  GetPosition(): THREE.Vector3 {
    return new THREE.Vector3(
      this.xCoordinate,
      this.yCoordinate,
      this.zCoordinate,
    )
  }

  set restraint(value) {
    this._restraint = value
    this.updateGeometryFromRestraint()
  }

  get restraint() {
    return this._restraint
  }

  public setMeshPositionFromCoordinates() {
    this.position.set(this.xCoordinate, this.yCoordinate, this.zCoordinate)
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
    this.geometry.dispose()

    const restraintType = RestraintContractUtils.GetRestraintType(
      this.restraint,
    )
    this.geometry = BeamOsNode.GetGeometry(restraintType)

    // Update the mesh
    this.geometry.attributes.position.needsUpdate = true
  }
}

export class BeamOsNodeProposal extends BeamOsNode {
  public static beamOsObjectType: BeamOsObjectType =
    BeamOsObjectTypes.NodeProposal
  constructor(
    public existingNodeId: number | undefined,
    beamOsid: number,
    xCoordinate: number,
    yCoordinate: number,
    zCoordinate: number,
    restraint: Restraint,
    yAxisUp: boolean,
  ) {
    super(
      beamOsid,
      xCoordinate,
      yCoordinate,
      zCoordinate,
      restraint,
      yAxisUp,
      BeamOsObjectTypes.NodeProposal,
    )
  }

  public IsExisting() {
    return this.existingNodeId != undefined
  }
}
