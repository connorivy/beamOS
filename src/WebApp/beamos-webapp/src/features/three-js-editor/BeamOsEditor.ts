import * as THREE from "three"
import { Raycaster } from "./Raycaster"
import { TransformController } from "./TransformController"
import { Selector, SelectorInfo } from "./Selector"
import { EditorApi } from "./EditorApi"
// import { Line2 } from 'three/addons/lines/Line2.js';
// import { LineGeometry } from 'three/addons/lines/LineGeometry.js';
// import { SimpleGui } from './SimpleGui';
import { EditorConfigurations } from "./EditorConfigurations"
// import type { IEditorEventsApi } from "../../../../../../codeGen/BeamOs.CodeGen.EditorApi/EditorEventsApi"
// import { DotnetApiFactory } from "./EditorApi/DotnetApiFactory"
import { Controls } from "./Controls"
import { Camera } from "./Camera"
import type { IEditorEventsApi } from "./EditorApi/EditorEventsApi"

export class BeamOsEditor {
  public scene: THREE.Scene
  sceneRoot: THREE.Group
  camera: Camera
  renderer: THREE.WebGLRenderer
  mouse: THREE.Vector2
  raycaster: Raycaster
  transformController: TransformController
  selector: Selector
  public api: EditorApi
  animationFrameId: number | undefined = undefined
  observer: ResizeObserver
  private controls: Controls
  private clock: THREE.Clock = new THREE.Clock()

  constructor(
    public domElement: HTMLElement,
    public eventsApi: IEditorEventsApi,
    private editorConfigurations: EditorConfigurations,
  ) {
    this.scene = new THREE.Scene()
    this.scene.add(new THREE.AmbientLight(0xaaaaaa, 10))

    // z-up scene by default
    this.sceneRoot = new THREE.Group()
    // this.sceneRoot.rotateX(-Math.PI / 2);
    // this.sceneRoot.up = new THREE.Vector3(0, 0, 1);
    this.scene.add(this.sceneRoot)

    const group = new THREE.Group()
    this.scene.add(group)
    this.camera = new Camera(this.domElement)
    document.addEventListener("keydown", (event: KeyboardEvent) => {
      if (event.key === "c") {
        void this.handleCameraSwitch()
      }
    })
    editorConfigurations.yAxisUp = false

    const selectorInfo = new SelectorInfo(this.eventsApi, domElement.id)

    this.renderer = new THREE.WebGLRenderer({
      canvas: domElement,
      antialias: true,
    })
    this.mouse = new THREE.Vector2(-1000, -1000)
    this.raycaster = new Raycaster(
      this.renderer,
      this.scene,
      this.mouse,
      this.camera.camera,
    )

    this.controls = new Controls(this.camera.camera, this.domElement)
    this.transformController = new TransformController(
      this.scene,
      this.camera.camera,
      this.domElement,
      this.controls,
      // dotnetDispatcherApi,
    )
    this.selector = new Selector(
      this.domElement,
      this.scene,
      this.mouse,
      this.raycaster.raycastInfo,
      selectorInfo,
      this.transformController,
      editorConfigurations,
      this.controls,
      this.camera.camera,
    )

    this.api = new EditorApi(
      this.camera.camera,
      this.controls,
      this.sceneRoot,
      editorConfigurations,
    )

    const callback = this.resizeCanvasToDisplaySize.bind(this)
    this.observer = new ResizeObserver(entries => {
      const width = entries[0].contentRect.width
      const height = entries[0].contentRect.height

      callback(width, height)
    })
    this.observer.observe(this.domElement)

    this.initCanvas()
    // this.initGui();
    //let simpleGui = new SimpleGui(this.editorConfigurations, this.scene, this.api);
    //console.log(simpleGui);
    this.animate()
  }

  static createFromId(
    domElementId: string,
    isReadOnly: boolean,
    eventsApi: IEditorEventsApi,
  ): BeamOsEditor {
    const domElement = document.getElementById(domElementId)
    if (!domElement) {
      throw new Error(`Unable to find dom element with id ${domElementId}`)
    }

    const editorConfigurations = new EditorConfigurations(isReadOnly)

    return new this(domElement, eventsApi, editorConfigurations)
  }

  initCanvas() {
    this.scene.background = new THREE.Color(0x333333)
    this.scene.matrixWorldAutoUpdate = true

    const grid = new THREE.Group()

    const grid1 = new THREE.GridHelper(30, 30, 0x282828)
    grid1.material.color.setHex(0x282828)
    grid1.material.vertexColors = false
    grid.add(grid1)

    const grid2 = new THREE.GridHelper(30, 6, 0x888888)
    grid2.material.color.setHex(0x888888)
    grid2.material.vertexColors = false
    grid.add(grid2)

    this.scene.add(grid)
  }

  resizeCanvasToDisplaySize(width: number, height: number) {
    // you must pass false here or three.js sadly fights the browser
    console.log("resizing to", width, height)
    this.renderer.setSize(width, height, false)

    if (this.camera.camera instanceof THREE.PerspectiveCamera) {
      this.camera.camera.aspect = width / height
    } else if (this.camera.camera instanceof THREE.OrthographicCamera) {
      // todo
    }
    this.camera.camera.updateProjectionMatrix()

    // set matLine resolution
    this.editorConfigurations.defaultElement1dMaterial.resolution.set(
      width,
      height,
    )
  }

  public animate() {
    this.animationFrameId = requestAnimationFrame(this.animate.bind(this))

    const delta = this.clock.getDelta()
    //const elapsed = this.clock.getElapsedTime();
    //console.log('camera up', this.camera.up);
    this.controls.update(delta)
    this.selector.animate()

    this.render()
  }

  render() {
    this.renderer.render(this.scene, this.camera.camera)

    this.raycaster.raycast()
  }

  public dispose() {
    this.observer.disconnect()
    this.observer.unobserve(this.domElement)
    this.selector.dispose()

    // very important to cancel the animation loop
    // or else the model will not be freed up and garbage collected which will lead to a huge memory leak
    if (this.animationFrameId == undefined) {
      throw new Error("animationFrameId was undefined")
    }
    cancelAnimationFrame(this.animationFrameId)

    // while the above worked most of the time, sometimes the memory leak persisted.
    // this snippet of clearing the scene objects helps too
    this.scene.traverse(object => {
      if (object instanceof THREE.Group) {
        object.clear()
      }
    })

    // unsure if this is necessary
    this.renderer.dispose()
  }

  /**
   * Switches between perspective and orthographic cameras and updates all dependent objects.
   */
  public async handleCameraSwitch() {
    // Switch the camera type (handled by Camera class)
    const lookAtTarget = this.camera.switchCamera()

    // Recreate controls with the new camera
    this.controls = new Controls(this.camera.camera, this.domElement)

    await this.camera.lookAt(
      lookAtTarget.x,
      lookAtTarget.y,
      lookAtTarget.z,
      this.controls,
    )

    // Recreate transformController with the new camera and controls
    this.transformController = new TransformController(
      this.scene,
      this.camera.camera,
      this.domElement,
      this.controls,
      // this.dotnetDispatcherApi,
    )

    // Recreate selector with the new camera, controls, and transformController
    const selectorInfo = new SelectorInfo(this.eventsApi, this.domElement.id)

    // Recreate raycaster with the new camera
    this.raycaster = new Raycaster(
      this.renderer,
      this.scene,
      this.mouse,
      this.camera.camera,
    )

    this.selector.dispose() // Dispose of the old selector
    this.selector = new Selector(
      this.domElement,
      this.scene,
      this.mouse,
      this.raycaster.raycastInfo,
      selectorInfo,
      this.transformController,
      this.editorConfigurations,
      this.controls,
      this.camera.camera,
    )

    // Recreate API with the new camera and controls
    this.api = new EditorApi(
      this.camera.camera,
      this.controls,
      this.sceneRoot,
      this.editorConfigurations,
    )
  }
}
