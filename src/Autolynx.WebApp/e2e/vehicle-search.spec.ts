import { test, expect } from '@playwright/test';

test.describe('Vehicle Search', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/search');
  });

  test('should display search form', async ({ page }) => {
    await expect(page.locator('.vehicle-search__title')).toContainText('Search for Vehicles');
    
    // Check all form fields are present
    await expect(page.getByLabel('Make')).toBeVisible();
    await expect(page.getByLabel('Model')).toBeVisible();
    await expect(page.getByLabel('Year From')).toBeVisible();
    await expect(page.getByLabel('Year To')).toBeVisible();
    await expect(page.getByLabel('Price From ($)')).toBeVisible();
    await expect(page.getByLabel('Price To ($)')).toBeVisible();
  });

  test('should navigate to results page with search criteria', async ({ page }) => {
    // Fill in search form
    await page.getByLabel('Make').fill('Toyota');
    await page.getByLabel('Model').fill('Camry');
    await page.getByLabel('Year From').fill('2020');
    await page.getByLabel('Year To').fill('2024');
    
    // Submit form
    await page.getByRole('button', { name: 'Search' }).click();
    
    // Check navigation and query params
    await expect(page).toHaveURL(/\/results\?.*make=Toyota.*model=Camry/);
  });

  test('should reset form fields', async ({ page }) => {
    // Fill in form
    await page.getByLabel('Make').fill('Toyota');
    await page.getByLabel('Model').fill('Camry');
    
    // Reset
    await page.getByRole('button', { name: 'Reset' }).click();
    
    // Check fields are cleared
    await expect(page.getByLabel('Make')).toHaveValue('');
    await expect(page.getByLabel('Model')).toHaveValue('');
  });
});
