# Passkey (WebAuthn) Implementation Plan for Blazor App

## Overview
This document outlines the implementation plan for adding passkey/WebAuthn authentication to the AccountsPOC Blazor application.

## Current State
- ASP.NET Core Identity is set up with JWT authentication
- No passkey/WebAuthn implementation exists
- .NET 10 ASP.NET Core Identity includes built-in passkey support via `IdentityUserPasskey<TKey>`

## Implementation Requirements

### 1. Database Schema Updates

#### Update ApplicationDbContext
The `ApplicationDbContext` needs to be updated to include passkey support:

```csharp
public class ApplicationDbContext : IdentityDbContext<User, Role, int, 
    IdentityUserClaim<int>, 
    UserRole, 
    IdentityUserLogin<int>, 
    IdentityRoleClaim<int>, 
    IdentityUserToken<int>,
    IdentityUserPasskey<int>>  // Add this parameter
{
    // Add DbSet for passkeys
    public DbSet<IdentityUserPasskey<int>> UserPasskeys => Set<IdentityUserPasskey<int>>();
    
    // Rest of DbContext...
}
```

#### Create Migration
```bash
dotnet ef migrations add AddPasskeySupport --project src/AccountsPOC.Infrastructure
dotnet ef database update --project src/AccountsPOC.Infrastructure
```

### 2. Server-Side API Endpoints

#### PasskeysController.cs
Create a new controller for passkey operations:

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PasskeysController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly IPasskeyService _passkeyService;

    [HttpGet("options/register")]
    public async Task<IActionResult> GetRegisterOptions()
    {
        // Generate registration options
        // Return challenge, user info, and credential creation options
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterPasskey([FromBody] PasskeyRegistrationDto dto)
    {
        // Verify registration response
        // Store credential in database
        // Return success/failure
    }

    [HttpGet("options/authenticate")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAuthenticateOptions([FromQuery] string? username)
    {
        // Generate authentication options
        // Return challenge and allowed credentials
    }

    [HttpPost("authenticate")]
    [AllowAnonymous]
    public async Task<IActionResult> AuthenticateWithPasskey([FromBody] PasskeyAuthenticationDto dto)
    {
        // Verify authentication response
        // Issue JWT token on success
        // Return token and user info
    }

    [HttpGet("list")]
    public async Task<IActionResult> ListPasskeys()
    {
        // Return list of user's registered passkeys
    }

    [HttpDelete("{credentialId}")]
    public async Task<IActionResult> DeletePasskey(string credentialId)
    {
        // Remove passkey from database
    }
}
```

### 3. Server-Side Services

#### IPasskeyService.cs
```csharp
public interface IPasskeyService
{
    Task<CredentialCreateOptions> GenerateRegistrationOptions(User user);
    Task<bool> VerifyAndStoreCredential(User user, PasskeyRegistrationDto registration);
    Task<AssertionOptions> GenerateAuthenticationOptions(string? username = null);
    Task<(bool Success, User? User)> VerifyAuthentication(PasskeyAuthenticationDto authentication);
    Task<List<PasskeyInfo>> GetUserPasskeys(int userId);
    Task<bool> DeletePasskey(int userId, string credentialId);
}
```

#### PasskeyService.cs
Implement the service using a WebAuthn library like `Fido2NetLib`:

```csharp
public class PasskeyService : IPasskeyService
{
    private readonly IFido2 _fido2;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;

    // Implementation of WebAuthn challenge generation and verification
}
```

### 4. Client-Side Blazor Components

#### PasskeyRegistration.razor
Component for registering a new passkey:

```razor
@page "/security/passkey-register"
@inject HttpClient Http
@inject IJSRuntime JSRuntime
@attribute [Authorize]

<h3>Register a Passkey</h3>

<button @onclick="RegisterPasskey">Register New Passkey</button>

@code {
    private async Task RegisterPasskey()
    {
        // 1. Get registration options from server
        var options = await Http.GetFromJsonAsync<RegistrationOptions>("api/passkeys/options/register");
        
        // 2. Call WebAuthn API
        var credential = await JSRuntime.InvokeAsync<object>("createCredential", options);
        
        // 3. Send credential to server
        var result = await Http.PostAsJsonAsync("api/passkeys/register", credential);
        
        // 4. Show success/error message
    }
}
```

#### PasskeyLogin.razor (or update Login.razor)
Add passkey login option to existing login page:

```razor
<button @onclick="LoginWithPasskey">Sign in with Passkey</button>

@code {
    private async Task LoginWithPasskey()
    {
        // 1. Get authentication options
        var options = await Http.GetFromJsonAsync<AuthenticationOptions>($"api/passkeys/options/authenticate?username={email}");
        
        // 2. Call WebAuthn API
        var assertion = await JSRuntime.InvokeAsync<object>("getAssertion", options);
        
        // 3. Send assertion to server
        var result = await Http.PostAsJsonAsync("api/passkeys/authenticate", assertion);
        
        // 4. Handle authentication result (store token, redirect)
    }
}
```

### 5. JavaScript Interop

#### wwwroot/js/webauthn.js
Create JavaScript functions for WebAuthn API calls:

```javascript
// Convert base64url to ArrayBuffer
function base64urlToBuffer(base64url) {
    const base64 = base64url.replace(/-/g, '+').replace(/_/g, '/');
    const binary = atob(base64);
    const buffer = new ArrayBuffer(binary.length);
    const bytes = new Uint8Array(buffer);
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }
    return buffer;
}

// Convert ArrayBuffer to base64url
function bufferToBase64url(buffer) {
    const bytes = new Uint8Array(buffer);
    let binary = '';
    for (let i = 0; i < bytes.length; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    const base64 = btoa(binary);
    return base64.replace(/\+/g, '-').replace(/\//g, '_').replace(/=/g, '');
}

// Register a new credential
window.createCredential = async function(options) {
    // Convert base64url strings to ArrayBuffers
    options.challenge = base64urlToBuffer(options.challenge);
    options.user.id = base64urlToBuffer(options.user.id);
    
    if (options.excludeCredentials) {
        options.excludeCredentials = options.excludeCredentials.map(cred => ({
            ...cred,
            id: base64urlToBuffer(cred.id)
        }));
    }
    
    // Call WebAuthn API
    const credential = await navigator.credentials.create({ publicKey: options });
    
    // Convert response to JSON
    return {
        id: credential.id,
        rawId: bufferToBase64url(credential.rawId),
        type: credential.type,
        response: {
            clientDataJSON: bufferToBase64url(credential.response.clientDataJSON),
            attestationObject: bufferToBase64url(credential.response.attestationObject),
        }
    };
};

// Authenticate with existing credential
window.getAssertion = async function(options) {
    // Convert base64url strings to ArrayBuffers
    options.challenge = base64urlToBuffer(options.challenge);
    
    if (options.allowCredentials) {
        options.allowCredentials = options.allowCredentials.map(cred => ({
            ...cred,
            id: base64urlToBuffer(cred.id)
        }));
    }
    
    // Call WebAuthn API
    const assertion = await navigator.credentials.get({ publicKey: options });
    
    // Convert response to JSON
    return {
        id: assertion.id,
        rawId: bufferToBase64url(assertion.rawId),
        type: assertion.type,
        response: {
            clientDataJSON: bufferToBase64url(assertion.response.clientDataJSON),
            authenticatorData: bufferToBase64url(assertion.response.authenticatorData),
            signature: bufferToBase64url(assertion.response.signature),
            userHandle: assertion.response.userHandle ? bufferToBase64url(assertion.response.userHandle) : null
        }
    };
};
```

### 6. NuGet Packages Required

Add to `AccountsPOC.WebAPI.csproj`:

```xml
<PackageReference Include="Fido2.NetFramework" Version="3.0.1" />
<!-- or -->
<PackageReference Include="Fido2" Version="3.0.1" />
```

### 7. Configuration

#### appsettings.json
```json
{
  "Fido2": {
    "ServerDomain": "localhost",
    "ServerName": "AccountsPOC",
    "Origin": "https://localhost:7001",
    "TimestampDriftTolerance": 300000
  }
}
```

#### Program.cs
```csharp
// Add Fido2 services
builder.Services.AddFido2(options =>
{
    options.ServerDomain = builder.Configuration["Fido2:ServerDomain"];
    options.ServerName = builder.Configuration["Fido2:ServerName"];
    options.Origins = new HashSet<string> { builder.Configuration["Fido2:Origin"] };
    options.TimestampDriftTolerance = int.Parse(builder.Configuration["Fido2:TimestampDriftTolerance"]);
});

// Add passkey service
builder.Services.AddScoped<IPasskeyService, PasskeyService>();
```

## Implementation Phases

### Phase 1: Foundation (Week 1)
1. Update ApplicationDbContext with passkey support
2. Create and run database migration
3. Install Fido2 NuGet package
4. Configure Fido2 in Program.cs

### Phase 2: Server-Side (Week 2)
1. Implement PasskeyService with registration and authentication
2. Create PasskeysController with all endpoints
3. Add DTOs for passkey operations
4. Test with API client (Postman)

### Phase 3: Client-Side (Week 3)
1. Create webauthn.js JavaScript file
2. Build PasskeyRegistration component
3. Update Login page with passkey option
4. Create PasskeyManagement component (list/delete)

### Phase 4: Integration & Testing (Week 4)
1. End-to-end testing of registration flow
2. End-to-end testing of authentication flow
3. Browser compatibility testing (Chrome, Edge, Safari, Firefox)
4. Mobile device testing
5. Documentation and user guide

## Security Considerations

1. **HTTPS Required**: WebAuthn only works over HTTPS (localhost is allowed for development)
2. **Origin Validation**: Always validate the origin in authentication requests
3. **Challenge Uniqueness**: Generate unique challenges for each operation
4. **Timeout**: Set appropriate timeout values for user actions
5. **User Verification**: Consider requiring user verification (PIN/biometric)
6. **Credential Storage**: Store credentials securely, never expose private keys
7. **Rate Limiting**: Implement rate limiting on authentication endpoints

## Browser Support

| Browser | Support |
|---------|---------|
| Chrome 67+ | ✅ Full Support |
| Edge 18+ | ✅ Full Support |
| Firefox 60+ | ✅ Full Support |
| Safari 14+ | ✅ Full Support (macOS 11+, iOS 14+) |
| Opera 54+ | ✅ Full Support |

## Fallback Authentication

- Always maintain password authentication as fallback
- Allow users to have multiple passkeys
- Provide recovery options if passkey is lost

## Testing Checklist

- [ ] Registration with platform authenticator (Windows Hello, Touch ID)
- [ ] Registration with security key (YubiKey, etc.)
- [ ] Authentication with platform authenticator
- [ ] Authentication with security key
- [ ] Multiple passkeys per user
- [ ] Passkey deletion
- [ ] Cross-platform use (register on desktop, use on mobile)
- [ ] Error handling (user cancels, timeout, invalid credential)

## Resources

- [WebAuthn Guide](https://webauthn.guide/)
- [Fido2NetLib Documentation](https://github.com/passwordless-lib/fido2-net-lib)
- [Microsoft: Enable passwordless sign-in with Microsoft Authenticator](https://learn.microsoft.com/en-us/azure/active-directory/authentication/howto-authentication-passwordless-phone)
- [W3C WebAuthn Specification](https://www.w3.org/TR/webauthn-2/)

## Current Status

**NOT IMPLEMENTED** - This document provides the complete implementation plan. Due to the complexity of WebAuthn implementation (requires multiple layers of cryptographic operations, JavaScript interop, and careful security considerations), this feature requires dedicated development time.

## Recommendation

Consider using a managed passkey provider like:
- **Hanko** - Open source WebAuthn solution
- **Auth0** - Supports passkeys
- **Azure AD B2C** - Supports FIDO2
- **Passage by 1Password** - Passkey-first authentication

This would significantly reduce implementation complexity and security risks.
