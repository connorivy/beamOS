
import { Routes, Route } from "react-router";
import { LoginPage } from "./features/login/Login";
import { HomePage } from "./features/home/Home";

export const App = () => {
  return (
    <Routes>
      <Route path="/" element={<HomePage />} />
      <Route path="/login" element={<LoginPage />} />
    </Routes>
  );
};
