import { expect, test } from "@playwright/test";

test("loads homepage and fetches user", async ({ page }) => {
  await page.goto("/");

  await expect(page.getByRole("heading", { name: "Beamos Admin" })).toBeVisible();
  await expect(page.getByTestId("three-viewer")).toBeVisible();

  await page.getByLabel("user-id").fill("00000000-0000-4000-8000-000000000001");
  await page.getByRole("button", { name: "Load User" }).click();
  await expect(page.getByTestId("status")).toContainText("Loaded");
  await expect(page.getByTestId("user-name")).toContainText("Ada Lovelace");
});
