import { test, expect } from '@playwright/test';

test.describe('Dashboard', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/dashboard');
  });

  test('should display dashboard page', async ({ page }) => {
    await expect(page.locator('.dashboard__title')).toContainText('Real-time Vehicle Search Dashboard');
  });

  test('should show connection status', async ({ page }) => {
    // Should show some connection status (connected, connecting, or disconnected)
    const statusChip = page.locator('.dashboard__status-chip');
    await expect(statusChip).toBeVisible();
    
    // The status text should be one of the expected values
    const statusText = await statusChip.textContent();
    expect(['Connected', 'Connecting...', 'Disconnected']).toContain(statusText?.trim());
  });

  test('should display appropriate message based on connection state', async ({ page }) => {
    // Wait for page to load
    await page.waitForLoadState('networkidle');
    
    // Should show either results, loading, or connection info
    const hasLoading = await page.locator('.dashboard__loading').isVisible();
    const hasInfo = await page.locator('.dashboard__info').isVisible();
    const hasResults = await page.locator('.dashboard__results').isVisible();
    
    expect(hasLoading || hasInfo || hasResults).toBeTruthy();
  });
});
