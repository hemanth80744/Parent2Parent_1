-- =============================================
-- Parent2Parent Database Schema
-- Generated for SmarterASP.NET Deployment
-- =============================================

-- 1. Create Tables

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Name VARCHAR(100) NOT NULL,
        Username VARCHAR(50) NOT NULL UNIQUE,
        Password VARCHAR(100) NOT NULL,
        School VARCHAR(150) NOT NULL,
        Class VARCHAR(50) NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Requests')
BEGIN
    CREATE TABLE Requests (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SenderId INT NOT NULL,
        ReceiverId INT NOT NULL,
        Status VARCHAR(50) DEFAULT 'Pending',
        CreatedAt DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_Requests_Sender FOREIGN KEY (SenderId) REFERENCES Users(Id),
        CONSTRAINT FK_Requests_Receiver FOREIGN KEY (ReceiverId) REFERENCES Users(Id)
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Messages')
BEGIN
    CREATE TABLE Messages (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        SenderId INT NOT NULL,
        ReceiverId INT NOT NULL,
        Message VARCHAR(MAX) NOT NULL,
        SentAt DATETIME DEFAULT GETDATE(),
        CONSTRAINT FK_Messages_Sender FOREIGN KEY (SenderId) REFERENCES Users(Id),
        CONSTRAINT FK_Messages_Receiver FOREIGN KEY (ReceiverId) REFERENCES Users(Id)
    );
END
GO

-- 2. Create Stored Procedures

-- User Registration
CREATE OR ALTER PROCEDURE sp_register_user
    @Name VARCHAR(100),
    @Username VARCHAR(50),
    @Password VARCHAR(100),
    @School VARCHAR(150),
    @Class VARCHAR(50)
AS
BEGIN
    INSERT INTO Users (Name, Username, Password, School, Class)
    VALUES (@Name, @Username, @Password, @School, @Class);
END
GO

-- User Login
CREATE OR ALTER PROCEDURE sp_login_user
    @Username VARCHAR(50),
    @Password VARCHAR(100)
AS
BEGIN
    SELECT Id, Name FROM Users 
    WHERE Username = @Username AND Password = @Password;
END
GO

-- Search Schools/Parents
CREATE OR ALTER PROCEDURE sp_search_school
    @SchoolName VARCHAR(150)
AS
BEGIN
    SELECT Id, Name, Class AS ChildClass 
    FROM Users 
    WHERE School LIKE '%' + @SchoolName + '%';
END
GO

-- Send Connection Request
CREATE OR ALTER PROCEDURE sp_send_request
    @SenderId INT,
    @ReceiverId INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM Requests WHERE SenderId = @SenderId AND ReceiverId = @ReceiverId AND Status = 'Pending')
    BEGIN
        INSERT INTO Requests (SenderId, ReceiverId, Status)
        VALUES (@SenderId, @ReceiverId, 'Pending');
    END
END
GO

-- View Requests (Incoming or Outgoing)
CREATE OR ALTER PROCEDURE sp_view_requests
    @UserId INT = NULL,
    @SenderId INT = NULL
AS
BEGIN
    IF @UserId IS NOT NULL
    BEGIN
        -- Incoming requests
        SELECT 
            r.Id AS RequestId,
            r.SenderId,
            r.ReceiverId,
            u.Name AS SenderName,
            r.Status,
            r.CreatedAt
        FROM Requests r
        JOIN Users u ON r.SenderId = u.Id
        WHERE r.ReceiverId = @UserId;
    END
    ELSE IF @SenderId IS NOT NULL
    BEGIN
        -- Outgoing requests
        SELECT 
            r.Id AS RequestId,
            r.SenderId,
            r.ReceiverId,
            u.Name AS ReceiverName,
            r.Status,
            r.CreatedAt
        FROM Requests r
        JOIN Users u ON r.ReceiverId = u.Id
        WHERE r.SenderId = @SenderId;
    END
END
GO

-- Accept Request
CREATE OR ALTER PROCEDURE sp_accept_request
    @RequestId INT
AS
BEGIN
    UPDATE Requests SET Status = 'Accepted' WHERE Id = @RequestId;
END
GO

-- Reject Request
CREATE OR ALTER PROCEDURE sp_reject_request
    @RequestId INT
AS
BEGIN
    UPDATE Requests SET Status = 'Rejected' WHERE Id = @RequestId;
END
GO

-- Send Message
CREATE OR ALTER PROCEDURE sp_send_message
    @SenderId INT,
    @ReceiverId INT,
    @Message VARCHAR(MAX)
AS
BEGIN
    -- Check if they are connected (optional but recommended)
    IF EXISTS (
        SELECT 1 FROM Requests 
        WHERE Status = 'Accepted' 
        AND ((SenderId = @SenderId AND ReceiverId = @ReceiverId) 
             OR (SenderId = @ReceiverId AND ReceiverId = @SenderId))
    )
    BEGIN
        INSERT INTO Messages (SenderId, ReceiverId, Message)
        VALUES (@SenderId, @ReceiverId, @Message);
    END
    ELSE
    BEGIN
        PRINT 'Users are not connected.';
    END
END
GO

-- Get Chat Messages
CREATE OR ALTER PROCEDURE sp_get_messages
    @User1 INT,
    @User2 INT
AS
BEGIN
    SELECT SenderId, ReceiverId, Message, SentAt
    FROM Messages
    WHERE (SenderId = @User1 AND ReceiverId = @User2)
       OR (SenderId = @User2 AND ReceiverId = @User1)
    ORDER BY SentAt ASC;
END
GO
