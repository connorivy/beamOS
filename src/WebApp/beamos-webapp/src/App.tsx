import { Routes, Route } from "react-router"
import { HomePage } from "./features/home/Home"
import ModelsPage from "./features/models-page/ModelsPage"
import { ModelEditorPage } from "./features/editors/ModelEditor"
import TutorialPage from "./features/tutorial/TutorialPage"
import type React from "react"
import SettingsPage from "./features/settings/SettingsPage"
import { Toaster } from "react-hot-toast"

export const App = () => {
  return (
    <>
      <Toaster />
      <Routes>{GetAllRoutes()}</Routes>
    </>
  )
}

export const GetAllRoutes = (): React.ReactElement[] => {
  return [
    <Route path="/" element={<HomePage />} />,
    <Route path="/settings" element={<SettingsPage />} />,
    <Route path="/models" element={<ModelsPage />} />,
    <Route path="/models/:modelId" element={<ModelEditorPage />} />,
    <Route path="/tutorial" element={<TutorialPage />} />,
  ]
}
