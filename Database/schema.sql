CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    Role NVARCHAR(50) NOT NULL,  -- Requester, Approver, Admin
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

CREATE TABLE Requests (
    RequestId INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    Department NVARCHAR(100) NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Pending', -- Pending, Approved, Rejected
    CreatedByUserId INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CONSTRAINT FK_Requests_Users FOREIGN KEY (CreatedByUserId) REFERENCES Users(UserId)
);

CREATE TABLE RequestApprovals (
    ApprovalId INT IDENTITY PRIMARY KEY,
    RequestId INT NOT NULL,
    ApproverUserId INT NOT NULL,
    Action NVARCHAR(20) NULL,  -- Approved/Rejected
    Comments NVARCHAR(500) NULL,
    ActionAt DATETIME2 NULL,
    CONSTRAINT FK_Approvals_Request FOREIGN KEY (RequestId) REFERENCES Requests(RequestId),
    CONSTRAINT FK_Approvals_User FOREIGN KEY (ApproverUserId) REFERENCES Users(UserId)
);

CREATE TABLE AuditLogs (
    AuditId INT IDENTITY PRIMARY KEY,
    RequestId INT NULL,
    Action NVARCHAR(100) NOT NULL,
    DoneByUserId INT NULL,
    Details NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);
