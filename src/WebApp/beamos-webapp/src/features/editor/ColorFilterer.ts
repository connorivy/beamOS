import type * as THREE from "three"
import type { IBeamOsMesh } from "./BeamOsMesh"
import { isBeamOsMesh } from "./Raycaster"

export class ColorFilterBuilder {
  private originalMaterials: Map<IBeamOsMesh, THREE.Material> = new Map<
    IBeamOsMesh,
    THREE.Material
  >()
  private filterMap = new Map<IBeamOsMesh, { color: number; ghost: boolean }>()
  private static greyColor = 0x808080 // Default grey color

  public add(mesh: IBeamOsMesh, color: number, ghost: boolean, force: boolean) {
    if (!force && this.filterMap.has(mesh)) {
      throw new Error(
        "Color filter already applied to this mesh. Use force=true to override.",
      )
    }

    this.filterMap.set(mesh, { color, ghost })
  }

  public apply() {
    for (const [mesh, filter] of this.filterMap.entries()) {
      if (!this.originalMaterials.has(mesh)) {
        this.originalMaterials.set(mesh, mesh.material)
      }
      mesh.SetColorFilter(filter.color, filter.ghost)
    }
  }

  public clear() {
    for (const [mesh, originalMaterial] of this.originalMaterials.entries()) {
      mesh.material = originalMaterial
    }
  }

  public static CreateFromAllGhosted(
    meshes: IBeamOsMesh[],
  ): ColorFilterBuilder {
    const builder = new ColorFilterBuilder()
    for (const mesh of meshes) {
      builder.add(mesh, ColorFilterBuilder.greyColor, true, false)
    }
    return builder
  }

  public static CreateFromAllGhostedScene(
    scene: THREE.Group,
  ): ColorFilterBuilder {
    const beamOsMeshes: IBeamOsMesh[] = []
    scene.traverse(object => {
      if (isBeamOsMesh(object)) {
        beamOsMeshes.push(object)
      }
    })
    return this.CreateFromAllGhosted(beamOsMeshes)
  }
}
