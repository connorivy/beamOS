// import { useEffect, useMemo, useState } from "react";
// import { Navigate, Route, Routes } from "react-router-dom";
// import { AppLayout } from "./layout/AppLayout";
// import { HomePage } from "./pages/HomePage";
// import { ModelsPage } from "./pages/ModelsPage";
// import type { WebPlugin } from "./plugins/types";

import { collectPluginRoutes } from "./plugins/types";
import { webPlugins } from "./plugins/registry";
import { matchPath, useLocation } from "react-router-dom";
import { AppLayout } from "./layout/AppLayout";

const routes = collectPluginRoutes(webPlugins);

export const App = () => {
  const { pathname: currentPath } = useLocation();
  const route = routes.find((entry) => matchPath({ path: entry.path, end: true }, currentPath));

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
  return (
    <AppLayout>
      <RouteComponent />
    </AppLayout>
  );
};
// export const App = () => {
//   const [plugins, setPlugins] = useState<WebPlugin[]>([]);
//   const [pluginsReady, setPluginsReady] = useState(false);

//   useEffect(() => {
//     let isMounted = true;

//     void loadWebPlugins()
//       .then((loadedPlugins) => {
//         if (isMounted) {
//           setPlugins(loadedPlugins);
//         }
//       })
//       .finally(() => {
//         if (isMounted) {
//           setPluginsReady(true);
//         }
//       });

//     return () => {
//       isMounted = false;
//     };
//   }, []);

//   const pluginRoutes = useMemo(
//     () => plugins.flatMap((plugin) => plugin.routes),
//     [plugins],
//   );

//   return (
//     <Routes>
//       <Route element={<AppLayout />}>
//         <Route path="/" element={<HomePage />} />
//         <Route path="/models" element={<ModelsPage />} />
//         {pluginRoutes.map(({ path, Component }) => (
//           <Route key={path} path={path} element={<Component />} />
//         ))}
//       </Route>
//       <Route
//         path="*"
//         element={pluginsReady ? <Navigate to="/" replace /> : null}
//       />
//     </Routes>
//   );
// };
