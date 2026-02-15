import { createTheme, type PaletteMode } from "@mui/material";

export type ThemeConfig = {
  primaryMain: string;
  primaryDark: string;
  primaryLight: string;
  backgroundDefault: string;
  backgroundPaper: string;
};

type CreateThemeArgs = {
  mode: PaletteMode;
  themeConfig: ThemeConfig;
};

export const defaultThemeConfig: ThemeConfig = {
  primaryMain: "#7e57c2",
  primaryDark: "#5e35b1",
  primaryLight: "#b085f5",
  backgroundDefault: "#f5f2ff",
  backgroundPaper: "#ffffff",
};

export const createAppTheme = ({ mode, themeConfig }: CreateThemeArgs) =>
  createTheme({
    palette: {
      mode,
      primary: {
        main: themeConfig.primaryMain,
        dark: themeConfig.primaryDark,
        light: themeConfig.primaryLight,
      },
      background: {
        default:
          mode === "light" ? themeConfig.backgroundDefault : "#12101a",
        paper: mode === "light" ? themeConfig.backgroundPaper : "#1b1628",
      },
    },
    shape: {
      borderRadius: 12,
    },
    typography: {
      fontSize: 13,
      h1: {
        fontWeight: 700,
      },
      h2: {
        fontWeight: 700,
      },
      h4: {
        fontWeight: 600,
      },
      body1: {
        fontSize: "0.95rem",
      },
      body2: {
        fontSize: "0.88rem",
      },
    },
  });
