import {
  AppBar,
  Box,
  Button,
  Container,
  Stack,
  Toolbar,
  Typography,
} from "@mui/material";
import { Link as RouterLink, Outlet, useLocation } from "react-router-dom";
import { useUiStore } from "../store/ui-store";

export const AppLayout = () => {
  const location = useLocation();
  const { mode, setMode } = useUiStore();

  return (
    <Box sx={{ minHeight: "100vh" }}>
      <AppBar position="sticky" color="transparent" elevation={0}>
        <Toolbar>
          <Typography variant="h6" sx={{ fontWeight: 700, flexGrow: 1 }}>
            beamOS
          </Typography>
          <Stack direction="row" spacing={1}>
            <Button
              component={RouterLink}
              to="/"
              variant={location.pathname === "/" ? "contained" : "text"}
            >
              Home
            </Button>
            <Button
              component={RouterLink}
              to="/models"
              variant={location.pathname === "/models" ? "contained" : "text"}
            >
              Models
            </Button>
            <Button variant="outlined" onClick={() => setMode(mode === "light" ? "dark" : "light")}>
              {mode === "light" ? "Dark" : "Light"}
            </Button>
          </Stack>
        </Toolbar>
      </AppBar>
      <Container sx={{ py: 4 }}>
        <Outlet />
      </Container>
    </Box>
  );
};
