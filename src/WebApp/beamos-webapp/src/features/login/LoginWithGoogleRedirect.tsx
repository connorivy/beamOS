import { useEffect } from "react";
import { useLocation, useNavigate } from "react-router";
import { Typography, Box } from "@mui/material";
import { useIdentityApiClient } from "../api-client/ApiClientContext";
import { useAuth } from "../../auth/AuthContext";

// Helper to get query params
function useQuery() {
    return new URLSearchParams(useLocation().search);
}

// function parseJwt(token: string): any {
//     try {
//         const base64Url = token.split('.')[1];
//         const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
//         const jsonPayload = decodeURIComponent(
//             atob(base64)
//                 .split('')
//                 .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
//                 .join('')
//         );
//         return JSON.parse(jsonPayload);
//     } catch {
//         return null;
//     }
// }

const LoginWithGoogleRedirect: React.FC = () => {
    const query = useQuery();
    const navigate = useNavigate();
    const apiClient = useIdentityApiClient();
    const { login } = useAuth();

    useEffect(() => {
        const code = query.get("code");
        if (!code) {
            alert("No code found in URL. Please try logging in again.");
            void navigate("/login");
            return;
        }

        const fetchAccessToken = async (code: string) => {
            try {
                const response = await apiClient.loginWithGoogleCode(code);
                login({ token: response });
            }
            catch {
                login({ token: null });
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

export default LoginWithGoogleRedirect;
