# IdentityService

## Purpose

IdentityService is the authentication and identity management microservice for the FitnessApp platform. It handles user registration, email verification, login, session management, password recovery, and password changes.

## Main Capabilities

- **User Registration** – Create new user accounts with email and password.
- **Email Verification** – Verify user email addresses via one-time passwords (OTP).
- **Authentication** – Login with email and password, with account lockout protection.
- **Session Management** – Issue and rotate JWT access/refresh token pairs.
- **Password Recovery** – Request and complete password resets via email verification.
- **Password Management** – Change password for authenticated users.

## Folder Organization

```
IdentityService/
├── Features/           # Vertical slices grouped by business capability
├── Domain/             # Rich Domain Entities, Enums, Value Objects
├── Persistence/        # EF Core configurations and migrations
├── Infrastructure/     # Email, authentication, and background job services
├── Common/             # Result pattern and error types
└── Extensions/         # Service registration extension methods
```

## Architectural Principles

- **Vertical Slice Architecture** – Each feature is a self-contained slice (Endpoint, Command, Handler, Validator).
- **CQRS** – Commands and queries are separated.
- **Minimal APIs** – No MVC controllers.
- **Result Pattern** – Handlers return domain results, not HTTP responses.
- **ProblemDetails** – Standardized error responses at the HTTP boundary.
- **Rich Domain Model** – Business rules live inside Domain Entities, not in Handlers.
- **Independence** – No shared libraries, no shared components between microservices.

## Security Decisions

- **Custom Identity Model** – ASP.NET Core Identity is not used.
- **BCrypt** – Password hashing uses BCrypt.
- **SHA-256 Hashed Tokens** – OTP and refresh tokens are stored as SHA-256 hashes only.
- **Refresh Token Rotation** – Each token use issues a new token and revokes the old one.
- **Account Lockout** – 5 failed login attempts trigger a 15-minute lockout.
- **JWT Configuration** – 15-minute access tokens, 7-day refresh tokens, zero clock skew.
- **OTP Expiration** – 10-minute lifetime, maximum 5 attempts per challenge.

## Current Implementation Status

### Implemented

- ✅ Rich Domain Entities (`User`, `VerificationChallenge`, `RefreshSession`) with full business rule encapsulation.
- ✅ Domain enums (`ChallengePurpose`).
- ✅ Complete folder structure following Vertical Slice Architecture.
- ✅ Skeleton files for all feature slices (Endpoints, Commands, Handlers, Validators).
- ✅ `IEmailSender` interface.
- ✅ Architecture documentation.

### Not Yet Implemented

- ⬜ Feature slice logic (Endpoints, Handlers, Validators).
- ⬜ Persistence layer (DbContext, entity configurations, migrations).
- ⬜ Infrastructure integrations (email providers, JWT, background jobs).
- ⬜ Result and Error types implementation.
- ⬜ Service registration and dependency injection.
- ⬜ `Program.cs` configuration.
- ⬜ NuGet package installation (BCrypt, EF Core, JWT, etc.).
