import * as THREE from "three";
import { TransformControls } from "three/addons/controls/TransformControls.js";
import {
    Coordinate3D,
    IEditorEventsApi,
    MoveNodeCommand,
} from "./EditorApi/EditorEventsApi";
import { BeamOsNode } from "./SceneObjects/BeamOsNode";
import { Controls } from "./Controls";

type DraggingChangedEvent = THREE.Event<"dragging-changed", TransformControls> & {
    value: unknown;
};

export class TransformController {
    public transformControl: TransformControls;
    private startLocation: Coordinate3D | undefined;

    constructor(
        scene: THREE.Scene,
        camera: THREE.Camera,
        private domElement: HTMLElement,
        private controls: Controls,
        private dispatcher: IEditorEventsApi
    ) {
        this.transformControl = new TransformControls(camera, domElement);
        scene.add(this.transformControl);

        this.transformControl.addEventListener(
            "dragging-changed",
            this.onDraggingChanged.bind(this)
        );

        this.transformControl.addEventListener(
            "objectChange",
            this.onObjectChanged.bind(this)
        );
    }

    async onDraggingChanged(event: DraggingChangedEvent) {
        const isDragging = Boolean(event.value);
        this.controls.enabled = !isDragging;
        const draggedObject = event.target.object as BeamOsNode;

        if (!isDragging) {
            if (this.startLocation === undefined) {
                throw new Error("start location is undefined");
            }

            await this.dispatcher.dispatchMoveNodeCommand(
                new MoveNodeCommand({
                    canvasId: this.domElement.id,
                    nodeId: draggedObject.beamOsId,
                    previousLocation: this.startLocation,
                    newLocation: new Coordinate3D({
                        x: draggedObject.position.x,
                        y: draggedObject.position.y,
                        z: draggedObject.position.z,
                    }),
                    handledByBlazor: false,
                    handledByEditor: true,
                    handledByServer: false,
                })
            );

            this.startLocation = undefined;
        } else {
            this.startLocation = new Coordinate3D({
                x: draggedObject.position.x,
                y: draggedObject.position.y,
                z: draggedObject.position.z,
            });
        }
    }

    onObjectChanged() {
        (this.transformControl.object as BeamOsNode).firePositionChangedEvent();
    }
}
