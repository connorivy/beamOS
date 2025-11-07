import { useAppDispatch, useAppSelector } from "../../app/hooks"
import { modelProposalsLoaded, selectModelResponseByCanvasId } from "../editors/editorsSlice"
import React from "react"
import { Typography, IconButton, Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button } from "@mui/material"
import { List, ListItem, ListItemButton, ListItemText } from "@mui/material"
import CheckIcon from "@mui/icons-material/Check"
import CloseIcon from "@mui/icons-material/Close"
import { useApiClient } from "../api-client/ApiClientContext"
import { useEditors } from "../editors/EditorContext"

export function ProposalsView({ canvasId }: { canvasId: string }) {
    const apiClient = useApiClient()
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
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
    const [dialogOpen, setDialogOpen] = React.useState(false);
    const [dialogAction, setDialogAction] = React.useState<'accept' | 'reject' | null>(null);

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

    function openDialog(action: 'accept' | 'reject') {
        setDialogAction(action);
        setDialogOpen(true);
    }

    function closeDialog() {
        setDialogOpen(false);
        setDialogAction(null);
    }

    async function handleDialogConfirm() {
        if (!selectedProposalId || !editorState.remoteModelId || !dialogAction) {
            return;
        }

        try {
            if (dialogAction === 'accept') {
                await apiClient.acceptModelProposal(editorState.remoteModelId, selectedProposalId, null);
            } else if (dialogAction === 'reject') {
                await apiClient.rejectModelProposal(selectedProposalId, editorState.remoteModelId);
            }
            
            // Clear the selected proposal and refresh the view
            setSelectedProposalId(null);
            await beamOsEditor.api.clearModelProposals();
        } catch (error) {
            console.error(`Failed to ${dialogAction} proposal:`, error);
        } finally {
            closeDialog();
        }
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
                    <ListItem 
                        disablePadding 
                        key={option.id}
                        secondaryAction={
                            selectedProposalId === option.id ? (
                                <>
                                    <IconButton
                                        edge="end"
                                        aria-label="accept"
                                        onClick={() => openDialog('accept')}
                                        sx={{ mr: 1 }}
                                    >
                                        <CheckIcon />
                                    </IconButton>
                                    <IconButton
                                        edge="end"
                                        aria-label="reject"
                                        onClick={() => openDialog('reject')}
                                    >
                                        <CloseIcon />
                                    </IconButton>
                                </>
                            ) : null
                        }
                    >
                        <ListItemButton
                            selected={selectedProposalId === option.id}
                            onClick={() => void handleProposalChange(option.id)}
                        >
                            <ListItemText primary={option.name} />
                        </ListItemButton>
                    </ListItem>
                ))}
            </List>

            <Dialog 
                open={dialogOpen} 
                onClose={closeDialog}
                disablePortal={false}
            >
                <DialogTitle>
                    {dialogAction === 'accept' ? 'Accept Proposal' : 'Reject Proposal'}
                </DialogTitle>
                <DialogContent>
                    <DialogContentText>
                        {dialogAction === 'accept' 
                            ? 'Are you sure you want to accept this model proposal? This will apply the changes to your model.'
                            : 'Are you sure you want to reject this model proposal? This will discard the proposed changes.'}
                    </DialogContentText>
                </DialogContent>
                <DialogActions>
                    <Button onClick={closeDialog}>
                        Cancel
                    </Button>
                    <Button 
                        onClick={() => void handleDialogConfirm()} 
                        variant="contained"
                        color={dialogAction === 'accept' ? 'primary' : 'error'}
                    >
                        {dialogAction === 'accept' ? 'Accept' : 'Reject'}
                    </Button>
                </DialogActions>
            </Dialog>
        </div>
    );
}