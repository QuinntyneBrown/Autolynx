import { test, expect } from '@playwright/test';

test.describe('Home Page', () => {
  test('should display home page with navigation', async ({ page }) => {
    await page.goto('/');
    
    // Check title
    await expect(page.locator('mat-toolbar')).toContainText('Autolynx');
    
    // Check hero section
    await expect(page.locator('.home__title')).toContainText('Welcome to Autolynx');
    
    // Check CTA buttons exist
    await expect(page.getByRole('button', { name: 'Start Searching' })).toBeVisible();
    await expect(page.getByRole('button', { name: 'View Dashboard' })).toBeVisible();
  });

  test('should navigate to search page', async ({ page }) => {
    await page.goto('/');
    
    await page.getByRole('button', { name: 'Start Searching' }).click();
    
    await expect(page).toHaveURL('/search');
    await expect(page.locator('.vehicle-search__title')).toContainText('Search for Vehicles');
  });

  test('should navigate to dashboard page', async ({ page }) => {
    await page.goto('/');
    
    await page.getByRole('button', { name: 'View Dashboard' }).click();
    
    await expect(page).toHaveURL('/dashboard');
    await expect(page.locator('.dashboard__title')).toContainText('Real-time Vehicle Search Dashboard');
  });
});
