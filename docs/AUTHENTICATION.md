# Authentication and Authorization

This document describes the authentication and role-based authorization implementation in Autolynx.

## Overview

Autolynx uses JWT (JSON Web Token) based authentication to secure API endpoints. The implementation includes:

- JWT token generation and validation
- Role-based authorization (Admin, User)
- Configurable authentication settings
- Secure endpoint protection

## Configuration

Authentication settings are configured in `appsettings.json` and `appsettings.Development.json`:

```json
{
  "JwtOptions": {
    "Secret": "your-secret-key-here-minimum-32-characters-long",
    "Issuer": "Autolynx",
    "Audience": "AutolynxUsers",
    "ExpirationMinutes": 60
  }
}
```

### Configuration Properties

- **Secret**: The secret key used to sign JWT tokens (minimum 32 characters)
- **Issuer**: The issuer of the JWT token
- **Audience**: The intended audience for the JWT token
- **ExpirationMinutes**: Token expiration time in minutes (default: 60)

### Security Considerations

⚠️ **Important**: In production environments:
- Store the secret key in secure configuration (Azure Key Vault, environment variables, etc.)
- Use a strong, randomly generated secret key
- Never commit the secret key to source control
- Consider using asymmetric keys (RSA) for enhanced security

## Authentication Flow

### 1. Login

Users authenticate by posting credentials to the `/api/auth/login` endpoint:

**Request:**
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "testpassword"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "testuser",
  "roles": ["User"]
}
```

### 2. Using the Token

Include the JWT token in the Authorization header for subsequent requests:

```http
GET /api/vehicles/search
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Role-Based Authorization

The system supports role-based access control:

### Roles

- **Admin**: Full access to all endpoints
- **User**: Standard user access

### Role Assignment

Currently, roles are assigned based on username (demonstration implementation):
- Username "admin" receives the "Admin" role
- All other usernames receive the "User" role

⚠️ **Note**: This is a demonstration implementation. In production, implement proper user management with database-backed authentication and authorization.

## Protected Endpoints

The following endpoints require authentication:

- `POST /api/vehicles/search` - Requires authentication (any authenticated user)

## Implementation Details

### Key Components

1. **JwtOptions** (`src/Autolynx.Core/Options/JwtOptions.cs`)
   - Configuration class for JWT settings

2. **AuthController** (`src/Autolynx.Api/Controllers/AuthController.cs`)
   - Handles authentication requests
   - Generates JWT tokens with user claims

3. **ConfigureServices** (`src/Autolynx.Api/ConfigureServices.cs`)
   - Configures JWT authentication middleware
   - Sets up token validation parameters

4. **Program.cs** (`src/Autolynx.Api/Program.cs`)
   - Configures authentication middleware pipeline

### Token Structure

JWT tokens include the following claims:
- `sub`: Subject (username)
- `name`: User's name
- `role`: User's role(s)
- `jti`: Unique token identifier
- `exp`: Expiration timestamp
- `iss`: Issuer
- `aud`: Audience

## Testing

The implementation includes comprehensive tests:

### Unit Tests
- `AuthControllerTests.cs` - Tests for authentication controller

### Integration Tests
- `AuthenticationIntegrationTests.cs` - End-to-end authentication tests
- `VehicleSearchIntegrationTests.cs` - Tests for protected endpoints

Run tests with:
```bash
dotnet test
```

## Example Usage

### cURL Example

```bash
# Login
TOKEN=$(curl -X POST http://localhost:5188/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"testuser","password":"testpassword"}' \
  | jq -r '.token')

# Use token to access protected endpoint
curl -X POST http://localhost:5188/api/vehicles/search \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"make":"Toyota","model":"Camry"}'
```

### JavaScript Example

```javascript
// Login
const loginResponse = await fetch('http://localhost:5188/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ username: 'testuser', password: 'testpassword' })
});
const { token } = await loginResponse.json();

// Search vehicles with authentication
const searchResponse = await fetch('http://localhost:5188/api/vehicles/search', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({ make: 'Toyota', model: 'Camry' })
});
const results = await searchResponse.json();
```

## Future Enhancements

Consider implementing:

1. **User Management**
   - User registration endpoint
   - Password hashing (bcrypt, PBKDF2)
   - User database storage

2. **Advanced Authorization**
   - Fine-grained permissions
   - Policy-based authorization
   - Resource-based authorization

3. **Security Features**
   - Refresh tokens
   - Token revocation
   - Rate limiting
   - Account lockout policies

4. **OAuth/OpenID Connect**
   - Support for external identity providers
   - Social login (Google, Microsoft, etc.)

## Troubleshooting

### Common Issues

**401 Unauthorized**
- Ensure the token is included in the Authorization header
- Verify the token hasn't expired
- Check that the secret key matches between token generation and validation

**Invalid Token**
- Verify the token format: `Bearer <token>`
- Check that the issuer and audience match configuration
- Ensure the secret key is correct

**Configuration Errors**
- Verify JwtOptions section exists in appsettings.json
- Check that the secret key is at least 32 characters
- Ensure authentication middleware is configured before authorization

## References

- [JWT.io](https://jwt.io/) - JWT debugger and documentation
- [Microsoft Identity Platform](https://docs.microsoft.com/en-us/azure/active-directory/develop/)
- [ASP.NET Core Security](https://docs.microsoft.com/en-us/aspnet/core/security/)

## Frontend Implementation

The Angular frontend includes complete authentication functionality:

### Components

**Login Page** (`src/app/pages/login/`)
- Username/password form
- Error handling
- Loading state
- Demo credentials display

### Services

**AuthService** (`src/app/services/auth.service.ts`)
- Login/logout functionality
- Token management (localStorage)
- User state management (BehaviorSubject)
- Role checking (isAdmin, hasRole)
- Current user observable

### Security Features

**Auth Interceptor** (`src/app/interceptors/auth.interceptor.ts`)
- Automatically adds JWT token to HTTP requests
- Adds `Authorization: Bearer {token}` header

**Auth Guard** (`src/app/guards/auth.guard.ts`)
- Protects authenticated routes
- Redirects to login page if not authenticated
- Preserves return URL for post-login redirect

### Protected Routes

The following routes require authentication:
- `/search` - Vehicle search page
- `/results` - Search results page
- `/dashboard` - User dashboard

### Usage Example

```typescript
// Login
this.authService.login('username', 'password').subscribe({
  next: () => this.router.navigate(['/dashboard']),
  error: (err) => console.error('Login failed', err)
});

// Check authentication status
const isLoggedIn = this.authService.isAuthenticated();

// Check user role
const isAdmin = this.authService.isAdmin();

// Get current user
this.authService.currentUser$.subscribe(user => {
  console.log(user?.username, user?.roles);
});

// Logout
this.authService.logout();
```

### Demo Credentials

For demonstration purposes:
- **Admin**: username = "admin", any password
- **User**: any other username, any password

### UI Features

- Login/logout button in navigation
- Current username and role display
- Role-based UI elements
- Automatic token inclusion in API requests
- Session persistence via localStorage
