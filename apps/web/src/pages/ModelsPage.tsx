import {
  Chip,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from "@mui/material";
import { useModelsStore, type ModelRole } from "../store/models-store";

const roleColor: Record<ModelRole, "primary" | "secondary" | "default"> = {
  owner: "primary",
  contributor: "secondary",
  reviewer: "default",
};

export const ModelsPage = () => {
  const models = useModelsStore((state) => state.models);

  return (
    <Stack spacing={2}>
      <Typography variant="h4">My Models</Typography>
      <Typography variant="body2" color="text.secondary">
        Models associated with the current user (mocked data for now).
      </Typography>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Description</TableCell>
              <TableCell>Role</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {models.map((model) => (
              <TableRow key={model.id} hover>
                <TableCell>{model.name}</TableCell>
                <TableCell>{model.description}</TableCell>
                <TableCell>
                  <Chip
                    size="small"
                    color={roleColor[model.role]}
                    label={model.role}
                    sx={{ textTransform: "capitalize" }}
                  />
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Stack>
  );
};
