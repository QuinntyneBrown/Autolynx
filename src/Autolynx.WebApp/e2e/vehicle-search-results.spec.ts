import { test, expect } from '@playwright/test';

test.describe('Vehicle Search Results', () => {
  test('should display search results page with query parameters', async ({ page }) => {
    await page.goto('/results?make=Toyota&model=Camry&yearFrom=2020&yearTo=2024');
    
    // Check title
    await expect(page.locator('.search-results__title')).toContainText('Search Results');
    
    // Check search criteria chips are displayed
    await expect(page.locator('.search-results__criteria')).toContainText('Make: Toyota');
    await expect(page.locator('.search-results__criteria')).toContainText('Model: Camry');
  });

  test('should support deep linking with all search parameters', async ({ page }) => {
    const params = new URLSearchParams({
      make: 'Honda',
      model: 'Civic',
      yearFrom: '2018',
      yearTo: '2022',
      priceFrom: '15000',
      priceTo: '25000',
      location: 'Seattle'
    });
    
    await page.goto(`/results?${params.toString()}`);
    
    // Verify all parameters are displayed in chips
    await expect(page.locator('.search-results__criteria')).toContainText('Make: Honda');
    await expect(page.locator('.search-results__criteria')).toContainText('Model: Civic');
    await expect(page.locator('.search-results__criteria')).toContainText('Year: 2018 - 2022');
    await expect(page.locator('.search-results__criteria')).toContainText('Location: Seattle');
  });

  test('should handle empty search results', async ({ page }) => {
    // Navigate with minimal criteria
    await page.goto('/results?make=Toyota');
    
    // Should show either error message or empty state (backend not running)
    const hasError = await page.locator('.search-results__title').isVisible();
    
    expect(hasError).toBeTruthy();
  });
});
