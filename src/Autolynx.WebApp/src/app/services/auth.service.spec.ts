import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService, LoginRequest, LoginResponse } from './auth.service';
import { environment } from '../../environments/environment';

describe('AuthService', () => {
  let service: AuthService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    httpMock = TestBed.inject(HttpTestingController);
    localStorage.clear();
  });

  afterEach(() => {
    httpMock.verify();
    localStorage.clear();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('login', () => {
    it('should successfully login and store token', () => {
      const mockResponse: LoginResponse = {
        token: 'test-token',
        username: 'testuser',
        roles: ['User']
      };

      service.login('testuser', 'password').subscribe(response => {
        expect(response).toEqual(mockResponse);
        expect(service.getToken()).toBe('test-token');
        expect(service.isAuthenticated()).toBe(true);
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({ username: 'testuser', password: 'password' });
      req.flush(mockResponse);
    });

    it('should update current user after login', (done) => {
      const mockResponse: LoginResponse = {
        token: 'test-token',
        username: 'admin',
        roles: ['Admin']
      };

      service.login('admin', 'password').subscribe(() => {
        service.currentUser$.subscribe(user => {
          expect(user).not.toBeNull();
          expect(user?.username).toBe('admin');
          expect(user?.roles).toEqual(['Admin']);
          done();
        });
      });

      const req = httpMock.expectOne(`${environment.apiUrl}/auth/login`);
      req.flush(mockResponse);
    });
  });

  describe('logout', () => {
    it('should clear token and user data', () => {
      localStorage.setItem('auth_token', 'test-token');
      localStorage.setItem('auth_user', JSON.stringify({ username: 'test', roles: ['User'] }));

      service.logout();

      expect(service.getToken()).toBeNull();
      expect(service.isAuthenticated()).toBe(false);
      expect(localStorage.getItem('auth_token')).toBeNull();
      expect(localStorage.getItem('auth_user')).toBeNull();
    });

    it('should update current user to null', (done) => {
      service.logout();
      service.currentUser$.subscribe(user => {
        expect(user).toBeNull();
        done();
      });
    });
  });

  describe('isAuthenticated', () => {
    it('should return true when token exists', () => {
      localStorage.setItem('auth_token', 'test-token');
      expect(service.isAuthenticated()).toBe(true);
    });

    it('should return false when token does not exist', () => {
      expect(service.isAuthenticated()).toBe(false);
    });
  });

  describe('hasRole', () => {
    beforeEach(() => {
      const user = { username: 'test', roles: ['Admin', 'User'] };
      localStorage.setItem('auth_user', JSON.stringify(user));
      // Force service to reload user
      service = TestBed.inject(AuthService);
    });

    it('should return true if user has the role', () => {
      expect(service.hasRole('Admin')).toBe(true);
      expect(service.hasRole('User')).toBe(true);
    });

    it('should return false if user does not have the role', () => {
      expect(service.hasRole('SuperAdmin')).toBe(false);
    });
  });

  describe('isAdmin', () => {
    it('should return true if user has Admin role', () => {
      const user = { username: 'admin', roles: ['Admin'] };
      localStorage.setItem('auth_user', JSON.stringify(user));
      service = TestBed.inject(AuthService);
      
      expect(service.isAdmin()).toBe(true);
    });

    it('should return false if user does not have Admin role', () => {
      const user = { username: 'user', roles: ['User'] };
      localStorage.setItem('auth_user', JSON.stringify(user));
      service = TestBed.inject(AuthService);
      
      expect(service.isAdmin()).toBe(false);
    });
  });

  describe('getCurrentUser', () => {
    it('should return current user when authenticated', () => {
      const user = { username: 'test', roles: ['User'] };
      localStorage.setItem('auth_user', JSON.stringify(user));
      service = TestBed.inject(AuthService);
      
      const currentUser = service.getCurrentUser();
      expect(currentUser).toEqual(user);
    });

    it('should return null when not authenticated', () => {
      const currentUser = service.getCurrentUser();
      expect(currentUser).toBeNull();
    });
  });
});
