import { expect, test } from "@playwright/test";

test("loads homepage", async ({ page }) => {
    await page.goto("/");

    await expect(page.getByRole("heading", { name: "beamOS" })).toBeVisible();
});
