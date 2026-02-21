import HistoryRoundedIcon from "@mui/icons-material/HistoryRounded";
import SearchIcon from "@mui/icons-material/Search";
import {
  Box,
  Button,
  Chip,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  InputAdornment,
  Paper,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import { Link as RouterLink, useNavigate } from "react-router-dom";
import { apiClient } from "../api/client";
import { useAuthStore } from "../store/auth-store";
import { useModelsStore, type ModelRole } from "../store/models-store";
import { kassimaliExample3_8Project } from "../../../../tests/fixtures/Kassimali_MatrixAnalysisOfStructures2ndEd/Kassimali_Example3_8.fixture";

export const TUTORIAL_PROJECT_ID = kassimaliExample3_8Project.project.id;

const roleColor: Record<ModelRole, "primary" | "secondary" | "default"> = {
  owner: "primary",
  contributor: "secondary",
  reviewer: "default",
};

type ModelCardItem = {
  id: string;
  name: string;
  description: string;
  badgeLabel?: string;
  createdAt?: string;
  role?: ModelRole;
};

const sampleModelCards: ModelCardItem[] = [
  {
    id: TUTORIAL_PROJECT_ID,
    name: "Tutorial",
    description: "Learn the basics of BeamOS with this interactive tutorial",
    createdAt: "2024-01-01T12:00:00Z",
    badgeLabel: "Sample",
  },
];

type RevisionHistoryDialogProps = {
  model: ModelCardItem | null;
  onClose: () => void;
};

const RevisionHistoryDialog = ({ model, onClose }: RevisionHistoryDialogProps) => {
  const navigate = useNavigate();
  const loadModels = useModelsStore((state) => state.loadProjects);
  const [isForkLoading, setIsForkLoading] = useState(false);
  const [forkError, setForkError] = useState<string | null>(null);

  const handleFork = async () => {
    if (!model) return;
    setIsForkLoading(true);
    setForkError(null);
    try {
      const { data, error } = await apiClient.POST("/api/projects/{projectId}/fork", {
        params: { path: { projectId: model.id } },
        body: { name: `${model.name} (fork)` },
      });
      if (error || !data) {
        setForkError("Failed to fork the project. Please try again.");
        return;
      }
      await loadModels();
      onClose();
      navigate(`/editor/projects/${data.id}/main`);
    } catch {
      setForkError("An unexpected error occurred.");
    } finally {
      setIsForkLoading(false);
    }
  };

  return (
    <Dialog open={!!model} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>Revision History – {model?.name}</DialogTitle>
      <DialogContent>
        <Stack spacing={2}>
          <Typography variant="body2" color="text.secondary">
            Branch: <strong>main</strong>
          </Typography>
          {forkError ? <Typography color="error">{forkError}</Typography> : null}
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={isForkLoading}>
          Close
        </Button>
        <Button
          variant="contained"
          onClick={() => void handleFork()}
          disabled={isForkLoading}
        >
          {isForkLoading ? "Forking…" : "Fork"}
        </Button>
      </DialogActions>
    </Dialog>
  );
};

const ModelCardsSection = ({
  title,
  models,
  onHistoryClick,
}: {
  title: string;
  models: ModelCardItem[];
  onHistoryClick?: (model: ModelCardItem) => void;
}) => {
  return (
    <Stack spacing={2}>
      <Typography variant="h5" sx={{ fontWeight: 600 }}>
        {title}
      </Typography>
      <Stack spacing={0}>
        {models.map((model) => (
          <Paper
            key={model.id}
            variant="outlined"
            sx={{
              p: 2.5,
              borderRadius: 2,
              bgcolor: "background.paper",
              display: "block",
            }}
          >
            <Stack direction="row" alignItems="flex-start" justifyContent="space-between">
              <Box
                component={RouterLink}
                to={`/editor/projects/${model.id}/main`}
                sx={{ textDecoration: "none", color: "inherit", flex: 1 }}
              >
                <Stack spacing={1.25}>
                  <Typography variant="h5" sx={{ fontWeight: 600 }}>
                    {model.name}
                  </Typography>
                  <Typography color="text.secondary">{model.description}</Typography>
                  {model.role ? (
                    <Chip
                      size="small"
                      color={roleColor[model.role]}
                      label={model.role}
                      sx={{ textTransform: "capitalize", width: "fit-content" }}
                    />
                  ) : null}
                  {model.badgeLabel ? (
                    <Typography sx={{ fontStyle: "italic", fontSize: "0.9rem" }}>
                      {model.badgeLabel}
                    </Typography>
                  ) : null}
                  {model.createdAt ? (
                    <Typography variant="body2" color="text.secondary">
                      {model.createdAt}
                    </Typography>
                  ) : null}
                </Stack>
              </Box>
              {onHistoryClick ? (
                <IconButton
                  size="small"
                  aria-label="revision history"
                  onClick={(e) => {
                    e.preventDefault();
                    onHistoryClick(model);
                  }}
                  sx={{ ml: 1, mt: 0.5 }}
                >
                  <HistoryRoundedIcon fontSize="small" />
                </IconButton>
              ) : null}
            </Stack>
          </Paper>
        ))}
      </Stack>
    </Stack>
  );
};

const UnauthenticatedModelsView = () => {
  return (
    <Stack spacing={5}>
      <Stack
        direction={{ xs: "column", sm: "row" }}
        justifyContent="space-between"
        alignItems={{ xs: "stretch", sm: "center" }}
        spacing={2}
      >
        <Typography variant="h3" sx={{ fontWeight: 600 }}>
          Models
        </Typography>
        <TextField
          placeholder="Search models..."
          size="small"
          sx={{ width: { xs: "100%", sm: 260 } }}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <SearchIcon fontSize="small" />
              </InputAdornment>
            ),
          }}
        />
      </Stack>

      <Stack spacing={2}>
        <Typography variant="h5" sx={{ fontWeight: 600 }}>
          My Models
        </Typography>
        <Box
          sx={{
            minHeight: 180,
            display: "grid",
            placeItems: "center",
          }}
        >
          <Stack spacing={2} alignItems="center">
            <Typography color="text.secondary">
              You need to log in to view or modify models.
            </Typography>
            <Button component={RouterLink} to="/login" variant="outlined">
              Log In
            </Button>
          </Stack>
        </Box>
      </Stack>
    </Stack>
  );
};

export const ModelsPage = () => {
  const models = useModelsStore((state) => state.models);
  const isLoading = useModelsStore((state) => state.isLoading);
  const error = useModelsStore((state) => state.error);
  const loadModels = useModelsStore((state) => state.loadProjects);
  const isAuthenticated = useAuthStore((state) => state.isAuthenticated);
  const [historyModel, setHistoryModel] = useState<ModelCardItem | null>(null);

  const userModelCards: ModelCardItem[] = models.map((model) => ({
    id: model.id,
    name: model.name,
    description: model.description,
    role: model.role,
  }));

  useEffect(() => {
    if (!isAuthenticated) {
      return;
    }
    void loadModels();
  }, [isAuthenticated, loadModels]);

  if (!isAuthenticated) {
    return (
      <Stack spacing={5}>
        <UnauthenticatedModelsView />
        <ModelCardsSection title="Sample Models" models={sampleModelCards} />
      </Stack>
    );
  }

  return (
    <Stack spacing={5}>
      {isLoading ? <Typography color="text.secondary">Loading models...</Typography> : null}
      {error ? <Typography color="error">{error}</Typography> : null}
      {!isLoading ? (
        <ModelCardsSection
          title="My Models"
          models={userModelCards}
          onHistoryClick={setHistoryModel}
        />
      ) : null}
      <ModelCardsSection title="Sample Models" models={sampleModelCards} />
      <RevisionHistoryDialog
        model={historyModel}
        onClose={() => setHistoryModel(null)}
      />
    </Stack>
  );
};

