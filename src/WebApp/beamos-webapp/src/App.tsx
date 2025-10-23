import { Routes, Route } from "react-router"
import { HomePage } from "./features/home/Home"
import ModelsPage from "./features/models-page/ModelsPage"
import ModelEditor from "./features/editors/ModelEditor"
import TutorialPage from "./features/tutorial/TutorialPage"
import type React from "react"
import SettingsPage from "./features/settings/SettingsPage"

export const App = () => {
  return <Routes>{GetAllRoutes()}</Routes>
}

export const GetAllRoutes = (): React.ReactElement[] => {
  return [
    <Route path="/" element={<HomePage />} />,
    <Route path="/settings" element={<SettingsPage />} />,
    <Route path="/models" element={<ModelsPage />} />,
    <Route path="/models/:modelId" element={<ModelEditor />} />,
    <Route path="/tutorial" element={<TutorialPage />} />,
  ]
}
