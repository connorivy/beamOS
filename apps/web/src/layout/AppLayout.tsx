import {
  AppBar,
  Box,
  Button,
  Container,
  Stack,
  Toolbar,
  Typography,
} from "@mui/material";
import type { ReactNode } from "react";
import { Link as RouterLink, Outlet, useLocation, useNavigate } from "react-router-dom";
import { useAuthStore } from "../store/auth-store";
import { useUiStore } from "../store/ui-store";

type AppLayoutProps = {
  children?: ReactNode;
};

export const AppLayout = ({ children }: AppLayoutProps) => {
  const location = useLocation();
  const isEditorRoute = location.pathname.startsWith("/editor/");
  const navigate = useNavigate();
  const { mode, setMode } = useUiStore();
  const { isAuthenticated, logout } = useAuthStore();

  const handleLogout = () => {
    logout();
    navigate("/");
  };

  if (isEditorRoute) {
    return <Box sx={{ minHeight: "100vh" }}>{children ?? <Outlet />}</Box>;
  }

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
            {isAuthenticated ? (
              <Button variant="outlined" onClick={handleLogout}>
                Log Out
              </Button>
            ) : (
              <Button
                component={RouterLink}
                to="/login"
                variant={location.pathname === "/login" ? "contained" : "outlined"}
              >
                Log In
              </Button>
            )}
            <Button variant="outlined" onClick={() => setMode(mode === "light" ? "dark" : "light")}>
              {mode === "light" ? "Dark" : "Light"}
            </Button>
          </Stack>
        </Toolbar>
      </AppBar>
      <Container sx={{ py: 4 }}>
        {children ?? <Outlet />}
      </Container>
    </Box>
  );
};
