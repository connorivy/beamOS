import { collectPluginRoutes } from "./plugins/types";
import { webPlugins } from "./plugins/registry";

const routes = collectPluginRoutes(webPlugins);

export const App = () => {
  const currentPath = window.location.pathname;
  const route = routes.find((entry) => entry.path === currentPath);

  if (!route) {
    return (
      <main className="route-layout">
        <section className="panel">
          <h1>404 Not Found</h1>
          <p>No OSS route is registered for <code>{currentPath}</code>.</p>
        </section>
      </main>
    );
  }

  const RouteComponent = route.Component;
  return <RouteComponent />;
};
