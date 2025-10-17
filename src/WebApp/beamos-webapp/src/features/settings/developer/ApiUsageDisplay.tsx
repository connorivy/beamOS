import { Box, Typography, Paper, Divider, Stack, Card, CardContent, Avatar, Chip, Tooltip } from "@mui/material";
import AccessTimeIcon from '@mui/icons-material/AccessTime';
import CallMadeIcon from '@mui/icons-material/CallMade';
import VpnKeyIcon from '@mui/icons-material/VpnKey';
import type { IApiUsageResponse, UsageBreakdownResponse } from "../../../../../../../codeGen/BeamOs.CodeGen.StructuralAnalysisApiClient/IdentityApiClientV1";
import { useEffect, useState } from "react";
import { useIdentityApiClient } from "../../api-client/ApiClientContext";

const ApiUsageDisplay = () => {
    const identityApiClient = useIdentityApiClient();
    const [apiUsage, setApiUsage] = useState<IApiUsageResponse>({
        totalCalls: 0,
        totalDurationMs: 0,
        breakdown: [],
    });

    useEffect(() => {
        const fetchUsage = async () => {
            try {
                const usageData = await identityApiClient.getApiUsage();
                setApiUsage(usageData);
            } catch (error) {
                console.error("Failed to fetch API usage data:", error);
            }
        };
        void fetchUsage();
    }, []);

    // Convert ms to seconds for display
    const totalSeconds = Math.round((apiUsage.totalDurationMs || 0) / 1000);
    const breakdown = apiUsage.breakdown ?? [];

    return (
        <Box mt={4}>
            <Typography variant="h5" gutterBottom>API Usage</Typography>
            <Divider sx={{ mb: 3 }} />
            <Paper elevation={3} sx={{ p: 3 }}>
                <Stack direction={{ xs: 'column', sm: 'row' }} spacing={4} alignItems="center" justifyContent="space-between" mb={2}>
                    <Stack direction="row" spacing={2} alignItems="center">
                        <Avatar sx={{ width: 48, height: 48 }}>
                            <CallMadeIcon fontSize="large" />
                        </Avatar>
                        <Box>
                            <Typography variant="subtitle2" color="text.secondary">Total API Calls</Typography>
                            <Typography variant="h6" sx={{ fontWeight: 600 }}>{apiUsage.totalCalls}</Typography>
                        </Box>
                    </Stack>
                    <Stack direction="row" spacing={2} alignItems="center">
                        <Avatar sx={{ width: 48, height: 48 }}>
                            <AccessTimeIcon fontSize="large" />
                        </Avatar>
                        <Box>
                            <Typography variant="subtitle2" color="text.secondary">Total Usage (seconds)</Typography>
                            <Typography variant="h6" sx={{ fontWeight: 600 }}>{totalSeconds}</Typography>
                        </Box>
                    </Stack>
                </Stack>
                <Divider sx={{ my: 2 }} />
                <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 2 }}>Breakdown by Token</Typography>
                <Stack spacing={2}>
                    {breakdown.length === 0 && (
                        <Typography variant="body2">No token usage data available.</Typography>
                    )}
                    {breakdown.map((tokenUsage: UsageBreakdownResponse, idx: number) => (
                        tokenUsage.isToken ? (
                            <Card key={`${tokenUsage.actorName}-${idx.toString()}`} variant="outlined" sx={{ borderRadius: 2 }}>
                                <CardContent>
                                    <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems="center" justifyContent="space-between">
                                        <Stack direction="row" spacing={2} alignItems="center">
                                            <Avatar>
                                                <VpnKeyIcon />
                                            </Avatar>
                                            <Box>
                                                <Tooltip title="Token Name" arrow>
                                                    <Chip label={tokenUsage.actorName} variant="outlined" size="small" sx={{ fontWeight: 600, fontSize: 14 }} />
                                                </Tooltip>
                                            </Box>
                                        </Stack>
                                        <Stack direction="row" spacing={3} alignItems="center">
                                            <Stack direction="row" spacing={1} alignItems="center">
                                                <CallMadeIcon />
                                                <Typography variant="body2" sx={{ fontWeight: 500 }}>{tokenUsage.numCalls} Calls</Typography>
                                            </Stack>
                                            <Stack direction="row" spacing={1} alignItems="center">
                                                <AccessTimeIcon />
                                                <Typography variant="body2" sx={{ fontWeight: 500 }}>{Math.round((tokenUsage.totalDurationMs || 0) / 1000)} sec</Typography>
                                            </Stack>
                                        </Stack>
                                    </Stack>
                                </CardContent>
                            </Card>
                        ) : null
                    ))}
                </Stack>
            </Paper>
        </Box>
    );
};

export default ApiUsageDisplay;
