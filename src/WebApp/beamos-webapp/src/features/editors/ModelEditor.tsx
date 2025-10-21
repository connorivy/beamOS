import { useState } from "react";
import { RemoteEditorComponent } from "./EditorComponent";
import { useParams } from "react-router-dom";
import AppBarMain from "../../components/AppBarMain";

// Types for panel content
export enum PanelContent {
    None,
    ProposalInfo,
    Element1dResults,
    ModelRepairScenarioCreator,
}

const ModelEditor = () => {
    // Get modelId from route params
    const { modelId } = useParams<{ modelId: string }>();
    // State for panel content stack and lock
    const [panelContentStack, setPanelContentStack] = useState<PanelContent[]>([]);
    const [panelContentLocked] = useState(false);

    if (!modelId) {
        // Handle error, show a message, or redirect
        return <div>Model ID is required.</div>;
    }

    // Panel navigation handlers
    const goBack = () => {
        if (!panelContentLocked && panelContentStack.length > 0) {
            setPanelContentStack(stack => stack.slice(0, -1));
        }
    };

    // Layout and rendering
    // modelId will be used for child components and logic (see Blazor version)
    return (
        <div className="relative h-full w-full">
            <AppBarMain />
            <RemoteEditorComponent modelId={modelId} isReadOnly={false} />
            {/* Example toolbar rendering, replace with actual components. modelId will be passed as needed. */}
            {/* <div className="flex items-center justify-center absolute left-4 right-4 top-4">
                <AnalysisToolbar ModelId={modelId} />
            </div> */}
            {/* Example floating sidebar and panel rendering */}
            {panelContentStack.length > 0 && (
                <div className="floating-responsive-panel">
                    {!panelContentLocked && panelContentStack.length > 1 && (
                        <button className="absolute top-2 left-2 z-10" onClick={goBack}>
                            ← Back
                        </button>
                    )}
                    {/* Switch panel content */}
                    {(() => {
                        const panelContent = panelContentStack[panelContentStack.length - 1];
                        switch (panelContent) {
                            case PanelContent.ProposalInfo:
                                return <div>Proposal Info Panel</div>;
                            case PanelContent.Element1dResults:
                                return <div>Element1d Results Panel</div>;
                            case PanelContent.ModelRepairScenarioCreator:
                                return <div>Model Repair Scenario Panel</div>;
                            default:
                                return null;
                        }
                    })()}
                </div>
            )}
            {/* <AiAssistant ModelId={modelId} /> */}
        </div>
    );
};

export default ModelEditor;
