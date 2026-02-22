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

    // Click the Fork Project button to complete Mission 1.
    // The joyride overlay sits at the button's viewport coordinates, so even
    // { force: true } sends pointer events to the overlay, not the button.
    // Calling el.click() from evaluate() dispatches the event directly on
    // the DOM element and bubbles up through React's event delegation.
    await page.locator("#tutorial-fork-button").evaluate((el) =>
        (el as HTMLButtonElement).click(),
    );

    // After forking, the app navigates to the new forked project (a different ID).
    // Use expect().toHaveURL() with a predicate — it polls the URL via retry-ability,
    // which works for SPA navigation (history.pushState) unlike page.waitForURL whose
    // default waitUntil:'load' never fires for client-side navigation.
    await expect(page).toHaveURL(
        (url) => url.pathname.startsWith("/editor/projects/") && !url.pathname.includes(TUTORIAL_PROJECT_ID),
        { timeout: 30_000 },
    );

    // The URL should be a different project than the tutorial
    const newUrl = page.url();
    const newProjectId = newUrl.match(/\/editor\/projects\/([^/]+)\/main/)?.[1];
    expect(newProjectId).toBeDefined();
    expect(newProjectId).not.toBe(TUTORIAL_PROJECT_ID);

    // Should land on the editor page for the forked project
    await expect(page.getByRole("heading", { name: "beamOS Editor" })).toBeVisible();

    // Action 2 (Mission 2): create/switch to a feature branch for isolated changes.
    const branchName = "feature/add-loads";
    await page.goto(`/editor/projects/${newProjectId}/${branchName}`);

    await expect(page).toHaveURL(`/editor/projects/${newProjectId}/${branchName}`);
    await expect(page.getByText(branchName)).toBeVisible();
});
