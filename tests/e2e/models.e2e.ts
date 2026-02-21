import { expect, test } from "@playwright/test";
import { kassimaliExample3_8Project } from "../fixtures/Kassimali_MatrixAnalysisOfStructures2ndEd/Kassimali_Example3_8.fixture";

test("clicking tutorial card navigates to the editor", async ({ page }) => {
    await page.goto("/models");

    await expect(page.getByRole("heading", { name: "Sample Models" })).toBeVisible();

    const tutorialCard = page.getByRole("link", { name: /Tutorial/ });
    await expect(tutorialCard).toBeVisible();

    await tutorialCard.click();

    await expect(page).toHaveURL(`/editor/projects/${kassimaliExample3_8Project.project.id}/main`);
});

test("tutorial editor shows Mission 1 onboarding tour and fork button", async ({ page }) => {
    await page.goto(`/editor/projects/${kassimaliExample3_8Project.project.id}/main`);

    // The joyride tour should show the Mission 1 intro step
    await expect(page.getByText("Mission 1 — Fork the Model")).toBeVisible();
    await expect(page.getByText("Fork this model to create your own working copy.")).toBeVisible();

    // Advance to the fork step
    await page.getByRole("button", { name: /Next/ }).click();

    // The fork button should be highlighted
    await expect(page.getByRole("button", { name: /Fork Project/ })).toBeVisible();
});
