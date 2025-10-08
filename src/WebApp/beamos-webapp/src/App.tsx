

import { Routes, Route } from "react-router";
// import { LoginPage } from "./features/login/Login";
import { HomePage } from "./features/home/Home";
import type React from "react";
// import LoginWithGoogleRedirect from "./features/login/LoginWithGoogleRedirect";

export const App = () => {
  return (
    <Routes>
      {/* <Route path="/" element={<HomePage />} /> */}
      {GetAllRoutes().map(route => route)}
      {/* <Route path="/login" element={<LoginPage />} />
      <Route path="/login-with-google-redirect" element={<LoginWithGoogleRedirect />} /> */}
    </Routes>
  );
};

export const GetAllRoutes = (): React.ReactElement[] => {
  return [
    <Route path="/" element={<HomePage />} />
  ];
};