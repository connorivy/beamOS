import type React from 'react';
import { useEffect } from 'react';
import { selectModelsPage, setSearchTerm, showCreateModelDialog, userModelsLoaded } from './modelsPageSlice';
import type { RootState } from '../../../store';
import './ModelsPage.css';
import AppBarMain from '../../components/AppBarMain';
import { useApiClient } from '../api-client/ApiClientContext';
import { useAppDispatch, useAppSelector } from '../../app/hooks';

const ModelsPage: React.FC = () => {
    const apiClient = useApiClient();
    const dispatch = useAppDispatch();
    const {
        isLoading,
        isAuthenticated,
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
        <div className="models-page-container">
            <AppBarMain />
            <div className="header">
                <h1>Models</h1>
                <input
                    type="text"
                    placeholder="Search models..."
                    value={searchTerm}
                    onChange={e => dispatch(setSearchTerm(e.target.value))}
                />
                {isAuthenticated && (
                    <button onClick={() => dispatch(showCreateModelDialog())}>
                        Create Model
                    </button>
                )}
            </div>
            {isLoading ? (
                <div className="loading">Loading...</div>
            ) : (
                <>
                    <h2>My Models</h2>
                    {isAuthenticated ? (
                        filteredModels.length === 0 ? (
                            <div className="no-models">
                                <p>You don't have any models yet.</p>
                                <button onClick={() => dispatch(showCreateModelDialog())}>
                                    Create Your First Model
                                </button>
                            </div>
                        ) : (
                            <div className="models-grid">
                                {filteredModels.map((model) => (
                                    <div key={model.id} className="model-card">
                                        <h3>{model.name}</h3>
                                        <p>{model.description}</p>
                                        <span>{model.role}</span>
                                        <span>{model.lastModified}</span>
                                        <button>
                                            View
                                        </button>
                                    </div>
                                ))}
                            </div>
                        )
                    ) : (
                        <div className="login-prompt">
                            <p>You need to log in to view or modify models.</p>
                            <button>Log In</button>
                        </div>
                    )}
                    <h2>Sample Models</h2>
                    <div className="sample-models-grid">
                        {sampleModels.map((model) => (
                            <div key={model.id} className="model-card">
                                <h3>{model.name}</h3>
                                <p>{model.description}</p>
                                <span>{model.role}</span>
                                <button>
                                    View
                                </button>
                            </div>
                        ))}
                    </div>
                </>
            )}
            {error && <div className="error">{error}</div>}
        </div>
    );
};

export default ModelsPage;
