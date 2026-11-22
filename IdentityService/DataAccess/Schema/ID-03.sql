-- Upgrade an existing Identity database; new databases use the same EF model.
SET XACT_ABORT ON;
BEGIN TRANSACTION;

DECLARE @lockResult int;
EXEC @lockResult = sys.sp_getapplock
    @Resource = N'IdentityService.Schema.ID-03',
    @LockMode = 'Exclusive',
    @LockOwner = 'Transaction';
IF @lockResult < 0 THROW 50001, 'Could not lock the Identity schema for ID-03.', 1;

IF COL_LENGTH('dbo.AspNetUsers', 'IsEnabled') IS NULL
    ALTER TABLE dbo.AspNetUsers ADD IsEnabled bit NOT NULL
        CONSTRAINT DF_AspNetUsers_IsEnabled DEFAULT (1) WITH VALUES;

IF OBJECT_ID('dbo.RefreshSessions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshSessions (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_RefreshSessions PRIMARY KEY,
        UserId nvarchar(450) NOT NULL,
        TokenHash char(64) NOT NULL,
        ExpiresAtUtc datetimeoffset NOT NULL,
        CONSTRAINT FK_RefreshSessions_AspNetUsers_UserId FOREIGN KEY (UserId)
            REFERENCES dbo.AspNetUsers(Id) ON DELETE CASCADE
    );
    CREATE UNIQUE INDEX IX_RefreshSessions_TokenHash ON dbo.RefreshSessions(TokenHash);
    CREATE INDEX IX_RefreshSessions_UserId ON dbo.RefreshSessions(UserId);
END;

COMMIT TRANSACTION;
