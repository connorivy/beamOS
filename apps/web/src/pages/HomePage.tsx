import {
  Avatar,
  Box,
  Button,
  Card,
  CardContent,
  Grid,
  Link,
  Paper,
  Stack,
  Typography,
} from "@mui/material";
import AutoStoriesOutlinedIcon from "@mui/icons-material/AutoStoriesOutlined";
import ScienceOutlinedIcon from "@mui/icons-material/ScienceOutlined";
import QueryStatsOutlinedIcon from "@mui/icons-material/QueryStatsOutlined";
import GitHubIcon from "@mui/icons-material/GitHub";
import LinkedInIcon from "@mui/icons-material/LinkedIn";

const features = [
  {
    icon: AutoStoriesOutlinedIcon,
    title: "Open Source",
    description:
      "No more outdated, black box structural analysis programs. beamOS source code is free and open to the public.",
  },
  {
    icon: ScienceOutlinedIcon,
    title: "Robust Code Testing",
    description:
      "Code is tested against many solved problems from textbooks, research papers, and other structural analysis programs to make sure results are always accurate.",
  },
  {
    icon: QueryStatsOutlinedIcon,
    title: "Data Access Through APIs",
    description:
      "Data is easily retrievable through cloud-based APIs instead of locked in proprietary file formats. You own your data. In the age of AI, data is your most valuable asset.",
  },
];

export const HomePage = () => {
  return (
    <Stack spacing={5}>
      <Grid container spacing={4} alignItems="center">
        <Grid size={{ xs: 12, md: 7 }}>
          <Typography variant="h2" sx={{ mb: 2 }}>
            A better way to design structures
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ maxWidth: 620 }}>
            Powerful, open-source, and cloud-based structural analysis software and
            APIs. beamOS is enabling safer and more efficient designs.
          </Typography>
          <Stack direction={{ xs: "column", sm: "row" }} spacing={1.5} sx={{ mt: 3 }}>
            <Button variant="contained">
              Get Started
            </Button>
            <Button variant="outlined">
              Learn about reliability
            </Button>
          </Stack>
        </Grid>
        <Grid size={{ xs: 12, md: 5 }}>
          <Paper sx={{ overflow: "hidden" }}>
            <Box
              component="img"
              src="https://images.unsplash.com/photo-1557804506-669a67965ba0?auto=format&fit=crop&w=1400&q=80"
              alt="header image"
              sx={{
                display: "block",
                width: "100%",
                height: 240,
                objectFit: "cover",
              }}
            />
          </Paper>
        </Grid>
      </Grid>

      <Grid container spacing={3}>
        {features.map((feature) => (
          <Grid key={feature.title} size={{ xs: 12, md: 4 }}>
            <Card variant="outlined" sx={{ height: "100%" }}>
              <CardContent>
                <Stack spacing={2} alignItems="center" textAlign="center">
                  <Avatar sx={{ width: 48, height: 48, bgcolor: "primary.main" }}>
                  <feature.icon />
                </Avatar>
                <Typography variant="h5">
                  {feature.title}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  {feature.description}
                </Typography>
                </Stack>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      <Stack alignItems="center" spacing={1.5} sx={{ py: 1 }}>
        <Stack direction="row" spacing={2}>
          <Link href="https://github.com" color="inherit" target="_blank" rel="noreferrer">
            <GitHubIcon fontSize="small" />
          </Link>
          <Link href="https://linkedin.com" color="inherit" target="_blank" rel="noreferrer">
            <LinkedInIcon fontSize="small" />
          </Link>
        </Stack>
        <Typography variant="body2" color="text.secondary">
          © 2026 beamOS. All rights reserved.
        </Typography>
      </Stack>
    </Stack>
  );
};
