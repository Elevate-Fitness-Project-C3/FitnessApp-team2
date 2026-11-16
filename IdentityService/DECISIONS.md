# IdentityService – Architectural Decisions

This document records all confirmed architectural decisions and distinguishes them from unresolved technical details.

---

## Confirmed Decisions

### 1. Custom Identity Model

**Decision:** Use a custom identity model. Do NOT use ASP.NET Core Identity.

**Rationale:** Full control over the user entity, authentication flow, and session management without the overhead and constraints of the ASP.NET Core Identity framework.

---

### 2. Password Hashing – BCrypt

**Decision:** Use BCrypt for password hashing.

**Details:** Password hashing and verification are handled by an infrastructure abstraction, not inside the Domain Entity. The Domain Entity manages the password hash value only.

---

### 3. Separate VerificationChallenges Table

**Decision:** Store email verification challenges in a dedicated `VerificationChallenges` table, separate from the `Users` table.

**Details:**
- OTP is stored as a SHA-256 hash only. Plaintext OTP is never persisted.
- OTP expiration: 10 minutes.
- Maximum OTP attempts: 5 per challenge.
- Challenges track consumed/expired/exhausted state.

---

### 4. Separate RefreshSessions Table

**Decision:** Store refresh token sessions in a dedicated `RefreshSessions` table.

**Details:**
- Refresh tokens are stored as SHA-256 hashes only. Plaintext refresh tokens are never persisted.
- Refresh Token Rotation: each token use issues a new token and revokes the old session.
- Sessions track the replacement chain via `ReplacedBySessionId`.

---

### 5. SHA-256 Token Hashing

**Decision:** All stored OTP values and refresh tokens are SHA-256 hashed before persistence.

**Details:** Token hashing is performed in the application/infrastructure layer, not inside Domain Entities.

---

### 6. Refresh Token Rotation

**Decision:** Implement Refresh Token Rotation.

**Details:** When a refresh token is used, the existing session is revoked and a new session with a new token is created. This limits the window of exposure for stolen tokens.

---

### 7. Background Job for Cleanup

**Decision:** Implement a background job to clean expired `VerificationChallenges` and `RefreshSessions` records.

**Status:** Skeleton created. Implementation deferred.

---

### 8. IEmailSender Abstraction

**Decision:** Use an `IEmailSender` interface to abstract email sending.

**Providers:**
- Development: Mailtrap or Mailpit.
- Production: Resend or Brevo.

**Status:** Interface created. Provider implementations deferred.

---

### 9. JWT Configuration

**Decision:**
- Access Token lifetime: 15 minutes.
- Refresh Token lifetime: 7 days.
- `ClockSkew = TimeSpan.Zero` (no tolerance for clock drift).
- Claims: `sub` (user ID), `email`, `role`, `jti` (JWT ID).

**Status:** Configuration deferred to infrastructure implementation.

---

### 10. Rich Domain Model

**Decision:** Domain Entities use Rich Domain Model principles.

**Details:**
- Entities encapsulate business rules and enforce invariants.
- Private constructors and setters protect internal state.
- Public methods represent domain behaviors.
- No anemic data containers.

---

### 11. Account Lockout

**Decision:** After 5 failed login attempts, password login is locked for 15 minutes.

**Details:**
- Failed login counter is tracked on the `User` entity.
- Successful login resets the counter and clears lockout.
- Lockout is enforced by the entity itself.

---

## Unresolved Technical Details

The following points require decisions during implementation:

1. **Email provider selection** – Final choice between Mailtrap vs. Mailpit (dev) and Resend vs. Brevo (prod).
2. **OTP format** – Length, character set, and generation strategy for one-time passwords.
3. **Database provider** – Specific RDBMS and connection strategy.
4. **Rate limiting** – Whether to apply endpoint-level rate limiting beyond the domain lockout.
5. **Logging strategy** – Structured logging configuration and sensitive data masking.
6. **Health checks** – Whether IdentityService exposes health check endpoints.
7. **Background job scheduling** – Cleanup interval and batch size for expired record removal.
