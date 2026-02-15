import GoogleIcon from "@mui/icons-material/Google";
import { Box, Button, Paper, Stack, Typography } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useAuthStore } from "../store/auth-store";

export const LoginPage = () => {
  const navigate = useNavigate();
  const loginWithGoogle = useAuthStore((state) => state.loginWithGoogle);

  const handleGoogleLogin = () => {
    loginWithGoogle();
    navigate("/models");
  };

  return (
    <Box sx={{ minHeight: "70vh", display: "grid", placeItems: "center" }}>
      <Paper
        variant="outlined"
        sx={{
          width: "100%",
          maxWidth: 420,
          px: { xs: 3, sm: 4 },
          py: 4,
          bgcolor: "background.paper",
        }}
      >
        <Stack spacing={3} alignItems="stretch">
          <Stack spacing={1}>
            <Typography variant="h4">Log In</Typography>
            <Typography color="text.secondary">
              Sign in to access and manage your models.
            </Typography>
          </Stack>
          <Button
            variant="contained"
            size="large"
            startIcon={<GoogleIcon />}
            onClick={handleGoogleLogin}
          >
            Continue with Google
          </Button>
        </Stack>
      </Paper>
    </Box>
  );
};
