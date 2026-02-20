import { seedSampleProjects } from "./seed-sample-projects";

const DEFAULT_API_BASE_URL = "http://127.0.0.1:3001";
const API_BASE_URL = process.env.API_BASE_URL ?? DEFAULT_API_BASE_URL;
// const API_HEALTH_URL = `${API_BASE_URL}/health`;
// const HEALTH_WAIT_TIMEOUT_MS = 60_000;
// const HEALTH_WAIT_INTERVAL_MS = 500;

// const waitForApi = async () => {
//   const started = Date.now();

//   while (Date.now() - started < HEALTH_WAIT_TIMEOUT_MS) {
//     try {
//       const response = await fetch(API_HEALTH_URL);
//       if (response.ok) {
//         return;
//       }
//     } catch {
//       // API is not up yet.
//     }

//     await new Promise((resolve) => setTimeout(resolve, HEALTH_WAIT_INTERVAL_MS));
//   }

//   throw new Error(`Timed out waiting for API at ${API_HEALTH_URL}`);
// };

// await waitForApi();
await seedSampleProjects(API_BASE_URL);
console.log(`Seeded sample projects on ${API_BASE_URL}`);
