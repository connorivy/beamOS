import { FormEvent, useState } from "react";
import { ThreeViewer } from "../components/ThreeViewer";
import { fetchUser } from "../lib/api";

export const HomePage = () => {
  const [userId, setUserId] = useState("1");
  const [status, setStatus] = useState<string>("Ready");
  const [userName, setUserName] = useState<string>("");

  const onSubmit = async (event: FormEvent) => {
    event.preventDefault();
    setStatus("Loading...");

    try {
      const user = await fetchUser(userId);
      setUserName(user.name);
      setStatus("Loaded");
    } catch (error) {
      setStatus(error instanceof Error ? error.message : "Unknown error");
    }
  };

  const onLoginClick = () => {
    window.location.href = "/login";
  };

  return (
    <main className="layout">
      <section className="panel">
        <h1>Beamos Admin</h1>
        <p>Three.js scene + typed Bun endpoint contracts.</p>

        <form onSubmit={onSubmit} className="row">
          <input value={userId} onChange={(e) => setUserId(e.target.value)} aria-label="user-id" />
          <button type="submit">Load User</button>
          <button type="button" onClick={onLoginClick}>
            Login
          </button>
        </form>

        <p data-testid="status">Status: {status}</p>
        <p data-testid="user-name">User: {userName || "-"}</p>
      </section>

      <section className="panel viewer-wrap">
        <h2>3D Viewer</h2>
        <ThreeViewer />
      </section>
    </main>
  );
};
