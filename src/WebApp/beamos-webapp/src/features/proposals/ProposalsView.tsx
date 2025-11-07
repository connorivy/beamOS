import { useAppDispatch, useAppSelector } from "../../app/hooks"
import { modelProposalsLoaded, selectModelResponseByCanvasId } from "../editors/editorsSlice"
import React from "react"
import { Typography } from "@mui/material"
import { List, ListItem, ListItemButton, ListItemText } from "@mui/material"
import { useApiClient } from "../api-client/ApiClientContext"
import { useEditors } from "../editors/EditorContext"

export function ProposalsView({ canvasId }: { canvasId: string }) {
    const apiClient = useApiClient()
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const editor = useAppSelector(state =>
        state.editors[canvasId]
    )
    const proposalIds: number[] = [
        ...Object.keys(modelResponse?.proposals ?? {}).map(id => Number(id))
    ]
    const dispatch = useAppDispatch()
    const editors = useEditors()
    const beamOsEditor = editors[canvasId]
    const editorState = useAppSelector(state => state.editors[canvasId])

    if (proposalIds.length === 0) {
        return <Typography variant="body1" color="textSecondary">No Model Proposals</Typography>;
    }

    // Get proposal names (assuming modelResponse.proposals is an object with id keys)
    const proposalOptions = proposalIds.map(id => ({
        id,
        name: modelResponse?.proposals?.[id]?.name ?? `Proposal ${id}`
    }));

    const [selectedProposalId, setSelectedProposalId] = React.useState<number | null>(null);

    async function handleProposalChange(value: number | null) {
        setSelectedProposalId(value);
        await beamOsEditor.api.clearModelProposals();
        if (value === null) {
            return;
        }
        if (!editorState.remoteModelId) {
            throw new Error("Remote Model ID is not set in the editor state.");
        }

        var proposal = await apiClient.getModelProposal(value, editorState.remoteModelId);
        dispatch(modelProposalsLoaded({ canvasId, proposals: [proposal] }));
        await beamOsEditor.api.displayModelProposal(proposal);
    }

    return (
        <div className="p-2" id="model-proposals-select">
            <Typography variant="h6" gutterBottom>Model Proposals</Typography>
            <List>
                <ListItem disablePadding key="none">
                    <ListItemButton
                        selected={selectedProposalId === null}
                        onClick={() => void handleProposalChange(null)}
                    >
                        <ListItemText primary={<em>-- No Selection --</em>} />
                    </ListItemButton>
                </ListItem>
                {proposalOptions.map(option => (
                    <ListItem disablePadding key={option.id}>
                        <ListItemButton
                            selected={selectedProposalId === option.id}
                            onClick={() => void handleProposalChange(option.id)}
                        >
                            <ListItemText primary={option.name} />
                        </ListItemButton>
                    </ListItem>
                ))}
            </List>
        </div>
    );
}