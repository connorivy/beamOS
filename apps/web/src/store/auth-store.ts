import { create } from "zustand";

type AuthState = {
  isAuthenticated: boolean;
  loginWithGoogle: () => void;
  logout: () => void;
};

export const useAuthStore = create<AuthState>((set) => ({
  isAuthenticated: false,
  loginWithGoogle: () => set({ isAuthenticated: true }),
  logout: () => set({ isAuthenticated: false }),
}));
