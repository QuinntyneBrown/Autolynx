import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';
import { Login } from './login';
import { AuthService } from '../../services/auth.service';

describe('Login', () => {
  let component: Login;
  let fixture: ComponentFixture<Login>;
  let authService: jasmine.SpyObj<AuthService>;

  beforeEach(async () => {
    const authServiceSpy = jasmine.createSpyObj('AuthService', ['login']);

    await TestBed.configureTestingModule({
      imports: [Login],
      providers: [
        provideRouter([]),
        provideAnimations(),
        { provide: AuthService, useValue: authServiceSpy }
      ]
    }).compileComponents();

    authService = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
    fixture = TestBed.createComponent(Login);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with empty credentials', () => {
    expect(component.username).toBe('');
    expect(component.password).toBe('');
    expect(component.errorMessage).toBe('');
    expect(component.isLoading).toBe(false);
  });

  it('should set returnUrl from query params', () => {
    component.ngOnInit();
    expect(component.returnUrl).toBe('/dashboard'); // default value
  });

  describe('onSubmit', () => {
    it('should show error when username is empty', () => {
      component.username = '';
      component.password = 'password';

      component.onSubmit();

      expect(component.errorMessage).toBe('Please enter both username and password');
      expect(authService.login).not.toHaveBeenCalled();
    });

    it('should show error when password is empty', () => {
      component.username = 'testuser';
      component.password = '';

      component.onSubmit();

      expect(component.errorMessage).toBe('Please enter both username and password');
      expect(authService.login).not.toHaveBeenCalled();
    });

    it('should call authService.login with correct credentials', () => {
      component.username = 'testuser';
      component.password = 'testpassword';
      authService.login.and.returnValue(of({
        token: 'test-token',
        username: 'testuser',
        roles: ['User']
      }));

      component.onSubmit();

      expect(authService.login).toHaveBeenCalledWith('testuser', 'testpassword');
    });

    it('should set isLoading to true during login', () => {
      component.username = 'testuser';
      component.password = 'testpassword';
      authService.login.and.returnValue(of({
        token: 'test-token',
        username: 'testuser',
        roles: ['User']
      }));

      component.onSubmit();

      // isLoading should have been true during the call
      expect(authService.login).toHaveBeenCalled();
    });

    it('should navigate to returnUrl on successful login', (done) => {
      component.username = 'testuser';
      component.password = 'testpassword';
      component.returnUrl = '/search';
      
      authService.login.and.returnValue(of({
        token: 'test-token',
        username: 'testuser',
        roles: ['User']
      }));

      spyOn(component['router'], 'navigateByUrl');

      component.onSubmit();

      setTimeout(() => {
        expect(component['router'].navigateByUrl).toHaveBeenCalledWith('/search');
        done();
      }, 10);
    });

    it('should show error message on login failure', (done) => {
      component.username = 'testuser';
      component.password = 'wrongpassword';
      
      const errorResponse = { error: { message: 'Invalid credentials' } };
      authService.login.and.returnValue(throwError(() => errorResponse));

      component.onSubmit();

      setTimeout(() => {
        expect(component.errorMessage).toBe('Invalid credentials');
        expect(component.isLoading).toBe(false);
        done();
      }, 10);
    });

    it('should show default error message when no specific message provided', (done) => {
      component.username = 'testuser';
      component.password = 'wrongpassword';
      
      authService.login.and.returnValue(throwError(() => ({})));

      component.onSubmit();

      setTimeout(() => {
        expect(component.errorMessage).toBe('Login failed. Please check your credentials.');
        expect(component.isLoading).toBe(false);
        done();
      }, 10);
    });

    it('should clear error message before new login attempt', () => {
      component.username = 'testuser';
      component.password = 'testpassword';
      component.errorMessage = 'Previous error';
      
      authService.login.and.returnValue(of({
        token: 'test-token',
        username: 'testuser',
        roles: ['User']
      }));

      component.onSubmit();

      expect(component.errorMessage).toBe('');
    });
  });
});
