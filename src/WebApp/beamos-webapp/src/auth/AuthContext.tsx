import { createContext, useContext, useState, type ReactNode } from "react"

export type User = {
  token: string | null
  // name: string;
  email: string
  // avatarUrl?: string;
}

export type AuthContextType = {
  user: User | null
  login: (user: User) => void
  logout: () => void
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined)

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  // log in the user by default for development purposes
  const [user, setUser] = useState<User | null>({
    token: "dummy-token",
    email: "dummyuser@beamos.net",
  })

  const login = (user: User) => {
    setUser(user)
  }
  const logout = () => {
    setUser(null)
  }

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider")
  }
  return context
}
