import { expect, test } from "@playwright/test";
import { kassimaliExample3_8Project } from "../fixtures/Kassimali_MatrixAnalysisOfStructures2ndEd/Kassimali_Example3_8.fixture";

const TUTORIAL_PROJECT_ID = kassimaliExample3_8Project.project.id;

test("clicking tutorial card navigates to the editor", async ({ page }) => {
    await page.goto("/models");

    await expect(page.getByRole("heading", { name: "Sample Models" })).toBeVisible();

    const tutorialCard = page.getByRole("link", { name: /Tutorial/ });
    await expect(tutorialCard).toBeVisible();

    await tutorialCard.click();

    await expect(page).toHaveURL(`/editor/projects/${TUTORIAL_PROJECT_ID}/main`);
});

test("tutorial editor: complete Mission 1 by forking and land on new project page", async ({ page }) => {
    await page.goto(`/editor/projects/${TUTORIAL_PROJECT_ID}/main`);

    // Step 1: Mission intro modal should appear
    await expect(page.getByText("Mission 1 — Fork the Model")).toBeVisible();
    await expect(page.getByText("Fork this model to create your own working copy.")).toBeVisible();

    // Advance to the fork step
    await page.getByRole("button", { name: /Next/ }).click();

    // Step 2: Spotlight on Fork button with instructions
    await expect(page.getByText("Fork the Project")).toBeVisible();

    // Click the Fork Project button to complete Mission 1
    await page.getByRole("button", { name: /Fork Project/ }).click();

    // After forking, the app navigates to the new forked project (a different ID)
    await page.waitForURL(
        (url) => url.pathname.startsWith("/editor/projects/") && !url.pathname.includes(TUTORIAL_PROJECT_ID),
        { timeout: 15_000 },
    );

    // The URL should be a different project than the tutorial
    const newUrl = page.url();
    const newProjectId = newUrl.match(/\/editor\/projects\/([^/]+)\/main/)?.[1];
    expect(newProjectId).toBeDefined();
    expect(newProjectId).not.toBe(TUTORIAL_PROJECT_ID);

    // Should land on the editor page for the forked project
    await expect(page.getByRole("heading", { name: "beamOS Editor" })).toBeVisible();
});
