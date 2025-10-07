import { test, expect } from '@playwright/test';

test('homepage loads and shows title', async ({ page }) => {
  await page.goto('/');
  await expect(page).toHaveTitle(/beam|react/i);
});

test('should display main app container', async ({ page }) => {
  await page.goto('/');
  // Check for the root app container
  await expect(page.locator('#root')).toBeVisible();
});
