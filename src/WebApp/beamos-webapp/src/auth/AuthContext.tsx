import { createContext, useContext, useState, type ReactNode } from "react";

export type User = {
  token: string | null;
  // name: string;
  email: string;
  // avatarUrl?: string;
};

export type AuthContextType = {
  user: User | null;
  login: (user: User) => void;
  logout: () => void;
};

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  // log in the user by default for development purposes
  const [user, setUser] = useState<User | null>({
    token: "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ1c2VyQGVtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJVc2VyIiwiYXVkIjpbImh0dHBzOi8vbG9jYWxob3N0OjcxOTMiLCJodHRwczovL2xvY2FsaG9zdDo3MTk0Il0sImV4cCI6NDg3MDA5MDM2MCwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NzE5NCJ9.CTW9SNJl4kSvAlJGZ7dDpFsCc-6hVeOu6OhJFllzGwkE2FZwBd34i7q8nIaKQDQf3T8-O-GqyF7Jbey2ULDPOA",
    email: "dummyuser@beamos.net"
  });

  const login = (user: User) => { setUser(user); };
  const logout = () => { setUser(null); };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
