import { StrictMode } from "react"
import { createRoot } from "react-dom/client"
// import { Provider } from "react-redux"

import { App } from "./App"
// import { store } from "./app/store"
import "./index.css"
import { ThemeProvider } from "@mui/material/styles"
import CssBaseline from "@mui/material/CssBaseline"
import theme from "./theme"
import { BrowserRouter } from "react-router"
import { AuthProvider } from "./auth/AuthContext"
import { ApiClientProvider } from "./features/api-client/ApiClientContext"
import { StoreProvider } from "./app/StoreProvider"

const container = document.getElementById("root")

if (container) {
  const root = createRoot(container)

  root.render(
    <StrictMode>
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <ApiClientProvider>
          <StoreProvider>
            {/* <Provider store={store}> */}
            <BrowserRouter>
              <AuthProvider>
                <App />
              </AuthProvider>
            </BrowserRouter>
            {/* </Provider> */}
          </StoreProvider>
        </ApiClientProvider>
      </ThemeProvider>
    </StrictMode>,
  )
} else {
  throw new Error(
    "Root element with ID 'root' was not found in the document. Ensure there is a corresponding HTML element with the ID 'root' in your HTML file.",
  )
}
