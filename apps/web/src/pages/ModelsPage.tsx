import SearchIcon from "@mui/icons-material/Search";
import {
  Box,
  Button,
  Chip,
  InputAdornment,
  Paper,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useEffect } from "react";
import { Link as RouterLink } from "react-router-dom";
import { useAuthStore } from "../store/auth-store";
import { useModelsStore, type ModelRole } from "../store/models-store";

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
    id: "sample-001",
    name: "Tutorial",
    description: "Learn the basics of BeamOS with this interactive tutorial",
    createdAt: "2024-01-01T12:00:00Z",
    badgeLabel: "Sample",
  },
];

const ModelCardsSection = ({
  title,
  models,
}: {
  title: string;
  models: ModelCardItem[];
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
            }}
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
      {!isLoading ? <ModelCardsSection title="My Models" models={userModelCards} /> : null}
      <ModelCardsSection title="Sample Models" models={sampleModelCards} />
    </Stack>
  );
};
