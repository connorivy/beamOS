import type React from "react";
// import { useAppSelector } from "../../app/hooks";
// import { element1dIdSelector } from "../editors/selection-info/element1d/element1dSelectionSlice";
// import { nodeIdSelector } from "../editors/selection-info/node/nodeSelectionSlice";
// import { selectModelResponseByCanvasId } from "../editors/editorsSlice";
import { Element1dResultCharts } from "./charts/Element1dCharts";
import { element1dIdSelector } from "../editors/selection-info/element1d/element1dSelectionSlice";
import { selectSelectedResultSetId } from "../editors/editorsSlice";
import { useAppSelector } from "../../app/hooks";

const ResultViewer: React.FC<{ canvasId: string, onOpen: () => void, onClose: () => void }> = ({ canvasId, onOpen, onClose }) => {
    const selectedElement1dId = useAppSelector(element1dIdSelector)
    const selectedResultSetId = useAppSelector(state => selectSelectedResultSetId(state, canvasId))
    // const selectedNodeId = useAppSelector(nodeIdSelector)
    // const modelState = useAppSelector(state =>
    //     selectModelResponseByCanvasId(state, canvasId)
    // )

    if (selectedElement1dId && selectedResultSetId) {
        onOpen();
        return (
            <Element1dResultCharts
                canvasId={canvasId}
                element1dId={selectedElement1dId}
                resultSetId={selectedResultSetId} />
        )
    }
    else {
        onClose();
    }

    // if (selectedElement1dId !== null) {
    //     const element1d = modelState?.element1ds[selectedElement1dId]
    //     return (
    //         <div>
    //             <h2>Element 1D Details</h2>
    //             {element1d ? (
    //                 <pre>{JSON.stringify(element1d, null, 2)}</pre>
    //             ) : (
    //                 <p>No data available for Element 1D ID: {selectedElement1dId}</p>
    //             )}
    //         </div>
    //     )
    // }

    // if (selectedNodeId !== null) {
    //     const node = modelState?.nodes[selectedNodeId]
    //     return (
    //         <div>
    //             <h2>Node Details</h2>
    //             {node ? (
    //                 <pre>{JSON.stringify(node, null, 2)}</pre>
    //             ) : (
    //                 <p>No data available for Node ID: {selectedNodeId}</p>
    //             )}
    //         </div>
    //     )
    // }

    // return (
    //     <div style={{ display: "flex", justifyContent: "center", alignItems: "center", height: "100%" }}>
    //         <h1>Results Viewer</h1>
    //     </div>
    // );
};

export default ResultViewer;
