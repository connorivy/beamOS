import { Line2 } from "three/addons/lines/Line2.js";
import { LineGeometry } from "three/addons/lines/LineGeometry.js";
import { GUI } from "three/addons/libs/lil-gui.module.min.js";
import { EditorConfigurations } from "./EditorConfigurations";
// import { INodeResponse, IPointResponse, IRestraintResponse, NodeResponse } from './EditorApi/EditorApiAlpha';

export class SimpleGui {
    startX: number = 0;
    startY: number = 0;
    startZ: number = 0;
    endX: number = 10;
    endY: number = 10;
    endZ: number = 10;
    startNodeId: string = "";
    endNodeId: string = "";

    constructor(
        private config: EditorConfigurations,
        private scene: THREE.Scene
    ) {
        let gui = new GUI();
        let params = {
            "line type": 0,
            // 'world units': matLine.worldUnits,
            // 'visualize threshold': matThresholdLine.visible,
            width: this.config.defaultElement1dMaterial.linewidth,
            alphaToCoverage:
                this.config.defaultElement1dMaterial.alphaToCoverage,
            // 'threshold': raycaster.params.Line2.threshold,
            // 'translation': raycaster.params.Line2.threshold,
            animate: true,
            // addLine: this.addElement1d.bind(this),
            // addNodes: this.addNodes.bind(this),
            startX: this.startX,
            startY: this.startY,
            startZ: this.startZ,
            endX: this.endX,
            endY: this.endY,
            endZ: this.endZ,
        };

        gui.add(params, "width", 0.1, 10).onChange(
            this.changeLineWidth.bind(this)
        );

        gui.add(params, "startX");
        gui.add(params, "startY");
        gui.add(params, "startZ");
        gui.add(params, "endX");
        gui.add(params, "endY");
        gui.add(params, "endZ");
    }

    changeLineWidth(val: number) {
        this.config.defaultElement1dMaterial.linewidth = val;
    }

    addLine() {
        const lineGeometry = new LineGeometry();
        lineGeometry.setPositions([
            this.startX,
            this.startY,
            this.startZ,
            this.endX,
            this.endY,
            this.endZ,
        ]);
        // lineGeometry.setColors( colors );

        let line = new Line2(
            lineGeometry,
            this.config.defaultElement1dMaterial
        );
        line.computeLineDistances();
        line.scale.set(1, 1, 1);
        this.scene.add(line);
    }
}
