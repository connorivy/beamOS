

import { Routes, Route } from "react-router";
// import { LoginPage } from "./features/login/Login";
import { HomePage } from "./features/home/Home";
import type React from "react";
import SettingsPage from "./pages/SettingsPage";
// import LoginWithGoogleRedirect from "./features/login/LoginWithGoogleRedirect";

export const App = () => {
  return (
    <Routes>
      {/* <Route path="/" element={<HomePage />} /> */}
      {GetAllRoutes()}
      {/* <Route path="/login" element={<LoginPage />} />
      <Route path="/login-with-google-redirect" element={<LoginWithGoogleRedirect />} /> */}
    </Routes>
  );
};

export const GetAllRoutes = (): React.ReactElement[] => {
  return [
    <Route path="/" element={<HomePage />} />,
    <Route path="/settings" element={<SettingsPage />} />,
  ];
};