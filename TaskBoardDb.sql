/* ===========================================================================
   TaskBoardDb: co so du lieu cho website quan ly cong viec dang Kanban.
   Chay toan bo file mot lan trong SQL Server Management Studio.
   Canh bao: file nay XOA va TAO LAI co so du lieu TaskBoardDb neu da ton tai.
   Tai khoan mau: admin / Admin@123, an, binh, chi, dung / Member@123
   =========================================================================== */
USE master;
GO
IF DB_ID(N'TaskBoardDb') IS NOT NULL
BEGIN
    ALTER DATABASE TaskBoardDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TaskBoardDb;
END
GO
CREATE DATABASE TaskBoardDb;
GO
USE TaskBoardDb;
GO

/* ---------- 1. Users: tai khoan nguoi dung ---------- */
CREATE TABLE Users (
    UserId       int IDENTITY(1,1) NOT NULL,
    Username     nvarchar(50)  NOT NULL,
    PasswordHash nvarchar(100) NOT NULL,
    PasswordSalt nvarchar(50)  NOT NULL,
    FullName     nvarchar(100) NOT NULL,
    Email        nvarchar(100) NOT NULL,
    Role         nvarchar(20)  NOT NULL CONSTRAINT DF_Users_Role DEFAULT N'Member',
    IsActive     bit           NOT NULL CONSTRAINT DF_Users_IsActive DEFAULT 1,
    CreatedAt    datetime2     NOT NULL CONSTRAINT DF_Users_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Users PRIMARY KEY (UserId),
    CONSTRAINT UQ_Users_Username UNIQUE (Username),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT CK_Users_Role CHECK (Role IN (N'Admin', N'Member'))
);
GO

/* ---------- 2. Projects: du an (moi du an la mot bang Kanban) ---------- */
CREATE TABLE Projects (
    ProjectId   int IDENTITY(1,1) NOT NULL,
    Name        nvarchar(100) NOT NULL,
    Description nvarchar(500) NULL,
    OwnerId     int           NOT NULL,
    CreatedAt   datetime2     NOT NULL CONSTRAINT DF_Projects_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Projects PRIMARY KEY (ProjectId),
    CONSTRAINT FK_Projects_Users FOREIGN KEY (OwnerId) REFERENCES Users(UserId),
    CONSTRAINT CK_Projects_Name CHECK (LEN(LTRIM(RTRIM(Name))) > 0)
);
GO

/* ---------- 3. ProjectMembers: bang trung gian nhieu-nhieu giua Projects va Users ---------- */
CREATE TABLE ProjectMembers (
    ProjectId int       NOT NULL,
    UserId    int       NOT NULL,
    JoinedAt  datetime2 NOT NULL CONSTRAINT DF_ProjectMembers_JoinedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_ProjectMembers PRIMARY KEY (ProjectId, UserId),
    CONSTRAINT FK_ProjectMembers_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    CONSTRAINT FK_ProjectMembers_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

/* ---------- 4. BoardColumns: cac cot cua bang Kanban ---------- */
CREATE TABLE BoardColumns (
    ColumnId  int IDENTITY(1,1) NOT NULL,
    ProjectId int           NOT NULL,
    Title     nvarchar(50)  NOT NULL,
    SortOrder int           NOT NULL CONSTRAINT DF_BoardColumns_SortOrder DEFAULT 0,
    CONSTRAINT PK_BoardColumns PRIMARY KEY (ColumnId),
    CONSTRAINT FK_BoardColumns_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    CONSTRAINT UQ_BoardColumns_Project_Title UNIQUE (ProjectId, Title),
    CONSTRAINT CK_BoardColumns_Title CHECK (LEN(LTRIM(RTRIM(Title))) > 0)
);
GO

/* ---------- 5. Priorities: danh muc muc do uu tien ---------- */
CREATE TABLE Priorities (
    PriorityId    int IDENTITY(1,1) NOT NULL,
    Name          nvarchar(30) NOT NULL,
    PriorityLevel int          NOT NULL,
    ColorCode     nvarchar(7)  NOT NULL,
    CONSTRAINT PK_Priorities PRIMARY KEY (PriorityId),
    CONSTRAINT UQ_Priorities_Name UNIQUE (Name),
    CONSTRAINT CK_Priorities_Level CHECK (PriorityLevel BETWEEN 1 AND 4)
);
GO

/* ---------- 6. Tasks: the cong viec ---------- */
CREATE TABLE Tasks (
    TaskId      int IDENTITY(1,1) NOT NULL,
    ColumnId    int            NOT NULL,
    Title       nvarchar(200)  NOT NULL,
    Description nvarchar(2000) NULL,
    PriorityId  int            NOT NULL,
    AssigneeId  int            NULL,
    CreatedById int            NOT NULL,
    DueDate     datetime2      NULL,
    SortOrder   int            NOT NULL CONSTRAINT DF_Tasks_SortOrder DEFAULT 0,
    CreatedAt   datetime2      NOT NULL CONSTRAINT DF_Tasks_CreatedAt DEFAULT SYSDATETIME(),
    CompletedAt datetime2      NULL,
    CONSTRAINT PK_Tasks PRIMARY KEY (TaskId),
    CONSTRAINT FK_Tasks_BoardColumns FOREIGN KEY (ColumnId) REFERENCES BoardColumns(ColumnId) ON DELETE CASCADE,
    CONSTRAINT FK_Tasks_Priorities FOREIGN KEY (PriorityId) REFERENCES Priorities(PriorityId),
    CONSTRAINT FK_Tasks_Assignee FOREIGN KEY (AssigneeId) REFERENCES Users(UserId),
    CONSTRAINT FK_Tasks_CreatedBy FOREIGN KEY (CreatedById) REFERENCES Users(UserId),
    CONSTRAINT CK_Tasks_Title CHECK (LEN(LTRIM(RTRIM(Title))) > 0),
    CONSTRAINT CK_Tasks_SortOrder CHECK (SortOrder >= 0)
);
GO

/* ---------- 7. Comments: binh luan tren the ---------- */
CREATE TABLE Comments (
    CommentId int IDENTITY(1,1) NOT NULL,
    TaskId    int            NOT NULL,
    UserId    int            NOT NULL,
    Content   nvarchar(1000) NOT NULL,
    CreatedAt datetime2      NOT NULL CONSTRAINT DF_Comments_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_Comments PRIMARY KEY (CommentId),
    CONSTRAINT FK_Comments_Tasks FOREIGN KEY (TaskId) REFERENCES Tasks(TaskId) ON DELETE CASCADE,
    CONSTRAINT FK_Comments_Users FOREIGN KEY (UserId) REFERENCES Users(UserId),
    CONSTRAINT CK_Comments_Content CHECK (LEN(LTRIM(RTRIM(Content))) > 0)
);
GO

/* ---------- 8. ActivityLogs: nhat ky hoat dong cua du an ---------- */
CREATE TABLE ActivityLogs (
    LogId     int IDENTITY(1,1) NOT NULL,
    ProjectId int           NOT NULL,
    UserId    int           NOT NULL,
    Message   nvarchar(500) NOT NULL,
    CreatedAt datetime2     NOT NULL CONSTRAINT DF_ActivityLogs_CreatedAt DEFAULT SYSDATETIME(),
    CONSTRAINT PK_ActivityLogs PRIMARY KEY (LogId),
    CONSTRAINT FK_ActivityLogs_Projects FOREIGN KEY (ProjectId) REFERENCES Projects(ProjectId) ON DELETE CASCADE,
    CONSTRAINT FK_ActivityLogs_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
GO

/* ---------- Chi muc phuc vu truy van thuong dung ---------- */
CREATE INDEX IX_Projects_OwnerId ON Projects(OwnerId);
CREATE INDEX IX_ProjectMembers_UserId ON ProjectMembers(UserId);
CREATE INDEX IX_BoardColumns_ProjectId ON BoardColumns(ProjectId, SortOrder);
CREATE INDEX IX_Tasks_ColumnId ON Tasks(ColumnId, SortOrder);
CREATE INDEX IX_Tasks_AssigneeId ON Tasks(AssigneeId);
CREATE INDEX IX_Tasks_DueDate ON Tasks(DueDate);
CREATE INDEX IX_Comments_TaskId ON Comments(TaskId);
CREATE INDEX IX_ActivityLogs_Project_CreatedAt ON ActivityLogs(ProjectId, CreatedAt DESC);
GO

/* ===========================================================================
   DU LIEU MAU
   =========================================================================== */
INSERT INTO Priorities (Name, PriorityLevel, ColorCode) VALUES
    (N'Thấp', 1, N'#6b7280'),
    (N'Trung bình', 2, N'#2563eb'),
    (N'Cao', 3, N'#b45309'),
    (N'Khẩn cấp', 4, N'#dc2626');
GO

SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserId, Username, PasswordHash, PasswordSalt, FullName, Email, Role, IsActive) VALUES
    (1, N'admin', N'BCqmL4widhdR1FoAa6phU3zb+o0=', N'1U2iSKqENGUTTzcWuY2iRQ==', N'Quản trị viên', N'admin@example.com', N'Admin', 1),
    (2, N'an', N'eujuouvHQjEUvkQq/TmsbeWv1pw=', N'82gx/eGSrT4GdQnu5UZBBg==', N'Nguyễn An', N'an@example.com', N'Member', 1),
    (3, N'binh', N'yutzaopFAT23bwu7RzLWgM/XZ8M=', N'yACX59m6JeRoDQJFppBpcg==', N'Trần Bình', N'binh@example.com', N'Member', 1),
    (4, N'chi', N'xhSk+h1EnXx7TXEurhbiwB5MR90=', N'xbgwJerCmy9ooWeoI594fg==', N'Lê Chi', N'chi@example.com', N'Member', 1),
    (5, N'dung', N'Zc4Ox47ffLsOAX/bxsQrtV13TPo=', N'riVsNISt/32uV4v2AsFIOQ==', N'Phạm Dũng', N'dung@example.com', N'Member', 1);
SET IDENTITY_INSERT Users OFF;
GO

SET IDENTITY_INSERT Projects ON;
INSERT INTO Projects (ProjectId, Name, Description, OwnerId, CreatedAt) VALUES
    (1, N'Đồ án Lập trình Web', N'Công việc nhóm cho đồ án môn Lập trình Web.', 2, DATEADD(DAY, -10, SYSDATETIME())),
    (2, N'Ôn tập cuối kỳ', N'Danh sách nội dung cần ôn trước kỳ thi.', 3, DATEADD(DAY, -6, SYSDATETIME())),
    (3, N'Thiết kế giao diện', N'Chọn màu, font chữ và kiểm tra giao diện.', 4, DATEADD(DAY, -5, SYSDATETIME()));
SET IDENTITY_INSERT Projects OFF;
GO

INSERT INTO ProjectMembers (ProjectId, UserId, JoinedAt) VALUES
    (1, 2, DATEADD(DAY, -10, SYSDATETIME())),
    (1, 3, DATEADD(DAY, -9, SYSDATETIME())),
    (1, 4, DATEADD(DAY, -9, SYSDATETIME())),
    (2, 3, DATEADD(DAY, -6, SYSDATETIME())),
    (2, 5, DATEADD(DAY, -5, SYSDATETIME())),
    (3, 4, DATEADD(DAY, -5, SYSDATETIME())),
    (3, 2, DATEADD(DAY, -4, SYSDATETIME()));
GO

SET IDENTITY_INSERT BoardColumns ON;
INSERT INTO BoardColumns (ColumnId, ProjectId, Title, SortOrder) VALUES
    (1, 1, N'Cần làm', 1), (2, 1, N'Đang làm', 2), (3, 1, N'Hoàn thành', 3),
    (4, 2, N'Cần làm', 1), (5, 2, N'Đang làm', 2), (6, 2, N'Hoàn thành', 3),
    (7, 3, N'Cần làm', 1), (8, 3, N'Đang làm', 2), (9, 3, N'Hoàn thành', 3);
SET IDENTITY_INSERT BoardColumns OFF;
GO

SET IDENTITY_INSERT Tasks ON;
INSERT INTO Tasks (TaskId, ColumnId, Title, Description, PriorityId, AssigneeId, CreatedById, DueDate, SortOrder, CreatedAt, CompletedAt) VALUES
    (1, 3, N'Vẽ sơ đồ ERD', N'Vẽ sơ đồ thực thể và quan hệ cho 8 bảng của hệ thống.', 3, 2, 2, DATEADD(DAY, -2, SYSDATETIME()), 1, DATEADD(DAY, -9, SYSDATETIME()), DATEADD(DAY, -3, SYSDATETIME())),
    (2, 3, N'Viết script SQL tạo bảng', N'Tạo bảng, ràng buộc và dữ liệu mẫu.', 3, 3, 2, DATEADD(DAY, -1, SYSDATETIME()), 2, DATEADD(DAY, -8, SYSDATETIME()), DATEADD(DAY, -2, SYSDATETIME())),
    (3, 2, N'Xây dựng trang đăng nhập', N'Đăng nhập, đăng ký và phân quyền Admin, Member.', 4, 2, 2, DATEADD(DAY, 1, SYSDATETIME()), 1, DATEADD(DAY, -7, SYSDATETIME()), NULL),
    (4, 2, N'Làm giao diện bảng Kanban', N'Các cột, thẻ công việc và bố cục responsive.', 3, 4, 2, DATEADD(DAY, 3, SYSDATETIME()), 2, DATEADD(DAY, -6, SYSDATETIME()), NULL),
    (5, 1, N'Viết chức năng kéo thả thẻ', N'Kéo thẻ giữa các cột và lưu vị trí vào cơ sở dữ liệu.', 3, 3, 2, DATEADD(DAY, 5, SYSDATETIME()), 1, DATEADD(DAY, -5, SYSDATETIME()), NULL),
    (6, 1, N'Viết báo cáo chương 4', N'Thiết kế hệ thống và cơ sở dữ liệu.', 2, NULL, 2, DATEADD(DAY, 10, SYSDATETIME()), 2, DATEADD(DAY, -4, SYSDATETIME()), NULL),
    (7, 4, N'Ôn chương LINQ', N'Ôn Where, Select, Join và GroupBy.', 2, 3, 3, DATEADD(DAY, 2, SYSDATETIME()), 1, DATEADD(DAY, -5, SYSDATETIME()), NULL),
    (8, 5, N'Làm lại bài tập Razor View', N'Làm lại bài về Layout View và Partial View.', 2, 5, 3, DATEADD(DAY, 4, SYSDATETIME()), 1, DATEADD(DAY, -4, SYSDATETIME()), NULL),
    (9, 6, N'Tổng hợp Data Annotation', N'Liệt kê các thuộc tính Required, StringLength và Range.', 1, 3, 3, DATEADD(DAY, -1, SYSDATETIME()), 1, DATEADD(DAY, -5, SYSDATETIME()), DATEADD(DAY, -2, SYSDATETIME())),
    (10, 7, N'Chọn bảng màu và font chữ', N'Chọn màu chính, màu nhấn và font hiển thị.', 1, 4, 4, DATEADD(DAY, 6, SYSDATETIME()), 1, DATEADD(DAY, -4, SYSDATETIME()), NULL),
    (11, 8, N'Kiểm tra giao diện ở 375, 768 và 1366 px', N'Kiểm tra bố cục ở ba chiều rộng màn hình.', 3, 4, 4, DATEADD(DAY, 1, SYSDATETIME()), 1, DATEADD(DAY, -3, SYSDATETIME()), NULL),
    (12, 9, N'Kiểm tra form đăng ký', N'Thử các trường hợp nhập sai dữ liệu.', 2, 2, 4, DATEADD(DAY, -3, SYSDATETIME()), 1, DATEADD(DAY, -5, SYSDATETIME()), DATEADD(DAY, -4, SYSDATETIME()));
SET IDENTITY_INSERT Tasks OFF;
GO

SET IDENTITY_INSERT Comments ON;
INSERT INTO Comments (CommentId, TaskId, UserId, Content, CreatedAt) VALUES
    (1, 3, 3, N'Mình đã kiểm tra phần băm mật khẩu, chạy ổn.', DATEADD(DAY, -2, SYSDATETIME())),
    (2, 3, 2, N'Cảm ơn bạn. Mình sẽ thêm kiểm tra tài khoản bị khóa.', DATEADD(DAY, -1, SYSDATETIME())),
    (3, 4, 4, N'Cột cuối cùng sẽ là cột hoàn thành.', DATEADD(DAY, -1, SYSDATETIME())),
    (4, 7, 3, N'Nên làm thêm ví dụ GroupBy.', DATEADD(HOUR, -20, SYSDATETIME())),
    (5, 11, 4, N'Ở 375 px nút bị tràn, cần chỉnh lại.', DATEADD(HOUR, -6, SYSDATETIME())),
    (6, 1, 2, N'Sơ đồ đã gửi cho cả nhóm.', DATEADD(DAY, -3, SYSDATETIME()));
SET IDENTITY_INSERT Comments OFF;
GO

SET IDENTITY_INSERT ActivityLogs ON;
INSERT INTO ActivityLogs (LogId, ProjectId, UserId, Message, CreatedAt) VALUES
    (1, 1, 2, N'đã tạo dự án', DATEADD(DAY, -10, SYSDATETIME())),
    (2, 1, 2, N'đã thêm thẻ "Vẽ sơ đồ ERD"', DATEADD(DAY, -9, SYSDATETIME())),
    (3, 1, 2, N'đã chuyển thẻ "Vẽ sơ đồ ERD" từ "Đang làm" sang "Hoàn thành"', DATEADD(DAY, -3, SYSDATETIME())),
    (4, 2, 3, N'đã tạo dự án', DATEADD(DAY, -6, SYSDATETIME())),
    (5, 2, 3, N'đã chuyển thẻ "Tổng hợp Data Annotation" từ "Đang làm" sang "Hoàn thành"', DATEADD(DAY, -2, SYSDATETIME())),
    (6, 3, 4, N'đã tạo dự án', DATEADD(DAY, -5, SYSDATETIME()));
SET IDENTITY_INSERT ActivityLogs OFF;
GO

/* Kiem tra nhanh: so dong cua moi bang */
SELECT N'Users' AS TableName, COUNT(*) AS Total FROM Users
UNION ALL SELECT N'Projects', COUNT(*) FROM Projects
UNION ALL SELECT N'ProjectMembers', COUNT(*) FROM ProjectMembers
UNION ALL SELECT N'BoardColumns', COUNT(*) FROM BoardColumns
UNION ALL SELECT N'Priorities', COUNT(*) FROM Priorities
UNION ALL SELECT N'Tasks', COUNT(*) FROM Tasks
UNION ALL SELECT N'Comments', COUNT(*) FROM Comments
UNION ALL SELECT N'ActivityLogs', COUNT(*) FROM ActivityLogs;
GO
