-- ============================================================================
-- SECTION 1: DATABASE CREATION
-- ============================================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'QLDeAnCongTy_Lab03P2')
BEGIN
    ALTER DATABASE QLDeAnCongTy_Lab03P2 SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE QLDeAnCongTy_Lab03P2;
    PRINT 'Existing QLDeAnCongTy_Lab03P2 database has been dropped.';
END
GO

CREATE DATABASE QLDeAnCongTy_Lab03P2;
GO

PRINT 'QLDeAnCongTy_Lab03P2 database created successfully.';
GO

USE QLDeAnCongTy_Lab03P2;
GO

-- ============================================================================
-- SECTION 2: TABLE CREATION
-- ============================================================================

-- Table: PHONGBAN (Department)
CREATE TABLE PHONGBAN
(
    TENPHG          NVARCHAR(30)    NOT NULL,       -- Department name
    MAPHG           INT             NOT NULL,       -- Department code
    TRPHG           CHAR(9)         NULL,           -- Manager employee ID
    NG_NHANCHUC     DATE            NULL,           -- Manager start date
    
    CONSTRAINT PK_PHONGBAN PRIMARY KEY (MAPHG)
);
GO

-- Table: DIADIEM_PHG (Department Locations)
CREATE TABLE DIADIEM_PHG
(
    MAPHG           INT             NOT NULL,       -- Department code
    DIADIEM         NVARCHAR(50)    NOT NULL,       -- Location
    
    CONSTRAINT PK_DIADIEM_PHG PRIMARY KEY (MAPHG, DIADIEM)
);
GO

-- Table: NHANVIEN (Employee)
CREATE TABLE NHANVIEN
(
    HONV            NVARCHAR(20)    NOT NULL,       -- Last name
    TENLOT          NVARCHAR(20)    NULL,           -- Middle name
    TENNV           NVARCHAR(20)    NOT NULL,       -- First name
    MANV            CHAR(9)         NOT NULL,       -- Employee ID
    NGAYSINH        DATE            NOT NULL,       -- Date of birth
    DCHI            NVARCHAR(100)   NOT NULL,       -- Address
    PHAI            NVARCHAR(5)     NOT NULL,       -- Gender
    LUONG           DECIMAL(10,2)   NOT NULL,       -- Salary
    MA_NQL          CHAR(9)         NULL,           -- Supervisor ID
    PHG             INT             NOT NULL,       -- Department number
    
    CONSTRAINT PK_NHANVIEN PRIMARY KEY (MANV),
    CONSTRAINT CHK_PHAI CHECK (PHAI IN (N'Nam', N'Nữ')),
    CONSTRAINT CHK_LUONG CHECK (LUONG >= 0)
);
GO

-- Table: DEAN (Project)
CREATE TABLE DEAN
(
    TENDA           NVARCHAR(50)    NOT NULL,       -- Project name
    MADA            INT             NOT NULL,       -- Project code
    DDIEM_DA        NVARCHAR(50)    NOT NULL,       -- Project location
    PHONG           INT             NOT NULL,       -- Department managing project
    
    CONSTRAINT PK_DEAN PRIMARY KEY (MADA)
);
GO

-- Table: PHANCONG (Project Assignment)
CREATE TABLE PHANCONG
(
    MA_NVIEN        CHAR(9)         NOT NULL,       -- Employee ID
    SODA            INT             NOT NULL,       -- Project code
    THOIGIAN        DECIMAL(5,1)    NOT NULL,       -- Hours worked
    
    CONSTRAINT PK_PHANCONG PRIMARY KEY (MA_NVIEN, SODA),
    CONSTRAINT CHK_THOIGIAN CHECK (THOIGIAN >= 0)
);
GO

-- Table: THANNHAN (Dependent)
CREATE TABLE THANNHAN
(
    MA_NVIEN        CHAR(9)         NOT NULL,       -- Employee ID
    TENTN           NVARCHAR(30)    NOT NULL,       -- Dependent name
    PHAI            NVARCHAR(5)     NOT NULL,       -- Gender
    NGAYSINH        DATE            NOT NULL,       -- Date of birth
    QUANHE          NVARCHAR(20)    NOT NULL,       -- Relationship
    
    CONSTRAINT PK_THANNHAN PRIMARY KEY (MA_NVIEN, TENTN),
    CONSTRAINT CHK_THANNHAN_PHAI CHECK (PHAI IN (N'Nam', N'Nữ'))
);
GO

PRINT 'All tables created successfully.';
GO

-- ============================================================================
-- SECTION 3: FOREIGN KEY CONSTRAINTS
-- ============================================================================

-- PHONGBAN foreign keys
ALTER TABLE PHONGBAN
ADD CONSTRAINT FK_PHONGBAN_TRPHG 
    FOREIGN KEY (TRPHG) REFERENCES NHANVIEN(MANV);
GO

-- DIADIEM_PHG foreign keys
ALTER TABLE DIADIEM_PHG
ADD CONSTRAINT FK_DIADIEM_PHG_MAPHG 
    FOREIGN KEY (MAPHG) REFERENCES PHONGBAN(MAPHG);
GO

-- NHANVIEN foreign keys
ALTER TABLE NHANVIEN
ADD CONSTRAINT FK_NHANVIEN_MA_NQL 
    FOREIGN KEY (MA_NQL) REFERENCES NHANVIEN(MANV);
GO

ALTER TABLE NHANVIEN
ADD CONSTRAINT FK_NHANVIEN_PHG 
    FOREIGN KEY (PHG) REFERENCES PHONGBAN(MAPHG);
GO

-- DEAN foreign keys
ALTER TABLE DEAN
ADD CONSTRAINT FK_DEAN_PHONG 
    FOREIGN KEY (PHONG) REFERENCES PHONGBAN(MAPHG);
GO

-- PHANCONG foreign keys
ALTER TABLE PHANCONG
ADD CONSTRAINT FK_PHANCONG_MA_NVIEN 
    FOREIGN KEY (MA_NVIEN) REFERENCES NHANVIEN(MANV);
GO

ALTER TABLE PHANCONG
ADD CONSTRAINT FK_PHANCONG_SODA 
    FOREIGN KEY (SODA) REFERENCES DEAN(MADA);
GO

-- THANNHAN foreign keys
ALTER TABLE THANNHAN
ADD CONSTRAINT FK_THANNHAN_MA_NVIEN 
    FOREIGN KEY (MA_NVIEN) REFERENCES NHANVIEN(MANV);
GO

PRINT 'All foreign key constraints added successfully.';
GO

-- ============================================================================
-- SECTION 4: DATA INSERTION
-- ============================================================================

-- Insert PHONGBAN (Departments) - Insert without TRPHG first
INSERT INTO PHONGBAN (TENPHG, MAPHG, TRPHG, NG_NHANCHUC) VALUES
(N'Nghiên cứu', 5, NULL, NULL),
(N'Điều hành', 4, NULL, NULL),
(N'Quản lý', 1, NULL, NULL);
GO

-- Insert NHANVIEN (Employees)
INSERT INTO NHANVIEN (HONV, TENLOT, TENNV, MANV, NGAYSINH, DCHI, PHAI, LUONG, MA_NQL, PHG) VALUES
(N'Đinh', N'Bá', N'Tiên', '123456789', '1965-01-09', N'731 Trần Hưng Đạo, Q5, TP.HCM', N'Nam', 30000, '333445555', 5),
(N'Nguyễn', N'Thanh', N'Tùng', '333445555', '1955-12-08', N'638 Nguyễn Văn Cừ, Q5, TP.HCM', N'Nam', 40000, '888665555', 5),
(N'Bùi', N'Ngọc', N'Hằng', '453453453', '1972-07-31', N'543 Mai Thị Lựu, Q1, TP.HCM', N'Nữ', 25000, '333445555', 5),
(N'Lê', N'Quỳnh', N'Như', '666884444', '1962-09-15', N'975 Bà Triệu, TP. Huế', N'Nữ', 43000, '333445555', 4),
(N'Trần', N'Thanh', N'Tâm', '888665555', '1937-11-10', N'450 Trần Hưng Đạo, TP.HCM', N'Nam', 55000, NULL, 1),
(N'Trần', N'Hồng', N'Quang', '987654321', '1969-03-29', N'22 Nguyễn Tri Phương, Q.10, TP.HCM', N'Nam', 38000, '888665555', 4),
(N'Nguyễn', N'Mạnh', N'Hùng', '987987987', '1969-01-19', N'22B Nguyễn Tri Phương, Q.10, TP.HCM', N'Nam', 25000, '987654321', 4),
(N'Phạm', N'Văn', N'Vinh', '999887777', '1965-07-19', N'34 Mai Thị Lựu, Q.1, TP.HCM', N'Nam', 25000, '987654321', 4);
GO

-- Update PHONGBAN with department managers
UPDATE PHONGBAN SET TRPHG = '333445555', NG_NHANCHUC = '1988-05-22' WHERE MAPHG = 5;
UPDATE PHONGBAN SET TRPHG = '888665555', NG_NHANCHUC = '1971-06-19' WHERE MAPHG = 1;
UPDATE PHONGBAN SET TRPHG = '987654321', NG_NHANCHUC = '1985-01-01' WHERE MAPHG = 4;
GO

-- Insert DIADIEM_PHG (Department Locations)
INSERT INTO DIADIEM_PHG (MAPHG, DIADIEM) VALUES
(1, N'TP.HCM'),
(4, N'Hà Nội'),
(5, N'Nha Trang'),
(5, N'TP.HCM'),
(5, N'Vũng Tàu');
GO

-- Insert DEAN (Projects)
INSERT INTO DEAN (TENDA, MADA, DDIEM_DA, PHONG) VALUES
(N'Sản phẩm X', 1, N'Vũng Tàu', 5),
(N'Sản phẩm Y', 2, N'Nha Trang', 5),
(N'Sản phẩm Z', 3, N'TP.HCM', 5),
(N'Tin học hóa', 10, N'Hà Nội', 4),
(N'Cáp quang', 20, N'TP.HCM', 1),
(N'Đào tạo', 30, N'Hà Nội', 4);
GO

-- Insert PHANCONG (Project Assignments)
INSERT INTO PHANCONG (MA_NVIEN, SODA, THOIGIAN) VALUES
('123456789', 1, 32.5),
('123456789', 2, 7.5),
('333445555', 2, 10.0),
('333445555', 3, 10.0),
('333445555', 10, 10.0),
('333445555', 20, 10.0),
('453453453', 1, 20.0),
('453453453', 2, 20.0),
('666884444', 3, 40.0),
('888665555', 20, 0.0),
('987654321', 20, 15.0),
('987654321', 30, 20.0),
('987987987', 10, 35.0),
('987987987', 30, 5.0),
('999887777', 10, 10.0),
('999887777', 30, 30.0);
GO

-- Insert THANNHAN (Dependents)
INSERT INTO THANNHAN (MA_NVIEN, TENTN, PHAI, NGAYSINH, QUANHE) VALUES
('123456789', N'Minh', N'Nam', '1978-01-01', N'Con trai'),
('123456789', N'Mai', N'Nữ', '1976-04-05', N'Con gái'),
('123456789', N'Phương', N'Nữ', '1968-05-03', N'Vợ chồng'),
('333445555', N'Châu', N'Nữ', '1960-05-05', N'Vợ chồng'),
('333445555', N'Tiên', N'Nam', '1978-10-25', N'Con trai'),
('333445555', N'Xuân', N'Nam', '1973-02-22', N'Con trai'),
('987654321', N'Hà', N'Nữ', '1976-05-03', N'Vợ chồng');
GO

PRINT 'All sample data inserted successfully.';
GO