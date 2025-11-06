import { useAppSelector } from "../../app/hooks"
import { selectModelResponseByCanvasId } from "../editors/editorsSlice"
import React from "react"
import { FormControl, InputLabel, Select, MenuItem, Typography, SelectChangeEvent } from "@mui/material"

export function ProposalsView({ canvasId }: { canvasId: string }) {
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    )
    const proposalIds: number[] = [
        ...Object.keys(modelResponse?.proposals ?? {}).map(id => Number(id))
    ]

    if (proposalIds.length === 0) {
        return <Typography variant="body1" color="textSecondary">No Model Proposals</Typography>
    }

    // Get proposal names (assuming modelResponse.proposals is an object with id keys)
    const proposalOptions = proposalIds.map(id => ({
        id,
        name: modelResponse?.proposals?.[id]?.name ?? `Proposal ${id}`
    }))

    const [selectedProposalId, setSelectedProposalId] = React.useState<string>("")

    function handleProposalChange(e: SelectChangeEvent) {
        const value = e.target.value
        setSelectedProposalId(value)
        // Stub: call a function when proposal changes
        // onProposalChange(value === "" ? null : Number(value)) // can be implemented later
    }

    return (
        <FormControl fullWidth variant="outlined" size="small">
            <InputLabel id="proposal-select-label">Model Proposal</InputLabel>
            <Select
                labelId="proposal-select-label"
                id="proposal-select"
                value={selectedProposalId}
                label="Model Proposal"
                onChange={handleProposalChange}
            >
                <MenuItem value="">
                    <em>-- No Selection --</em>
                </MenuItem>
                {proposalOptions.map(option => (
                    <MenuItem key={option.id} value={option.id.toString()}>
                        {option.name}
                    </MenuItem>
                ))}
            </Select>
        </FormControl>
    )
}