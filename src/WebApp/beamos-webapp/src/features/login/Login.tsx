import { Box, Button, Typography } from "@mui/material";
import GoogleIcon from "@mui/icons-material/Google";
import type { JSX } from "react"

export const LoginPage = (): JSX.Element => {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", alignItems: "center", justifyContent: "center", minHeight: "60vh" }}>
      <Typography variant="h4" sx={{ mb: 4, fontWeight: 700 }}>Login</Typography>
      <Button
        variant="contained"
        color="primary"
  startIcon={<GoogleIcon />}
        sx={{ fontWeight: 700, px: 4, py: 1.5 }}
        onClick={() => window.location.href = "/api/auth/google"}
      >
        Login with Google
      </Button>
    </Box>
  );
}
