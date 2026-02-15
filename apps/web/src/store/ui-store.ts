import { create } from "zustand";
import { defaultThemeConfig, type ThemeConfig } from "../theme/theme";

type UiState = {
  mode: "light" | "dark";
  themeConfig: ThemeConfig;
  setMode: (mode: UiState["mode"]) => void;
  setThemeConfig: (config: Partial<ThemeConfig>) => void;
};

export const useUiStore = create<UiState>((set) => ({
  mode: "dark",
  themeConfig: defaultThemeConfig,
  setMode: (mode) => set({ mode }),
  setThemeConfig: (config) =>
    set((state) => ({
      themeConfig: { ...state.themeConfig, ...config },
    })),
}));
