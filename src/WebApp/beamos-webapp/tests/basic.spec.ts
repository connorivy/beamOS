import { test, expect } from "@playwright/test"

test("homepage loads and shows title", async ({ page }) => {
  await page.goto("/")
  await expect(page).toHaveTitle(/beam|react/i)
})

test("should display main app container", async ({ page }) => {
  await page.goto("/")
  // Check for the root app container
  await expect(page.locator("#root")).toBeVisible()
})

test("user-profile menu shows Settings option", async ({ page }) => {
  await page.goto("/")
  // Click the user-profile icon button (assuming it has aria-label or title 'User Profile' or similar)
  // Adjust selector as needed for your app
  await page.getByRole("button", { name: /user[- ]?profile/i }).click()
  // Assert that the 'Settings' option is visible
  await expect(page.getByRole("menuitem", { name: /settings/i })).toBeVisible()
})
