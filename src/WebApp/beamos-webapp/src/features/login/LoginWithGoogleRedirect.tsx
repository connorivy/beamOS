
import { useEffect, useLayoutEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { Typography, Box } from "@mui/material";
import { useIdentityApiClient } from "../api-client/ApiClientContext";

// Read backend URL from Vite environment variable
const identityBackendUrl = import.meta.env.VITE_IDENTITY_BACKEND_URL as string | undefined;

// Helper to get query params
function useQuery() {
    return new URLSearchParams(useLocation().search);
}

const LoginWithGoogleRedirect: React.FC = () => {
    const query = useQuery();
    const navigate = useNavigate();
    const [token, setToken] = useState<string | null>(null);
    const apiClient = useIdentityApiClient();

    useEffect(() => {
        const code = query.get("code");
        if (!identityBackendUrl) {
            alert("Identity backend URL is not configured. Please contact support.");
            return;
        }
        if (!code) {
            alert("No code found in URL. Please try logging in again.");
            void navigate("/login");
            return;
        }

        const fetchAccessToken = async (code: string) => {
            try {
                const response = await apiClient.loginWithGoogleCode(code);
                setToken(response);
            }
            catch {
                setToken(null);
            }
        };

        void fetchAccessToken(code);
        void navigate("/");
    }, []);

    return (
        <Box sx={{ display: "flex", flexDirection: "column", alignItems: "center", justifyContent: "center", minHeight: "60vh" }}>
            <Typography variant="h5">Logging in...</Typography>
        </Box>
    );
};
