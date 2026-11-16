# IdentityService – Architecture

## Overview

IdentityService is an independent microservice responsible for user identity management, authentication, and session management within the FitnessApp platform. It is a single ASP.NET Core Web API project with no shared libraries or dependencies on other microservices.

## Architectural Style

### Vertical Slice Architecture (VSA)

The service is organized by **feature** rather than by technical layer. Each feature (e.g., `RegisterUser`, `Login`) is a self-contained vertical slice containing its own Endpoint, Command, Handler, and Validator.

### CQRS

Commands and Queries are separated. Each feature slice contains a dedicated Command (or Query) and Handler.

### Minimal APIs

All HTTP endpoints use ASP.NET Core Minimal APIs. No MVC controllers.

### Result Pattern

Handlers return domain results, not HTTP responses. The Result Pattern is used inside Handlers to communicate success or failure.

### ProblemDetails

HTTP error responses are formatted using the ProblemDetails standard (RFC 9457) at the Endpoint boundary.

---

## Folder Responsibilities

### `Features/`

Contains all vertical slices grouped by business capability:

| Group                | Slices                                          |
|----------------------|-------------------------------------------------|
| `Registration`       | `RegisterUser`                                  |
| `EmailVerification`  | `VerifyEmail`, `ResendVerificationCode`         |
| `Authentication`     | `Login`                                         |
| `Sessions`           | `RefreshToken`, `Logout`                        |
| `PasswordRecovery`   | `RequestPasswordReset`, `CompletePasswordReset` |
| `PasswordManagement` | `ChangePassword`                                |

Each slice contains:

- **Endpoint.cs** – Maps the HTTP route and translates between HTTP and the application layer.
- **Command.cs** – Represents the request payload for the operation.
- **Handler.cs** – Executes the business workflow using the Result Pattern.
- **Validator.cs** – Validates the command before the handler executes (present only where input validation is required).

### `Domain/`

Contains the core business model. This is a folder inside the Web API project, not a separate class library.

- **`Entities/`** – Rich Domain Entities (`User`, `VerificationChallenge`, `RefreshSession`) that encapsulate business rules, protect invariants, and expose domain behaviors through methods. These are not anemic data containers.
- **`Enums/`** – Domain enumerations representing business concepts.
- **`ValueObjects/`** – Reserved for immutable value types if needed in the future.

### `Persistence/`

Contains database concerns:

- **`Configurations/`** – EF Core entity type configurations.
- **`Migrations/`** – EF Core migrations.

### `Infrastructure/`

Contains external service integrations:

- **`Email/`** – `IEmailSender` abstraction and email provider implementations.
  - **`Providers/`** – Concrete email provider implementations (Mailtrap/Mailpit for development, Resend/Brevo for production).
- **`Authentication/`** – JWT generation, token hashing, and related authentication infrastructure.
- **`BackgroundJobs/`** – Background services for cleaning expired verification challenges and refresh sessions.

### `Common/`

Cross-cutting types used within the service:

- **`Results/`** – Result Pattern types.
- **`Errors/`** – Domain error definitions.

### `Extensions/`

Service registration and middleware extension methods.

---

## Rich Domain Model

Domain Entities are the **only** place where business rules live. Each entity:

1. Protects its internal state through encapsulation (private setters, private constructors).
2. Exposes meaningful public methods representing domain behaviors.
3. Enforces its own business invariants.
4. Prevents invalid state transitions.
5. Does not depend on infrastructure concerns (no database, no HTTP, no email sending).

---

## Service Independence

IdentityService is completely independent:

- No shared libraries or BuildingBlocks.
- No shared components between microservices.
- Each microservice owns its own database and domain model.
- Communication between services (if needed) happens through well-defined contracts, not shared code.
