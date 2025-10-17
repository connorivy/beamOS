import type React from 'react';
import { useEffect } from 'react';
import { selectModelsPage, setSearchTerm, showCreateModelDialog, userModelsLoaded } from './modelsPageSlice';
import type { RootState } from '../../../store';
import AppBarMain from '../../components/AppBarMain';
import { useApiClient } from '../api-client/ApiClientContext';
import { useAppDispatch, useAppSelector } from '../../app/hooks';
import { useAuth } from '../../auth/AuthContext';
import Container from '@mui/material/Container';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Card from '@mui/material/Card';
import CardContent from '@mui/material/CardContent';
import Typography from '@mui/material/Typography';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';

const ModelsPage: React.FC = () => {
    const apiClient = useApiClient();
    const { user } = useAuth();
    const dispatch = useAppDispatch();
    const {
        isLoading,
        models,
        sampleModels,
        searchTerm,
        error,
    } = useAppSelector((state: RootState) => selectModelsPage(state));

    useEffect(() => {
        const fetchData = async () => {
            try {
                const models = await apiClient.getModels();
                dispatch(userModelsLoaded(models));
            } catch (error) {
                console.error('Error fetching models:', error);
            }
        };
        void fetchData();
    }, [apiClient, dispatch]);

    // Filter models by search term
    const filteredModels = models.filter((model) =>
        model.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    return (
        <Box minHeight="100vh" display="flex" flexDirection="column" alignItems="center" width="100%">
            <AppBarMain />
            <Container maxWidth="lg" sx={{ mt: 6, mb: 4 }}>
                <Box display="flex" flexDirection={{ xs: 'column', md: 'row' }} alignItems="center" justifyContent="space-between" mb={4} gap={2}>
                    <Typography variant="h4" fontWeight={700}>Models</Typography>
                    <Box display="flex" flexDirection="row" alignItems="center" gap={2} width={{ xs: '100%', md: 'auto' }}>
                        <TextField
                            variant="outlined"
                            size="small"
                            placeholder="Search models..."
                            value={searchTerm}
                            onChange={e => dispatch(setSearchTerm(e.target.value))}
                            sx={{ flex: 1, minWidth: 180 }}
                        />
                        {user && (
                            <Button variant="outlined" onClick={() => dispatch(showCreateModelDialog())}>
                                Create Model
                            </Button>
                        )}
                    </Box>
                </Box>
                {isLoading ? (
                    <Typography align="center" mt={8} variant="h6">Loading...</Typography>
                ) : (
                    <>
                        <Typography variant="h6" fontWeight={600} mt={6} mb={2}>My Models</Typography>
                        {user ? (
                            filteredModels.length === 0 ? (
                                <Box display="flex" flexDirection="column" alignItems="center" gap={2} my={6}>
                                    <Typography>You don't have any models yet.</Typography>
                                    <Button variant="outlined" onClick={() => dispatch(showCreateModelDialog())}>
                                        Create Your First Model
                                    </Button>
                                </Box>
                            ) : (
                                <Grid columns={12} spacing={3} maxWidth="lg">
                                    {filteredModels.map((model) => (
                                        <Grid key={model.id} size={{ xs: 12, md: 6, lg: 4 }}>
                                            <Card variant="outlined">
                                                <CardContent>
                                                    <Typography variant="h6" fontWeight={700}>{model.name}</Typography>
                                                    <Typography variant="body2" mb={1}>{model.description}</Typography>
                                                    <Typography variant="caption" fontStyle="italic">{model.role}</Typography><br />
                                                    <Typography variant="caption">{model.lastModified}</Typography>
                                                    <Box display="flex" justifyContent="flex-end" mt={2}>
                                                        <Button variant="outlined" size="small">View</Button>
                                                    </Box>
                                                </CardContent>
                                            </Card>
                                        </Grid>
                                    ))}
                                </Grid>
                            )
                        ) : (
                            <Box display="flex" flexDirection="column" alignItems="center" gap={2} my={6}>
                                <Typography>You need to log in to view or modify models.</Typography>
                                <Button variant="outlined">Log In</Button>
                            </Box>
                        )}
                        <Typography variant="h6" fontWeight={600} mt={8} mb={2}>Sample Models</Typography>
                        <Grid columns={12} spacing={3} maxWidth="lg">
                            {sampleModels.map((model) => (
                                <Grid key={model.id} size={{ xs: 12, md: 6, lg: 4 }}>
                                    <Card variant="outlined">
                                        <CardContent>
                                            <Typography variant="h6" fontWeight={700}>{model.name}</Typography>
                                            <Typography variant="body2" mb={1}>{model.description}</Typography>
                                            <Typography variant="caption" fontStyle="italic">{model.role}</Typography>
                                            <Box display="flex" justifyContent="flex-end" mt={2}>
                                                <Button variant="outlined" size="small">View</Button>
                                            </Box>
                                        </CardContent>
                                    </Card>
                                </Grid>
                            ))}
                        </Grid>
                    </>
                )}
                {error && <Typography color="error" mt={4} align="center">{error}</Typography>}
            </Container>
        </Box>
    );
};

export default ModelsPage;
