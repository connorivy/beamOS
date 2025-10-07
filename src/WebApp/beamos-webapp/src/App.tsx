
import AppBar from "@mui/material/AppBar";
import Box from "@mui/material/Box";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import Container from "@mui/material/Container";
import Grid from "@mui/material/Grid";
import Card from "@mui/material/Card";
import CardContent from "@mui/material/CardContent";
import IconButton from "@mui/material/IconButton";
import GitHubIcon from "@mui/icons-material/GitHub";
import LinkedInIcon from "@mui/icons-material/LinkedIn";
import { Routes, Route, useNavigate } from "react-router";
import { LoginPage } from "./features/login/Login";

export const App = () => {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="*" element={<HomePage />} />
    </Routes>
  );
};

function HomePage() {
  const navigate = useNavigate();
  return (
    <Box sx={{ minHeight: "100vh" }}>
      {/* Navbar */}
      <AppBar position="static" sx={{ boxShadow: 0 }}>
        <Toolbar sx={{ justifyContent: "space-between" }}>
          <Box sx={{ display: "flex", alignItems: "center" }}>
            <img src="https://raw.githubusercontent.com/Loopple/loopple-public-assets/main/motion-tailwind/img/logos/logo-1.png" alt="beamOS logo" style={{ height: 32, marginRight: 12 }} />
            <Typography variant="h6" sx={{ fontWeight: 700, letterSpacing: 2 }}>
              beamOS
            </Typography>
          </Box>
          <Box>
            <Button color="inherit" href="/reliability" sx={{ textTransform: "none", fontWeight: 600 }}>
              Reliability
            </Button>
            <Button color="inherit" href="/models" sx={{ textTransform: "none", fontWeight: 600 }}>
              Models
            </Button>
            <Button color="inherit" sx={{ textTransform: "none", fontWeight: 600 }} onClick={() => { navigate("/login"); }}>Login</Button>
            <IconButton color="inherit" href="https://github.com/connorivy/beamOS" target="_blank">
              <GitHubIcon />
            </IconButton>
            <IconButton color="inherit" href="https://www.linkedin.com/in/connor-ivy-15a601183/" target="_blank">
              <LinkedInIcon />
            </IconButton>
          </Box>
        </Toolbar>
      </AppBar>
      {/* Hero Section */}
      <Container maxWidth="lg" sx={{ pt: { xs: 8, md: 12 }, mb: 0 }}>
        <Grid container spacing={6} columns={12} sx={{ alignItems: "center", mb: 8 }}>
          <Grid size={{ xs: 12, md: 6 }}>
            <Typography variant="h2" sx={{ fontWeight: 900, mb: 3, fontSize: { xs: 36, md: 56 } }}>
              A better way to design structures
            </Typography>
            <Typography variant="h6" sx={{ mb: 4 }}>
              Powerful, open-source, and cloud-based structural analysis software and APIs. <b>beamOS</b> is enabling safer and more efficient designs.
            </Typography>
            <Button variant="contained" color="primary" href="/models" sx={{ mr: 2, fontWeight: 700, px: 4, py: 1.5 }}>
              Get Started
            </Button>
            <Button variant="outlined" color="secondary" href="/reliability" sx={{ fontWeight: 700, px: 4, py: 1.5 }}>
              Learn about reliability
            </Button>
          </Grid>
          <Grid size={{ xs: 12, md: 6 }} sx={{ display: { xs: "none", md: "flex" }, justifyContent: "flex-end" }}>
            <Box component="img" src="./images/Designer.png" alt="header image" sx={{ width: "80%", borderRadius: 3, boxShadow: 3 }} />
          </Grid>
        </Grid>

        {/* Features Section */}
        <Grid container spacing={4} columns={12} sx={{ justifyContent: "center", alignItems: "stretch" }}>
          <Grid size={{ xs: 12, md: 4 }} sx={{ display: 'flex' }}>
            <Card sx={{ borderRadius: 3, boxShadow: 2, minHeight: 260, flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
              <CardContent sx={{ display: "flex", flexDirection: "column", alignItems: "center", gap: 2 }}>
                <Box sx={{ bgcolor: "#6366f1", borderRadius: 2, width: 64, height: 64, display: "flex", alignItems: "center", justifyContent: "center", mb: 2 }}>
                  {/* Open Source Icon */}
                  <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 25 24" fill="none"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" stroke="white" d="M12 6.042A8.967 8.967 0 0 0 6 3.75c-1.052 0-2.062.18-3 .512v14.25A8.987 8.987 0 0 1 6 18c2.305 0 4.408.867 6 2.292m0-14.25a8.966 8.966 0 0 1 6-2.292c1.052 0 2.062.18 3 .512v14.25A8.987 8.987 0 0 0 18 18a8.967 8.967 0 0 0-6 2.292m0-14.25v14.25"/></svg>
                </Box>
                <Typography variant="h5" sx={{ fontWeight: 800, mb: 1 }}>Open Source</Typography>
                <Typography align="center">
                  No more outdated, black box structural analysis programs. <b>beamOS</b> source code is free and open to the public.
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ xs: 12, md: 4 }} sx={{ display: 'flex' }}>
            <Card sx={{ borderRadius: 3, boxShadow: 2, minHeight: 260, flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
              <CardContent sx={{ display: "flex", flexDirection: "column", alignItems: "center", gap: 2 }}>
                <Box sx={{ bgcolor: "#6366f1", borderRadius: 2, width: 64, height: 64, display: "flex", alignItems: "center", justifyContent: "center", mb: 2 }}>
                  {/* Robust Code Testing Icon */}
                  <svg className="h-8 w-8" width="32" height="32" viewBox="0 0 24 24" strokeWidth="2" stroke="white" fill="none" strokeLinecap="round" strokeLinejoin="round"><line x1="9" y1="3" x2="15" y2="3" /><line x1="10" y1="9" x2="14" y2="9" /><path d="M10 3v6l-4 11a.7 .7 0 0 0 .5 1h11a.7 .7 0 0 0 .5 -1l-4 -11v-6"/></svg>
                </Box>
                <Typography variant="h5" sx={{ fontWeight: 800, mb: 1 }}>Robust Code Testing</Typography>
                <Typography align="center">
                  Code is tested against many solved problems from textbooks, research papers, and other structural analysis programs to make sure results are always accurate.
                </Typography>
              </CardContent>
            </Card>
          </Grid>
          <Grid size={{ xs: 12, md: 4 }} sx={{ display: 'flex' }}>
            <Card sx={{ borderRadius: 3, boxShadow: 2, minHeight: 260, flex: 1, display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center' }}>
              <CardContent sx={{ display: "flex", flexDirection: "column", alignItems: "center", gap: 2 }}>
                <Box sx={{ bgcolor: "#6366f1", borderRadius: 2, width: 64, height: 64, display: "flex", alignItems: "center", justifyContent: "center", mb: 2 }}>
                  {/* API Access Icon */}
                  <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 25 24" fill="none"><path d="M3.29 14.78L3.2 14.69C2.81 14.3 2.81 13.67 3.2 13.28L9.29 7.18C9.68 6.79 10.31 6.79 10.7 7.18L13.99 10.47L20.38 3.29C20.76 2.86 21.43 2.85 21.83 3.25C22.2 3.63 22.22 4.23 21.87 4.62L14.7 12.69C14.32 13.12 13.66 13.14 13.25 12.73L10 9.48L4.7 14.78C4.32 15.17 3.68 15.17 3.29 14.78ZM4.7 20.78L10 15.48L13.25 18.73C13.66 19.14 14.32 19.12 14.7 18.69L21.87 10.62C22.22 10.23 22.2 9.63 21.83 9.25C21.43 8.85 20.76 8.86 20.38 9.29L13.99 16.47L10.7 13.18C10.31 12.79 9.68 12.79 9.29 13.18L3.2 19.28C2.81 19.67 2.81 20.3 3.2 20.69L3.29 20.78C3.68 21.17 4.32 21.17 4.7 20.78Z" fill="white"></path></svg>
                </Box>
                <Typography variant="h5" sx={{ fontWeight: 800, mb: 1 }}>Data Access Through APIs</Typography>
                <Typography align="center">
                  Data is easily retrievable through cloud-based APIs instead of locked in proprietary file formats. You own your data. In the age of AI, data is your most valuable asset.
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </Grid>
      </Container>

      {/* Footer */}
      <Box sx={{ py: 6, mt: 0 }}>
        <Container maxWidth="md" sx={{ display: "flex", flexDirection: "column", alignItems: "center" }}>
          <Box sx={{ display: "flex", gap: 2, mb: 2 }}>
            <IconButton color="inherit" href="https://github.com/connorivy/beamOS" target="_blank">
              <GitHubIcon />
            </IconButton>
            <IconButton color="inherit" href="https://www.linkedin.com/in/connor-ivy-15a601183/" target="_blank">
              <LinkedInIcon />
            </IconButton>
          </Box>
          <Typography variant="body2" align="center">
            © {new Date().getFullYear()} beamOS. All rights reserved.
          </Typography>
        </Container>
      </Box>

    </Box>
  );
}


