import { useEffect, useState } from "react";
import {
    Box,
    Typography,
    Divider,
    Button,
    TextField,
    Chip,
    IconButton,
    Stack,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    FormGroup,
    FormControlLabel,
    Checkbox,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Tooltip,
    Snackbar,
    CircularProgress,
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import { useIdentityApiClient } from "../features/api-client/ApiClientContext";
import type { IApiTokenResponse } from "../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/IdentityApiClientV1";

// Dummy scopes for demonstration
const SCOPES = [
    { label: "models:read", value: "models:read" },
    { label: "models:propose", value: "models:propose" },
    { label: "models:write", value: "models:write" },
];

export default function DeveloperSettingsTab() {
    const identityApiClient = useIdentityApiClient();

    const [tokens, setTokens] = useState<IApiTokenResponse[]>([]);
    const [createDialogOpen, setCreateDialogOpen] = useState(false);
    const [newName, setNewName] = useState("");
    const [newScopes, setNewScopes] = useState<string[]>([]);
    const [showToken, setShowToken] = useState<string | null>(null);
    const [copySnackbar, setCopySnackbar] = useState(false);
    const [deleteIdx, setDeleteIdx] = useState<number | null>(null);
    const [tokensLoading, setTokensLoading] = useState(true);

    useEffect(() => {
        const fetchTokens = async () => {
            setTokensLoading(true);
            try {
                const result = await identityApiClient.getUserApiTokens();
                setTokens(result);
            } catch {
                // handle error if needed
            } finally {
                setTokensLoading(false);
            }
        };
        void fetchTokens();
    }, [identityApiClient]);

    const handleDeleteRequest = (idx: number) => {
        setDeleteIdx(idx);
    };

    const handleDeleteConfirm = () => {
        if (deleteIdx !== null) {
            setTokens(tokens.filter((_, i) => i !== deleteIdx));
            setDeleteIdx(null);
        }
    };

    const handleDeleteCancel = () => {
        setDeleteIdx(null);
    };

    const handleScopeChange = (scope: string) => {
        setNewScopes(prev =>
            prev.includes(scope) ? prev.filter(s => s !== scope) : [...prev, scope]
        );
    };

    const handleCreate = () => {
        if (!newName || newScopes.length === 0) return;
        // Simulate token value
        const generatedToken = Math.random().toString(36).substring(2, 18);
        setTokens([
            ...tokens,
            { name: newName, scopes: newScopes, createdOn: new Date(), value: "" },
        ]);
        setShowToken(generatedToken);
        setNewName("");
        setNewScopes([]);
    };

    const handleDialogClose = () => {
        setCreateDialogOpen(false);
        setShowToken(null);
        setNewName("");
        setNewScopes([]);
    };

    const handleCopy = async () => {
        if (showToken) {
            try {
                await navigator.clipboard.writeText(showToken);
                setCopySnackbar(true);
            } catch {
                // fallback or error handling could go here
            }
        }
    };

    return (
        <Box>
            <Typography variant="h5" gutterBottom>API Tokens</Typography>
            <Divider sx={{ mb: 2 }} />
            <Box sx={{ display: "flex", justifyContent: "flex-end", mb: 2 }}>
                <Button variant="contained" onClick={() => { setCreateDialogOpen(true); }}>
                    Create Token
                </Button>
            </Box>
            <TableContainer component={Paper} variant="outlined">
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Name</TableCell>
                            <TableCell>Scopes</TableCell>
                            <TableCell>Created On</TableCell>
                            <TableCell align="right">Actions</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {tokensLoading ? (
                            <TableRow>
                                <TableCell colSpan={4} align="center">
                                    <CircularProgress size={32} />
                                </TableCell>
                            </TableRow>
                        ) : (
                            tokens.map((token, idx) => (
                                <TableRow key={token.name}>
                                    <TableCell>{token.name}</TableCell>
                                    <TableCell>
                                        <Stack direction="row" spacing={1}>
                                            {token.scopes.map(scope => (
                                                <Chip key={scope} label={scope} size="small" color="default" variant="outlined" />
                                            ))}
                                        </Stack>
                                    </TableCell>
                                    <TableCell>{token.created}</TableCell>
                                    <TableCell align="right">
                                        <Tooltip title="Delete">
                                            <IconButton color="error" onClick={() => { handleDeleteRequest(idx); }}>
                                                <DeleteIcon />
                                            </IconButton>
                                        </Tooltip>
                                    </TableCell>

                                    {/* Delete Confirmation Dialog */}
                                    <Dialog open={deleteIdx !== null} onClose={handleDeleteCancel}>
                                        <DialogTitle>Delete API Token?</DialogTitle>
                                        <DialogContent>
                                            <Typography gutterBottom>
                                                Are you sure you want to delete this API token? This action is <b>permanent</b> and cannot be undone.
                                            </Typography>
                                        </DialogContent>
                                        <DialogActions>
                                            <Button onClick={handleDeleteCancel}>Cancel</Button>
                                            <Button color="error" variant="contained" onClick={handleDeleteConfirm} autoFocus>
                                                Delete
                                            </Button>
                                        </DialogActions>
                                    </Dialog>
                                </TableRow>
                            ))
                        )}
                    </TableBody>
                </Table>
            </TableContainer>

            {/* Create Token Dialog */}
            <Dialog open={createDialogOpen} onClose={handleDialogClose}>
                <DialogTitle>Create New Token</DialogTitle>
                <DialogContent>
                    {showToken ? (
                        <Box sx={{ mt: 2, mb: 2 }}>
                            <Typography variant="subtitle1" gutterBottom>New Token</Typography>
                            <Paper variant="outlined" sx={{ p: 2, display: 'flex', alignItems: 'center', mb: 2 }}>
                                <Typography sx={{ wordBreak: 'break-all', mr: 2 }}>{showToken}</Typography>
                                <Tooltip title="Copy">
                                    <IconButton onClick={() => { void handleCopy(); }} color="primary">
                                        <ContentCopyIcon />
                                    </IconButton>
                                </Tooltip>
                            </Paper>
                            <Typography color="warning.main" sx={{ mb: 1 }}>
                                This is the <b>last time</b> you will see this token. Please copy and store it securely.
                            </Typography>
                            <Typography color="text.secondary" variant="body2">
                                Treat this token like a password. Do not share it or expose it in public places.
                            </Typography>
                        </Box>
                    ) : (
                        <Box sx={{ mt: 1 }}>
                            <TextField
                                label="Token Name"
                                value={newName}
                                onChange={e => { setNewName(e.target.value); }}
                                size="small"
                                fullWidth
                                sx={{ mb: 2 }}
                            />
                            <FormGroup row>
                                {SCOPES.map(scope => (
                                    <FormControlLabel
                                        key={scope.value}
                                        control={
                                            <Checkbox
                                                checked={newScopes.includes(scope.value)}
                                                onChange={() => { handleScopeChange(scope.value); }}
                                            />
                                        }
                                        label={scope.label}
                                    />
                                ))}
                            </FormGroup>
                        </Box>
                    )}
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleDialogClose}>Close</Button>
                    {!showToken && (
                        <Button variant="contained" onClick={handleCreate} disabled={!newName || newScopes.length === 0}>
                            Create
                        </Button>
                    )}
                </DialogActions>
            </Dialog>
            <Snackbar
                open={copySnackbar}
                autoHideDuration={2000}
                onClose={() => { setCopySnackbar(false); }}
                message="Token copied to clipboard!"
                anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
            />
        </Box>
    );
}
