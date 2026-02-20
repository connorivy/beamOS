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
