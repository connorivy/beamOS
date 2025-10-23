import type * as THREE from "three"
import { TransformControls } from "three/addons/controls/TransformControls.js"

// import type { IEditorEventsApi } from "./EditorApi/EditorEventsApi"
import { Coordinate3D } from "./EditorApi/EditorEventsApi"
import { BeamOsNode } from "./SceneObjects/BeamOsNode"
import type { Controls } from "./Controls"

export class TransformController {
  public transformControl: TransformControls
  private startLocation: Coordinate3D | undefined

  constructor(
    scene: THREE.Scene,
    camera: THREE.Camera,
    private domElement: HTMLElement,
    private controls: Controls,
    // private dispatcher: IEditorEventsApi,
  ) {
    this.transformControl = new TransformControls(camera, domElement)
    scene.add(this.transformControl)

    this.transformControl.addEventListener(
      "dragging-changed",
      this.onDraggingChanged.bind(this),
    )

    this.transformControl.addEventListener(
      "objectChange",
      this.onObjectChanged.bind(this),
    )
  }

  //   async onDraggingChanged(event: {
  //     value: boolean
  //     target: { object: unknown }
  //   }) {

  onDraggingChanged(event: unknown) {
    const dragEvent = event as {
      value: boolean
      target: { object: unknown }
    }
    this.controls.enabled = !dragEvent.value

    if (!(dragEvent.target.object instanceof BeamOsNode)) {
      return
    }

    if (!dragEvent.value) {
      if (this.startLocation === undefined) {
        throw new Error("start location is undefined")
      }

      //   this.dispatcher
      //     .dispatchMoveNodeCommand(
      //       new MoveNodeCommand({
      //         canvasId: this.domElement.id,
      //         nodeId: dragEvent.target.object.beamOsId,
      //         previousLocation: this.startLocation,
      //         newLocation: new Coordinate3D({
      //           x: dragEvent.target.object.position.x,
      //           y: dragEvent.target.object.position.y,
      //           z: dragEvent.target.object.position.z,
      //         }),
      //         handledByBlazor: false,
      //         handledByEditor: true,
      //         handledByServer: false,
      //       }),
      //     )
      //     .catch(console.error)

      this.startLocation = undefined
    } else {
      this.startLocation = new Coordinate3D({
        x: dragEvent.target.object.position.x,
        y: dragEvent.target.object.position.y,
        z: dragEvent.target.object.position.z,
      })
    }
  }

  onObjectChanged() {
    ;(this.transformControl.object as BeamOsNode).firePositionChangedEvent()
  }
}
