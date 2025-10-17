

import { Routes, Route } from "react-router";
import { HomePage } from "./features/home/Home";
import type React from "react";
import SettingsPage from "./features/settings/SettingsPage";

export const App = () => {
  return (
    <Routes>
      {GetAllRoutes()}
    </Routes>
  );
};

export const GetAllRoutes = (): React.ReactElement[] => {
  return [
    <Route path="/" element={<HomePage />} />,
    <Route path="/settings" element={<SettingsPage />} />,
  ];
};