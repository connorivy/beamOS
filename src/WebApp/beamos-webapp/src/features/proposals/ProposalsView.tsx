import { useAppDispatch, useAppSelector } from "../../app/hooks"
import { modelProposalsLoaded, selectModelResponseByCanvasId } from "../editors/editorsSlice"
import React from "react"
import { FormControl, InputLabel, Select, MenuItem, Typography } from "@mui/material"
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
        return <Typography variant="body1" color="textSecondary">No Model Proposals</Typography>
    }

    // Get proposal names (assuming modelResponse.proposals is an object with id keys)
    const proposalOptions = proposalIds.map(id => ({
        id,
        name: modelResponse?.proposals?.[id]?.name ?? `Proposal ${id}`
    }))

    const [selectedProposalId, setSelectedProposalId] = React.useState<number | null>(null)

    async function handleProposalChange(value: number | null) {
        setSelectedProposalId(value)
        await beamOsEditor.api.clearModelProposals()
        if (value === null) {
            return
        }
        if (!editorState.remoteModelId) {
            throw new Error("Remote Model ID is not set in the editor state.")
        }

        var proposal = await apiClient.getModelProposal(value, editorState.remoteModelId)
        dispatch(modelProposalsLoaded({ canvasId, proposals: [proposal] }));
        await beamOsEditor.api.displayModelProposal(proposal)
    }

    return (
        <FormControl fullWidth variant="outlined" size="small">
            <InputLabel id="proposal-select-label">Model Proposal</InputLabel>
            <Select
                labelId="proposal-select-label"
                id="proposal-select"
                value={selectedProposalId}
                label="Model Proposal"
                onChange={(e) => void handleProposalChange(e.target.value ? Number(e.target.value) : null)}
            >
                <MenuItem value={""}>
                    <em>-- No Selection --</em>
                </MenuItem>
                {proposalOptions.map(option => (
                    <MenuItem key={option.id} value={option.id}>
                        {option.name}
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    )
}