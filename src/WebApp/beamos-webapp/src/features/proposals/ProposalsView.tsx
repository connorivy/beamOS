import { useAppDispatch, useAppSelector } from "../../app/hooks"
import { modelLoaded, modelProposalsLoaded, selectModelResponseByCanvasId } from "../editors/editorsSlice"
import React from "react"
import { Typography, IconButton, Dialog, DialogTitle, DialogContent, DialogActions, Button, Box } from "@mui/material"
import { List, ListItem, ListItemButton, ListItemText } from "@mui/material"
import CheckIcon from '@mui/icons-material/Check'
import CloseIcon from '@mui/icons-material/Close'
import { useApiClient } from "../api-client/ApiClientContext"
import { useEditors } from "../editors/EditorContext"

export function ProposalsView({ canvasId }: { canvasId: string }) {
    // All hooks must be called before any return
    const apiClient = useApiClient();
    const modelResponse = useAppSelector(state =>
        selectModelResponseByCanvasId(state, canvasId)
    );
    const proposalIds: number[] = [
        ...Object.keys(modelResponse?.proposals ?? {}).map(id => Number(id))
    ];
    const dispatch = useAppDispatch();
    const editors = useEditors();
    const beamOsEditor = editors[canvasId];
    const editorState = useAppSelector(state => state.editors[canvasId]);
    const [selectedProposalId, setSelectedProposalId] = React.useState<number | null>(null);
    const [confirmDialogOpen, setConfirmDialogOpen] = React.useState(false);
    const [confirmAction, setConfirmAction] = React.useState<'accept' | 'reject' | null>(null);
    const [isProcessing, setIsProcessing] = React.useState(false);

    // Get proposal names (assuming modelResponse.proposals is an object with id keys)
    const proposalOptions: { id: number; name: string }[] = proposalIds.map(id => ({
        id,
        name: modelResponse?.proposals?.[id]?.name ?? `Proposal ${id}`
    }));

    if (proposalIds.length === 0) {
        return <Typography variant="body1" color="textSecondary">No Model Proposals</Typography>;
    }

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

    async function handleAcceptClick() {
        if (!selectedProposalId || !editorState.remoteModelId) {
            return;
        }
        setIsProcessing(true);
        try {
            var response = await apiClient.acceptModelProposal(editorState.remoteModelId, selectedProposalId, null);
            await beamOsEditor.api.clearModelProposals();
            dispatch(modelLoaded({ canvasId, model: response }));
            setSelectedProposalId(null);
        } catch (error) {
            console.error('Error accepting proposal:', error);
        } finally {
            setIsProcessing(false);
        }
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
                    {proposalOptions.map((option: { id: number; name: string }) => (
                        <ListItem
                            disablePadding
                            key={option.id}
                            secondaryAction={
                                selectedProposalId === option.id ? (
                                    <Box>
                                        <IconButton
                                            edge="end"
                                            aria-label="accept"
                                            onClick={() => void handleAcceptClick()}
                                            color="success"
                                            size="small"
                                            disabled={isProcessing}
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
                                            disabled={isProcessing}
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
                open={confirmDialogOpen && confirmAction === 'reject'}
                onClose={handleDialogClose}
                maxWidth="sm"
                fullWidth
            >
                <DialogTitle>Reject Proposal</DialogTitle>
                <DialogContent>
                    <Typography>
                        Are you sure you want to reject this proposal? This will discard all proposed changes.
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
                        color="error"
                        disabled={isProcessing}
                        aria-label="reject"
                    >
                        {isProcessing ? 'Processing...' : 'Reject'}
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
}