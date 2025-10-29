import { createTheme } from "@mui/material/styles"

const theme = createTheme({
  palette: {
    mode: "dark",
    primary: {
      main: "#5e6ad2",
      light: "#7c88e5",
      dark: "#4d58b8",
    },
    secondary: {
      main: "#8b92d1",
      light: "#a4aade",
      dark: "#7279bf",
    },
    background: {
      default: "#0d0d0d",
      paper: "#1a1a1a",
    },
    text: {
      primary: "#e8eaed",
      secondary: "#9ca3af",
    },
    divider: "#2a2a2a",
    action: {
      hover: "rgba(255, 255, 255, 0.05)",
      selected: "rgba(94, 106, 210, 0.12)",
    },
  },
  typography: {
    fontFamily: [
      "Inter",
      "-apple-system",
      "BlinkMacSystemFont",
      "Segoe UI",
      "Roboto",
      "Helvetica",
      "Arial",
      "sans-serif",
    ].join(","),
    fontWeightBold: 600,
    fontWeightRegular: 400,
    fontWeightMedium: 500,
    h2: {
      fontWeight: 700,
      letterSpacing: "-0.02em",
    },
    h5: {
      fontWeight: 600,
      letterSpacing: "-0.01em",
    },
    h6: {
      fontWeight: 500,
      letterSpacing: "-0.01em",
    },
  },
  shape: {
    borderRadius: 8,
  },
  components: {
    MuiAppBar: {
      styleOverrides: {
        root: {
          backgroundColor: "#1a1a1a",
          borderBottom: "1px solid #2a2a2a",
          backgroundImage: "none",
          "--Paper-overlay": "none",
        },
      },
    },
    // MuiCard: {
    //   styleOverrides: {
    //     root: {
    //       backgroundColor: "#1a1a1a",
    //       border: "1px solid #2a2a2a",
    //       backgroundImage: "none",
    //     },
    //   },
    // },
    MuiButton: {
      styleOverrides: {
        root: {
          textTransform: "none",
          fontWeight: 500,
          borderRadius: 6,
        },
        contained: {
          boxShadow: "none",
          "&:hover": {
            boxShadow: "none",
          },
        },
      },
    },
  },
})

export default theme
