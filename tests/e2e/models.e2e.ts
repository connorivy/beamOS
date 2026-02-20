import { expect, test } from "@playwright/test";
import { TUTORIAL_PROJECT_ID } from "../../apps/api/src/db/seed-sample-projects";

test("clicking tutorial card navigates to the editor", async ({ page }) => {
  await page.goto("/models");

  await expect(page.getByRole("heading", { name: "Sample Models" })).toBeVisible();

  const tutorialCard = page.getByRole("link", { name: /Tutorial/ });
  await expect(tutorialCard).toBeVisible();

  await tutorialCard.click();

  await expect(page).toHaveURL(
    `/editor/projects/${TUTORIAL_PROJECT_ID}/main`,
  );
});
