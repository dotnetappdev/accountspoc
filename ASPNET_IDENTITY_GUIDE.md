# ASP.NET Identity Implementation Guide

## Overview

This document describes the complete ASP.NET Identity implementation for the AccountsPOC application, including JWT authentication, user management, role-based authorization, and Blazor integration.

## Architecture

### Backend (WebAPI)
- **ASP.NET Core Identity** for user management and authentication
- **JWT (JSON Web Tokens)** for stateless authentication
- **Entity Framework Core** with SQLite for data persistence
- **Role-based authorization** with custom roles and permissions

### Frontend (Blazor Server)
- **Custom AuthenticationStateProvider** for managing auth state
- **Login and Register pages** for user authentication
- **JWT token management** in memory (can be upgraded to secure storage)
- **Route protection** with AuthorizeRouteView

## Key Components

### 1. Domain Entities

#### User Entity
```csharp
public class User : IdentityUser<int>
{
    public int TenantId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
```

#### Role Entity
```csharp
public class Role : IdentityRole<int>
{
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; } = false;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
```

### 2. Authentication API Endpoints

#### POST /api/auth/register
Registers a new user with the system.

**Request:**
```json
{
  "tenantId": 1,
  "username": "john.doe",
  "email": "john.doe@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "roleNames": ["User"]
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "john.doe@example.com",
  "username": "john.doe",
  "userId": 1,
  "tenantId": 1
}
```

#### POST /api/auth/login
Authenticates a user and returns a JWT token.

**Request:**
```json
{
  "email": "john.doe@example.com",
  "password": "Password123!"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "email": "john.doe@example.com",
  "username": "john.doe",
  "userId": 1,
  "tenantId": 1
}
```

#### GET /api/auth/me
Returns information about the currently authenticated user.

**Headers:**
```
Authorization: Bearer {token}
```

**Response:**
```json
{
  "userId": 1,
  "username": "john.doe",
  "email": "john.doe@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+1234567890",
  "tenantId": 1,
  "isActive": true,
  "roles": ["User"]
}
```

#### POST /api/auth/logout
Logs out the current user (client-side token deletion).

### 3. Default Roles and Users

The system seeds the following default roles and users:

**Roles:**
- **Administrator** - Full system access
- **Support** - Tenant and customer management access
- **Agent** - Limited customer data access
- **User** - Standard user access

**Default Users:**
| Username | Email | Password | Role |
|----------|-------|----------|------|
| admin | admin@accountspoc.com | Admin123! | Administrator |
| support | support@accountspoc.com | Support123! | Support |
| agent | agent@accountspoc.com | Agent123! | Agent |

### 4. JWT Configuration

JWT settings are configured in `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyForJWTTokenGenerationPleaseChangeThis12345",
    "Issuer": "AccountsPOC",
    "Audience": "AccountsPOCApp",
    "ExpireDays": "7"
  }
}
```

**Important:** Change the JWT Key in production!

### 5. Password Requirements

- Minimum 6 characters
- Requires at least one uppercase letter
- Requires at least one lowercase letter
- Requires at least one digit
- Non-alphanumeric characters are optional

## Blazor Integration

### Authentication State Provider

The `CustomAuthenticationStateProvider` manages authentication state in Blazor:

```csharp
public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    // Validates JWT token and creates ClaimsPrincipal
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    
    // Logs in user and stores JWT token
    public async Task<bool> LoginAsync(string email, string password)
    
    // Registers new user and stores JWT token
    public async Task<bool> RegisterAsync(RegisterDto dto)
    
    // Logs out user and clears JWT token
    public async Task LogoutAsync()
}
```

### Protected Routes

Routes are protected using `AuthorizeRouteView` in `Routes.razor`:

```razor
<AuthorizeRouteView RouteData="routeData" DefaultLayout="typeof(Layout.MainLayout)">
    <NotAuthorized>
        @if (context.User.Identity?.IsAuthenticated != true)
        {
            <RedirectToLogin />
        }
        else
        {
            <p role="alert">You are not authorized to access this resource.</p>
        }
    </NotAuthorized>
</AuthorizeRouteView>
```

### Login Page

URL: `/login`

Features:
- Email/username and password authentication
- Error message display
- Link to registration page
- Loading state during authentication

### Register Page

URL: `/register`

Features:
- Tenant selection
- Username, email, password fields
- Optional: First name, last name, phone number
- Password confirmation
- Link to login page
- Loading state during registration

## Security Features

### 1. Password Hashing
- Uses ASP.NET Identity's built-in PBKDF2 password hashing
- Secure by default with salting and key derivation

### 2. JWT Token Security
- Tokens are signed with HMAC-SHA256
- Include user ID, email, username, tenant ID, and roles as claims
- Configurable expiration (default 7 days)

### 3. Multi-Tenancy Support
- Each user belongs to a specific tenant
- Tenant ID is included in JWT claims
- Can be used for tenant-scoped queries and operations

### 4. Role-Based Authorization
- Roles are stored in database and linked to users
- Roles included as claims in JWT token
- Can use `[Authorize(Roles = "Admin")]` attribute on controllers

## Testing

### Test Authentication with cURL

#### Register a new user:
```bash
curl -X POST http://localhost:5049/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "tenantId": 1,
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "Test",
    "lastName": "User"
  }'
```

#### Login:
```bash
curl -X POST http://localhost:5049/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@accountspoc.com",
    "password": "Admin123!"
  }'
```

#### Get current user info:
```bash
TOKEN="your-jwt-token-here"
curl http://localhost:5049/api/auth/me \
  -H "Authorization: Bearer $TOKEN"
```

## Future Enhancements

### Recommended Improvements

1. **Token Refresh**
   - Implement refresh tokens for long-lived sessions
   - Short-lived access tokens (15 minutes)
   - Long-lived refresh tokens (7 days)

2. **Secure Token Storage**
   - Use `ProtectedBrowserStorage` or secure cookies in Blazor
   - Implement token encryption at rest

3. **Email Confirmation**
   - Require email verification on registration
   - Send confirmation emails with tokens

4. **Two-Factor Authentication (2FA)**
   - Support authenticator apps (TOTP)
   - SMS-based 2FA

5. **Password Reset**
   - Forgot password functionality
   - Email-based password reset tokens

6. **Account Lockout**
   - Lock accounts after failed login attempts
   - Configurable lockout duration

7. **Claims-Based Permissions**
   - Granular permission checking
   - Custom authorization policies

8. **Audit Logging**
   - Log all authentication events
   - Track user activities

9. **Social Login**
   - OAuth integration (Google, Microsoft, etc.)
   - External identity providers

10. **Session Management**
    - View active sessions
    - Revoke tokens/sessions
    - Force logout from all devices

## Troubleshooting

### Common Issues

#### 1. Invalid Token Errors
- Ensure JWT key matches between client and server
- Check token expiration
- Verify token format

#### 2. CORS Issues
- Ensure Blazor app origin is in CORS policy
- Check CORS middleware order in Program.cs

#### 3. Database Issues
- Delete `AccountsPOC.db` to recreate database
- Run migrations if schema changes

#### 4. Authentication Not Working in Blazor
- Check browser console for errors
- Verify API is running and accessible
- Ensure HttpClient base address is correct

## Conclusion

This implementation provides a solid foundation for authentication and authorization in the AccountsPOC application. It follows ASP.NET Core best practices and provides a clean separation between authentication logic and business logic.

The system is production-ready with the recommended enhancements implemented, particularly around token refresh, secure storage, and email confirmation.
