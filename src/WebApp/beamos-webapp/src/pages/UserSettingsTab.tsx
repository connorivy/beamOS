import { Box, Typography, Divider } from "@mui/material";
import { useAuth } from "../auth/AuthContext";

export default function UserSettingsTab() {
    const { user } = useAuth();
    if (!user) return null;

    return (
        <Box>
            <Typography variant="h5" gutterBottom>User Profile</Typography>
            <Divider sx={{ mb: 2 }} />
            <Typography variant="subtitle1">Email:</Typography>
            <Typography variant="body1" sx={{ mb: 2 }}>{user.email}</Typography>
            {/* Add more user info fields here as needed */}
        </Box>
    );
}
