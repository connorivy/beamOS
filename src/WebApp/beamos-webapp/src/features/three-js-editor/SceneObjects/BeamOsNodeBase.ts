import type * as THREE from "three"
import { BeamOsMesh } from "../BeamOsMesh"
import type { BeamOsObjectType } from "../EditorApi/EditorEventsApi"
import type { NodeEventMap } from "./BeamOsNode"

export abstract class BeamOsNodeBase extends BeamOsMesh<
  THREE.BufferGeometry,
  THREE.Material,
  NodeEventMap
> {
  constructor(
    public beamOsId: number,
    public beamOsObjectType: BeamOsObjectType,
    geometry?: THREE.BufferGeometry,
    material?: THREE.Material,
  ) {
    super(beamOsId, beamOsObjectType, geometry, material)
  }

  abstract GetPosition(): THREE.Vector3
}
