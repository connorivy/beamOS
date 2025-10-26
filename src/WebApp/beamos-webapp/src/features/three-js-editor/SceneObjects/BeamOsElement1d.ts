import * as THREE from "three"
import { Line2 } from "three/examples/jsm/lines/Line2.js"
import { LineGeometry, LineMaterial } from "three/examples/jsm/Addons.js"
import type { BeamOsNode } from "./BeamOsNode"
import type { IBeamOsMesh } from "../BeamOsMesh"
import {
  BeamOsObjectTypes,
  objectTypeToString,
} from "../EditorApi/EditorApiAlphaExtensions"
import type { BeamOsNodeBase } from "./BeamOsNodeBase"

export type Element1dEventMap = {
  moved: object
} & THREE.Object3DEventMap

export class BeamOsElement1d extends Line2 implements IBeamOsMesh {
  public static lineThickness = 0.1
  public static beamOsObjectType = BeamOsObjectTypes.Element1d
  public beamOsObjectType: number
  private onNodeMovedFunc: (_event: unknown) => void
  private previousMaterial: LineMaterial | undefined
  public beamOsUniqueId: string

  constructor(
    public beamOsId: number,
    public startNode: BeamOsNodeBase,
    public endNode: BeamOsNodeBase,
    lineMaterial: LineMaterial,
    objectType = BeamOsElement1d.beamOsObjectType,
  ) {
    super(new LineGeometry(), lineMaterial)

    this.beamOsObjectType = objectType
    this.beamOsUniqueId = objectTypeToString(objectType) + beamOsId.toString()

    this.setPositions()
    this.computeLineDistances()
    this.scale.set(1, 1, 1)

    this.onNodeMovedFunc = this.onNodeMoved.bind(this)
    startNode.addEventListener("moved", this.onNodeMovedFunc)
    endNode.addEventListener("moved", this.onNodeMovedFunc)
  }

  public SetColorFilter(color: number, ghost: boolean) {
    this.previousMaterial = this.material
    const copy = new LineMaterial({
      color: color,
      linewidth: this.material.linewidth,
      worldUnits: true,
      // transparent: true,
      // opacity: 0.2,
      depthTest: false,
    })
    if (ghost) {
      copy.opacity = 0.2
      copy.transparent = true
    }
    this.material = copy
  }

  public RemoveColorFilter() {
    if (this.previousMaterial == undefined) {
      throw new Error("Trying to unghost, but previous material is undefined")
    }
    this.material = this.previousMaterial
    this.previousMaterial = undefined
  }

  public ReplaceStartNode(newNode: BeamOsNode) {
    this.startNode.removeEventListener("moved", this.onNodeMovedFunc)
    this.startNode = newNode
    this.startNode.addEventListener("moved", this.onNodeMovedFunc)
    this.setPositions()
  }
  public ReplaceEndNode(newNode: BeamOsNode) {
    this.endNode.removeEventListener("moved", this.onNodeMovedFunc)
    this.endNode = newNode
    this.endNode.addEventListener("moved", this.onNodeMovedFunc)
    this.setPositions()
  }

  public GetPosition(): THREE.Vector3 {
    return BeamOsElement1d.GetMiddlePosition(
      this.startNode.position,
      this.endNode.position,
    )
  }

  // Strongly-typed event methods for custom event map
  public addEventListener<K extends keyof Element1dEventMap>(
    type: K,
    listener: (event: Element1dEventMap[K]) => void,
  ): void {
    super.addEventListener(type as string, listener)
  }

  public dispatchEvent<K extends keyof Element1dEventMap>(
    event: { type: K } & Element1dEventMap[K],
  ): void {
    super.dispatchEvent(
      event as unknown as THREE.BaseEvent<keyof THREE.Object3DEventMap>,
    )
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  onNodeMoved(_event: unknown) {
    // Remove this element from its parent (scene or group) and dispose resources
    if (this.parent) {
      this.parent.remove(this)
      this.geometry.dispose()
      if (typeof this.material.dispose === "function") {
        this.material.dispose()
      }
    }
    // Optionally, dispatch an event if needed (commented out since the element is removed)
    this.setPositions()
    this.dispatchEvent({ type: "moved" })
    // If you want to trigger additional cleanup, do it here
  }

  setPositions() {
    const startNodeLocation = this.startNode.GetPosition()
    const endNodeLocation = this.endNode.GetPosition()
    this.geometry.setPositions([
      startNodeLocation.x,
      startNodeLocation.y,
      startNodeLocation.z,
      endNodeLocation.x,
      endNodeLocation.y,
      endNodeLocation.z,
    ])
    this.geometry.attributes.position.needsUpdate = true
  }

  static GetMiddlePosition(
    start: THREE.Vector3,
    end: THREE.Vector3,
  ): THREE.Vector3 {
    const middleX = (start.x + end.x) / 2
    const middleY = (start.y + end.y) / 2
    const middleZ = (start.z + end.z) / 2
    return new THREE.Vector3(middleX, middleY, middleZ)
  }
}

export class BeamOsElement1dProposal extends BeamOsElement1d {
  public static beamOsObjectType = BeamOsObjectTypes.Element1dProposal
  constructor(
    public existingElementId: number | undefined,
    beamOsId: number,
    startNode: BeamOsNode,
    endNode: BeamOsNode,
    lineMaterial: LineMaterial,
  ) {
    super(
      beamOsId,
      startNode,
      endNode,
      lineMaterial,
      BeamOsElement1dProposal.beamOsObjectType,
    )
  }
}
