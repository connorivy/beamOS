import React from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { CssBaseline, ThemeProvider } from "@mui/material";
import { App } from "./App";
import { useUiStore } from "./store/ui-store";
import { createAppTheme } from "./theme/theme";
import "./styles.css";

const Root = () => {
  const { mode, themeConfig } = useUiStore();
  const theme = React.useMemo(
    () => createAppTheme({ mode, themeConfig }),
    [mode, themeConfig],
  );

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <App />
      </BrowserRouter>
    </ThemeProvider>
  );
};

createRoot(document.getElementById("root")!).render(
  <React.StrictMode>
    <Root />
  </React.StrictMode>,
);
