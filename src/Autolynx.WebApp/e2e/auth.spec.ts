import { test, expect } from '@playwright/test';

test.describe('Authentication', () => {
  test.beforeEach(async ({ page }) => {
    // Clear any existing auth state
    await page.context().clearCookies();
    await page.goto('/');
  });

  test.describe('Login Page', () => {
    test('should display login form', async ({ page }) => {
      await page.goto('/login');
      
      // Check page title
      await expect(page.locator('.login__title')).toContainText('Login');
      await expect(page.locator('.login__subtitle')).toContainText('Sign in to access your account');
      
      // Check form elements
      await expect(page.locator('input[name="username"]')).toBeVisible();
      await expect(page.locator('input[name="password"]')).toBeVisible();
      await expect(page.getByRole('button', { name: /login/i })).toBeVisible();
      
      // Check demo info
      await expect(page.locator('.login__demo-info')).toContainText('Demo Credentials');
    });

    test('should show validation error for empty credentials', async ({ page }) => {
      await page.goto('/login');
      
      // Try to submit without credentials
      await page.getByRole('button', { name: /login/i }).click();
      
      // Button should be disabled due to form validation
      await expect(page.getByRole('button', { name: /login/i })).toBeDisabled();
    });

    test('should successfully login with valid credentials', async ({ page }) => {
      await page.goto('/login');
      
      // Fill in credentials
      await page.locator('input[name="username"]').fill('testuser');
      await page.locator('input[name="password"]').fill('password123');
      
      // Submit form
      await page.getByRole('button', { name: /^login$/i }).click();
      
      // Should redirect to dashboard
      await expect(page).toHaveURL('/dashboard', { timeout: 10000 });
    });

    test('should login as admin with admin username', async ({ page }) => {
      await page.goto('/login');
      
      // Fill in admin credentials
      await page.locator('input[name="username"]').fill('admin');
      await page.locator('input[name="password"]').fill('password');
      
      // Submit form
      await page.getByRole('button', { name: /^login$/i }).click();
      
      // Should redirect to dashboard
      await expect(page).toHaveURL('/dashboard', { timeout: 10000 });
      
      // Check if admin role is displayed
      await page.goto('/');
      await expect(page.locator('.auth-nav__role')).toContainText('Admin');
    });

    test('should preserve return URL after login', async ({ page }) => {
      // Try to access protected route
      await page.goto('/search');
      
      // Should redirect to login with returnUrl
      await expect(page).toHaveURL(/\/login\?returnUrl=%2Fsearch/);
      
      // Login
      await page.locator('input[name="username"]').fill('testuser');
      await page.locator('input[name="password"]').fill('password');
      await page.getByRole('button', { name: /^login$/i }).click();
      
      // Should redirect back to search page
      await expect(page).toHaveURL('/search', { timeout: 10000 });
    });

    test('should show loading state during login', async ({ page }) => {
      await page.goto('/login');
      
      // Fill in credentials
      await page.locator('input[name="username"]').fill('testuser');
      await page.locator('input[name="password"]').fill('password');
      
      // Submit form
      await page.getByRole('button', { name: /^login$/i }).click();
      
      // Check loading state
      await expect(page.getByText(/logging in/i)).toBeVisible();
    });
  });

  test.describe('Protected Routes', () => {
    test('should redirect to login when accessing search without auth', async ({ page }) => {
      await page.goto('/search');
      
      await expect(page).toHaveURL(/\/login/);
    });

    test('should redirect to login when accessing results without auth', async ({ page }) => {
      await page.goto('/results');
      
      await expect(page).toHaveURL(/\/login/);
    });

    test('should redirect to login when accessing dashboard without auth', async ({ page }) => {
      await page.goto('/dashboard');
      
      await expect(page).toHaveURL(/\/login/);
    });

    test('should allow access to protected routes when authenticated', async ({ page }) => {
      // Login first
      await page.goto('/login');
      await page.locator('input[name="username"]').fill('testuser');
      await page.locator('input[name="password"]').fill('password');
      await page.getByRole('button', { name: /^login$/i }).click();
      
      await expect(page).toHaveURL('/dashboard', { timeout: 10000 });
      
      // Navigate to search
      await page.goto('/search');
      await expect(page).toHaveURL('/search');
      
      // Navigate to results
      await page.goto('/results');
      await expect(page).toHaveURL('/results');
      
      // Navigate to dashboard
      await page.goto('/dashboard');
      await expect(page).toHaveURL('/dashboard');
    });
  });

  test.describe('Logout', () => {
    test('should logout and redirect to home', async ({ page }) => {
      // Login first
      await page.goto('/login');
      await page.locator('input[name="username"]').fill('testuser');
      await page.locator('input[name="password"]').fill('password');
      await page.getByRole('button', { name: /^login$/i }).click();
      
      await expect(page).toHaveURL('/dashboard', { timeout: 10000 });
      
      // Go to home
      await page.goto('/');
      
      // Click logout
      await page.getByRole('button', { name: /logout/i }).click();
      
      // Should show login button
      await expect(page.getByRole('button', { name: /^login$/i })).toBeVisible();
      
      // Try to access protected route - should redirect to login
      await page.goto('/search');
      await expect(page).toHaveURL(/\/login/);
    });

    test('should clear user session on logout', async ({ page }) => {
      // Login first
      await page.goto('/login');
      await page.locator('input[name="username"]').fill('admin');
      await page.locator('input[name="password"]').fill('password');
      await page.getByRole('button', { name: /^login$/i }).click();
      
      await expect(page).toHaveURL('/dashboard', { timeout: 10000 });
      
      // Go to home and verify logged in
      await page.goto('/');
      await expect(page.locator('.auth-nav__username')).toContainText('admin');
      
      // Logout
      await page.getByRole('button', { name: /logout/i }).click();
      
      // Verify user is cleared
      await expect(page.locator('.auth-nav__username')).not.toBeVisible();
    });
  });

  test.describe('Session Persistence', () => {
    test('should persist session across page reloads', async ({ page }) => {
      // Login
      await page.goto('/login');
      await page.locator('input[name="username"]').fill('testuser');
      await page.locator('input[name="password"]').fill('password');
      await page.getByRole('button', { name: /^login$/i }).click();
      
      await expect(page).toHaveURL('/dashboard', { timeout: 10000 });
      
      // Reload page
      await page.reload();
      
      // Should still be on dashboard (not redirected to login)
      await expect(page).toHaveURL('/dashboard');
      
      // Should still be able to access protected routes
      await page.goto('/search');
      await expect(page).toHaveURL('/search');
    });
  });

  test.describe('Navigation Integration', () => {
    test('should show login button when not authenticated', async ({ page }) => {
      await page.goto('/');
      
      await expect(page.getByRole('button', { name: /^login$/i })).toBeVisible();
      await expect(page.locator('.auth-nav__username')).not.toBeVisible();
    });

    test('should show username and logout button when authenticated', async ({ page }) => {
      // Login
      await page.goto('/login');
      await page.locator('input[name="username"]').fill('testuser');
      await page.locator('input[name="password"]').fill('password');
      await page.getByRole('button', { name: /^login$/i }).click();
      
      await expect(page).toHaveURL('/dashboard', { timeout: 10000 });
      
      // Go to home
      await page.goto('/');
      
      // Should show username and logout
      await expect(page.locator('.auth-nav__username')).toContainText('testuser');
      await expect(page.getByRole('button', { name: /logout/i })).toBeVisible();
      await expect(page.getByRole('button', { name: /^login$/i })).not.toBeVisible();
    });

    test('should display admin role badge for admin users', async ({ page }) => {
      // Login as admin
      await page.goto('/login');
      await page.locator('input[name="username"]').fill('admin');
      await page.locator('input[name="password"]').fill('password');
      await page.getByRole('button', { name: /^login$/i }).click();
      
      await expect(page).toHaveURL('/dashboard', { timeout: 10000 });
      
      // Go to home
      await page.goto('/');
      
      // Should show admin badge
      await expect(page.locator('.auth-nav__role')).toContainText('Admin');
    });
  });
});
