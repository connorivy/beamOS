
import { Box, Button, Typography } from "@mui/material";
import GoogleIcon from "@mui/icons-material/Google";
import type { JSX } from "react";
// import { useIdentityApiClient } from "../api-client/ApiClientContext";

// Read backend URL from Vite environment variable, explicitly type as string | undefined
const identityBackendUrl = import.meta.env.VITE_IDENTITY_BACKEND_URL as string | undefined;
const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID as string | undefined;

export const LoginPage = (): JSX.Element => {
  // const apiClient = useIdentityApiClient();
  // You can now use backendUrl in your logic
  // Example: console.log("Backend URL:", backendUrl);

  return (
    <Box sx={{ display: "flex", flexDirection: "column", alignItems: "center", justifyContent: "center", minHeight: "60vh" }}>
      <Typography variant="h4" sx={{ mb: 4, fontWeight: 700 }}>Login</Typography>
      <Button
        variant="contained"
        color="primary"
        startIcon={<GoogleIcon />}
        sx={{ fontWeight: 700, px: 4, py: 1.5 }}
        onClick={() => {
          if (!identityBackendUrl) {
            alert("Identity backend URL is not configured. Please contact support.");
            return;
          }
          if (!googleClientId) {
            alert("Google client ID is not configured. Please contact support.");
            return;
          }
          // Generate a random state string
          const state = crypto.randomUUID().replace(/-/g, "");
          // Build the redirect URI (backend endpoint)
          const redirectUri = `${identityBackendUrl}/api/login-with-google-code`;
          // Build the Google OAuth URL
          const googleAuthUrl =
            "https://accounts.google.com/o/oauth2/v2/auth?" +
            `client_id=${encodeURIComponent(googleClientId)}&` +
            `redirect_uri=${encodeURIComponent(redirectUri)}&` +
            "response_type=code&" +
            "scope=openid%20profile%20email&" +
            `state=${encodeURIComponent(state)}`;

          // Redirect the user
          window.location.href = googleAuthUrl;
        }}
      >
        Login with Google
      </Button>
    </Box>
  );
}
