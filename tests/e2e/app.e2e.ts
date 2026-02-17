import { expect, test } from "@playwright/test";

test("loads homepage", async ({ page }) => {
  await page.setViewportSize({ width: 1280, height: 800 });
  await page.goto("/");

  await expect(
    page.getByRole("heading", { name: "A better way to design structures" }),
  ).toBeVisible();
  const screenshot = await page.screenshot({
    fullPage: true,
    animations: "disabled",
  });
  expect(screenshot.byteLength).toBeGreaterThan(10_000);
});

test("loads model revision route and renders summary", async ({ page }) => {
  await page.setViewportSize({ width: 1280, height: 800 });

  await page.route("**/api/models/*/branches/*/revision", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify({
        modelRevision: {
          id: "0195102e-1f21-7191-8a79-f1848ecf9ac5",
          modelId: "0195102e-1f21-7191-8a79-f1848ecf9ac6",
          name: "Demo Revision",
          parentRevisionId: null,
          secondParentRevisionId: null,
          authorId: "00000000-0000-4000-8000-000000000001",
          message: "Initial revision",
          createdAt: "2026-02-16T00:00:00.000Z",
          nodes: [
            {
              id: "0195102e-1f21-7191-8a79-f1848ecf9ac7",
              modelId: "0195102e-1f21-7191-8a79-f1848ecf9ac6",
              nodeTypeDescriminator: "external",
            },
            {
              id: "0195102e-1f21-7191-8a79-f1848ecf9ac8",
              modelId: "0195102e-1f21-7191-8a79-f1848ecf9ac6",
              nodeTypeDescriminator: "external",
            },
          ],
          materials: [
            {
              id: "0195102e-1f21-7191-8a79-f1848ecf9ac9",
              revisionId: "0195102e-1f21-7191-8a79-f1848ecf9ac5",
              name: "Steel",
              modulusOfElasticity: 200000000000,
              modulusOfRigidity: 79000000000,
            },
          ],
          modelSettings: {
            id: "0195102e-1f21-7191-8a79-f1848ecf9aca",
            revisionId: "0195102e-1f21-7191-8a79-f1848ecf9ac5",
            yAxisUp: false,
          },
          sectionProfiles: [
            {
              id: "0195102e-1f21-7191-8a79-f1848ecf9acb",
              revisionId: "0195102e-1f21-7191-8a79-f1848ecf9ac5",
              name: "W12x26",
            },
          ],
          element1ds: [
            {
              id: "0195102e-1f21-7191-8a79-f1848ecf9acc",
              revisionId: "0195102e-1f21-7191-8a79-f1848ecf9ac5",
              startNodeId: "0195102e-1f21-7191-8a79-f1848ecf9ac7",
              endNodeId: "0195102e-1f21-7191-8a79-f1848ecf9ac8",
              materialId: "0195102e-1f21-7191-8a79-f1848ecf9ac9",
              sectionProfileId: "0195102e-1f21-7191-8a79-f1848ecf9acb",
            },
          ],
        },
      }),
    });
  });

  await page.goto(
    "/models/0195102e-1f21-7191-8a79-f1848ecf9ac6/main",
  );

  await expect(page.getByRole("heading", { name: "Demo Revision" })).toBeVisible();
  await expect(page.getByText("Branch: main")).toBeVisible();
  const screenshot = await page.screenshot({
    fullPage: true,
    animations: "disabled",
  });
  expect(screenshot.byteLength).toBeGreaterThan(10_000);
});
