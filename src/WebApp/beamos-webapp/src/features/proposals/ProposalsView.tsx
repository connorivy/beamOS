import { useAppDispatch, useAppSelector } from "../../app/hooks"
import { modelProposalsLoaded, selectModelResponseByCanvasId } from "../editors/editorsSlice"
import React from "react"
import { Typography, IconButton, Dialog, DialogTitle, DialogContent, DialogActions, Button, Box } from "@mui/material"
import { List, ListItem, ListItemButton, ListItemText } from "@mui/material"
import CheckIcon from '@mui/icons-material/Check'
import CloseIcon from '@mui/icons-material/Close'
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
    const [confirmDialogOpen, setConfirmDialogOpen] = React.useState(false);
    const [confirmAction, setConfirmAction] = React.useState<'accept' | 'reject' | null>(null);
    const [isProcessing, setIsProcessing] = React.useState(false);

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

    function handleAcceptClick() {
        setConfirmAction('accept');
        setConfirmDialogOpen(true);
    }

    function handleRejectClick() {
        setConfirmAction('reject');
        setConfirmDialogOpen(true);
    }

    function handleDialogClose() {
        if (!isProcessing) {
            setConfirmDialogOpen(false);
            setConfirmAction(null);
        }
    }

    async function handleDialogConfirm() {
        if (!selectedProposalId || !editorState.remoteModelId || !confirmAction) {
            return;
        }

        setIsProcessing(true);
        try {
            if (confirmAction === 'accept') {
                await apiClient.acceptModelProposal(editorState.remoteModelId, selectedProposalId, null);
            } else {
                await apiClient.rejectModelProposal(selectedProposalId, editorState.remoteModelId);
            }
            
            // Clear the proposal from the view
            await beamOsEditor.api.clearModelProposals();
            setSelectedProposalId(null);
            setConfirmDialogOpen(false);
            setConfirmAction(null);
        } catch (error) {
            console.error(`Error ${confirmAction}ing proposal:`, error);
        } finally {
            setIsProcessing(false);
        }
    }

    return (
        <>
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
                                    <Box>
                                        <IconButton 
                                            edge="end" 
                                            aria-label="accept"
                                            onClick={handleAcceptClick}
                                            color="success"
                                            size="small"
                                        >
                                            <CheckIcon />
                                        </IconButton>
                                        <IconButton 
                                            edge="end" 
                                            aria-label="reject"
                                            onClick={handleRejectClick}
                                            color="error"
                                            size="small"
                                            sx={{ ml: 1 }}
                                        >
                                            <CloseIcon />
                                        </IconButton>
                                    </Box>
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
            </div>

            <Dialog 
                open={confirmDialogOpen} 
                onClose={handleDialogClose}
                maxWidth="sm"
                fullWidth
            >
                <DialogTitle>
                    {confirmAction === 'accept' ? 'Accept Proposal' : 'Reject Proposal'}
                </DialogTitle>
                <DialogContent>
                    <Typography>
                        {confirmAction === 'accept' 
                            ? 'Are you sure you want to accept this proposal? This will apply all changes to your model.'
                            : 'Are you sure you want to reject this proposal? This will discard all proposed changes.'}
                    </Typography>
                </DialogContent>
                <DialogActions>
                    <Button 
                        onClick={handleDialogClose} 
                        disabled={isProcessing}
                    >
                        Cancel
                    </Button>
                    <Button 
                        onClick={() => void handleDialogConfirm()} 
                        variant="contained"
                        color={confirmAction === 'accept' ? 'primary' : 'error'}
                        disabled={isProcessing}
                        aria-label={confirmAction === 'accept' ? 'accept' : 'reject'}
                    >
                        {isProcessing ? 'Processing...' : (confirmAction === 'accept' ? 'Accept' : 'Reject')}
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
}