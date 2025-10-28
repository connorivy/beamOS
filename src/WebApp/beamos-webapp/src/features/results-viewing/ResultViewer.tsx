import type React from "react";
import { useAppSelector } from "../../app/hooks";
import { element1dIdSelector } from "../editors/selection-info/element1d/element1dSelectionSlice";
import { nodeIdSelector } from "../editors/selection-info/node/nodeSelectionSlice";
import { selectModelResponseByCanvasId } from "../editors/editorsSlice";

const ResultViewer: React.FC<{ canvasId: string }> = ({ canvasId }) => {
    const selectedElement1dId = useAppSelector(element1dIdSelector)
    const selectedNodeId = useAppSelector(nodeIdSelector)
    const modelState = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )

    if (selectedElement1dId !== null) {
        const element1d = modelState?.element1ds[selectedElement1dId]
        return (
            <div>
                <h2>Element 1D Details</h2>
                {element1d ? (
                    <pre>{JSON.stringify(element1d, null, 2)}</pre>
                ) : (
                    <p>No data available for Element 1D ID: {selectedElement1dId}</p>
                )}
            </div>
        )
    }

    if (selectedNodeId !== null) {
        const node = modelState?.nodes[selectedNodeId]
        return (
            <div>
                <h2>Node Details</h2>
                {node ? (
                    <pre>{JSON.stringify(node, null, 2)}</pre>
                ) : (
                    <p>No data available for Node ID: {selectedNodeId}</p>
                )}
            </div>
        )
    }

    return (
        <div style={{ display: "flex", justifyContent: "center", alignItems: "center", height: "100%" }}>
            <h1>Results Viewer</h1>
        </div>
    );
};

export default ResultViewer;
