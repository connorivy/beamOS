import { createApiClient } from "@beamos/openapi-client";

export const TUTORIAL_PROJECT_ID = "00000000-0000-7000-8000-000000000002";

export const seedSampleProjects = async (apiBaseUrl: string) => {
  const client = createApiClient(apiBaseUrl);

  const listResponse = await client.GET("/api/projects");
  if (listResponse.data?.some((p) => p.id === TUTORIAL_PROJECT_ID)) {
    return;
  }

  const { error } = await client.POST("/api/projects", {
    body: {
      id: TUTORIAL_PROJECT_ID,
      name: "Tutorial",
      description: "Learn the basics of BeamOS with this interactive tutorial",
      modelSettings: {
        units: {
          pressure: "KilopoundForcePerSquareInch",
          area: "SquareInch",
          areaMomentOfInertia: "InchToTheFourth",
          warpingMomentOfInertia: "InchToTheSixth",
          volume: "CubicInch",
        },
        yAxisUp: true,
      },
    },
  });

  if (error) {
    throw new Error(`Failed to seed tutorial project: ${JSON.stringify(error)}`);
  }
};
