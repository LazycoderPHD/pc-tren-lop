use master
if exists (select * from sysdatabases where name = 'QLDeAnCongTy')
	drop database QLDeAnCongTy
GO
create database QLDeAnCongTy
go
use QLDeAnCongTy

CREATE TABLE NHANVIEN(
	MANV CHAR(3) CONSTRAINT PK_NHANVIEN PRIMARY KEY,
	HONV NVARCHAR(15) NOT NULL,
	TENLOT NVARCHAR(15) NOT NULL,
	TENNV NVARCHAR(15) NOT NULL,
	NGAYSINH DATE NOT NULL,
	DIACHI NVARCHAR(30) NOT NULL,
	PHAI NVARCHAR(3) CHECK (PHAI IN (N'NAM', N'NỮ')),
	LUONG INT DEFAULT (10000),
	MANQL CHAR(3),
	PHG TINYINT NOT NULL)

CREATE TABLE PHONGBAN(
	MAPHG TINYINT CONSTRAINT PK_PHONGBAN PRIMARY KEY,
	TENPHG NVARCHAR(15) NOT NULL,
	TRUONGPHG CHAR(3),
	NGAYNHAMCHUC DATE DEFAULT (GETDATE())
	)

CREATE TABLE DIADIEMPHG(
	MAPHG TINYINT CONSTRAINT DIADIEMPHG_PHONGBAN FOREIGN KEY REFERENCES PHONGBAN(MAPHG),
	DIADIEM NVARCHAR(15) NOT NULL,
	CONSTRAINT PK_DIADIEMPHG PRIMARY KEY(MAPHG, DIADIEM))

CREATE TABLE DEAN(
	MADA TINYINT CONSTRAINT PK_DEAN PRIMARY KEY,
	TENDA NVARCHAR(15) NOT NULL,
	DIADIEMDA NVARCHAR(15) NOT NULL,
	THOIGIAN DECIMAL(5, 2),
	PHONG TINYINT CONSTRAINT FK_DEAN_PHONGBAN FOREIGN KEY REFERENCES PHONGBAN(MAPHG)
	)

CREATE TABLE PHANCONG(
	MANV CHAR(3) CONSTRAINT FK_PHANCONG_NHANVIEN FOREIGN KEY REFERENCES NHANVIEN(MANV),
	SODA TINYINT CONSTRAINT FK_PHANCONG_DEAN FOREIGN KEY REFERENCES DEAN(MADA),
	THOIGIAN FLOAT NOT NULL,
	CONSTRAINT PK_PHANCONG PRIMARY KEY (MANV, SODA))

CREATE TABLE THANNHAN(
	MANVIEN CHAR(3) CONSTRAINT FK_THANNHAN_NHANVIEN FOREIGN KEY REFERENCES NHANVIEN(MANV),
	TENTN NVARCHAR(15),
	NGAYSINH DATE NOT NULL,
	PHAI NVARCHAR(3) CHECK (PHAI IN (N'NAM', N'NỮ')),
	QUANHE NVARCHAR(15) NOT NULL,
	CONSTRAINT PK_THANNHAN PRIMARY KEY (MANVIEN, TENTN))

ALTER TABLE NHANVIEN ADD 
	CONSTRAINT FK_NHANVIEN_NQL FOREIGN KEY (MANQL) REFERENCES NHANVIEN(MANV),
	CONSTRAINT FK_NHANVIEN_PHONGBAN FOREIGN KEY (PHG) REFERENCES PHONGBAN(MAPHG)
	
ALTER TABLE PHONGBAN ADD
	CONSTRAINT FK_PHONGBAN_NHANVIEN FOREIGN KEY (TRUONGPHG) REFERENCES NHANVIEN(MANV)

--sp_databases
--sp_tables
--sp_columns DEAN
--sp_helpconstraint DEAN
--sp_pkeys DEAN
--Nhập liệu
--SET DATEFORMAT DMY

INSERT INTO PHONGBAN (MAPHG, TENPHG, TRUONGPHG, NGAYNHAMCHUC)
VALUES
	(5, N'Nghiên cứu', NULL, '19780522'),
	(4, N'Điều hành', NULL, '19850101'),
	(1, N'Quản lý', NULL, '19710619')

--SP_COLUMNS NHANVIEN
INSERT INTO NHANVIEN --(MANV, HONV, TENLOT, TENNV, NGAYSINH, DIACHI, PHAI, LUONG, MANQL, PHG) 
VALUES
	('009', N'Đinh', N'Bá', N'Tiến', '02/11/1960', N'119 Cống Quỳnh, Tp HCM', N'Nam', 30000, '005', 5),
	('005', N'Nguyễn', N'Thanh', N'Tùng', '19620820', N'222 Nguyễn Văn Cừ, Tp HCM', N'Nam', 40000, '006', 5),
	('007', N'Bùi', N'Ngọc', N'Hằng', '19540311', N'332 Nguyễn Thái Học, Tp HCM', N'Nữ', 25000, '001', 4),
	('001', N'Lê', N'Quỳnh', N'Như', '19670201', N'291 Hồ Văn Huê, Tp HCM', N'Nữ', 43000, '006', 4),
	('004', N'Nguyễn', N'Mạnh', N'Hùng', '19670304', N'95 Bà Rịa, Vũng Tàu', N'Nam', 38000, '005', 5),
	('003', N'Trần', N'Thanh', N'Tâm', '19570504', N'34 Mai Thị Lự, Tp HCM', N'Nữ', 25000, '005', 5),
	('008', N'Trần', N'Hồng', N'Quang', '19670901', N'80 Lê Hồng Phong, Tp HCM', N'Nam', 25000, '001', 4),
	('006', N'Phạm', N'Văn', N'Vinh', '19650101', N'45 Trưng Vương, Hà Nội', N'Nam', 55000, null, 1),
	('010', N'Phạm', N'Minh', N'Hùng', '19851005', N'256 Lê Đại Hành, Tp HCM', N'Nam', 35000, '001', 1),
	('011', N'Phạm', N'Thanh', N'Kiệt', '19800414', N'80 Lê Hồng Phong, Tp HCM', N'Nam', 52000, '005', 1)

--Cập nhật trưởng phòng cho phòng ban
UPDATE PHONGBAN
SET TRUONGPHG = '009'
WHERE MAPHG = 5

UPDATE PHONGBAN
SET TRUONGPHG = '008'
WHERE MAPHG = 4

UPDATE PHONGBAN
SET TRUONGPHG = '006'
WHERE MAPHG = 1

INSERT INTO DIADIEMPHG (MAPHG, DIADIEM)
VALUES
	(1, N'TP HCM'),
	(4, N'Hà Nội'),
	(5, N'Vũng Tàu'),
	(5, N'Nha Trang'), 
	(5, N'TP HCM')

INSERT INTO DEAN (MADA, TENDA, DIADIEMDA, PHONG, THOIGIAN)
VALUES
	(1, N'Sản phẩm X', N'Vũng Tàu', 5, 80),
	(2, N'Sản phẩm Y', N'Nha Trang', 5, 60),
	(3, N'Sản phẩm Z', N'TP HCM', 5, 100),
	(10, N'Tin học hoá', N'Hà Nội', 4, 150),
	(20, N'Cáp quang', N'TP HCM', 1, 75),
	(30, N'Đào tạo', N'Hà Nội', 4, 60)

INSERT INTO THANNHAN (MANVIEN, TENTN, NGAYSINH, PHAI, QUANHE)
VALUES
	('005', N'Trinh', '19760405', N'Nữ', N'Con gái'),
	('005', N'Khang', '19731025', N'Nam', N'Con trai'),
	('005', N'Phương', '19480503', N'Nữ', N'Vợ'),
	('001', N'Minh', '19320229', N'Nam', N'Chồng'),
	('009', N'Tiến', '19780101', N'Nam', N'Con trai'),
	('009', N'Châu', '19781230', N'Nữ', N'Con gái'),
	('009', N'Phương', '19570505', N'Nữ', N'Vợ')

INSERT INTO PHANCONG (MANV, SODA, THOIGIAN)
VALUES
	('009', 1, 32),
	('009', 2, 8),
	('004', 3, 40),
	('003', 1, 20.0),
	('003', 2, 20.0),
	('008', 10, 35),
	('008', 30, 5),
	('001', 30, 20),
	('001', 20, 15),
	('006', 20, 30),
	('005', 3, 10),
	('005', 10, 10),
	('005', 20, 10),
	('007', 30, 30),
	('007', 10, 10),
	('010', 3, 20),
	('010', 10, 15),
	('011', 1, 5),
	('011', 2, 5),
	('011', 3, 5),
	('011', 10, 5),
	('011', 20, 5),
	('011', 30, 5)

select * from dean
select * from DIADIEMPHG
select * from nhanvien
select * from phancong
select * from phongban
select * from thannhan
--A1. Find employees who work on Project 1 OR Project 2, but NOT on both.

select * from nhanvien
where manv IN (select MANV from phancong
                where soda = 1 
                and MANV not in(select MANV from phancong
                                    where soda = 2))

UNION

select * from nhanvien
where manv IN (select MANV from phancong
                where soda = 2 
                and MANV not in(select MANV from phancong
                                    where soda = 1))

--A2. List all employees along with their department name, supervisor name, and number of projects they work on.
select NV.MANV, NV.HONV, NV.TENLOT, NV.TENNV, TENPHG, NVQL.TENNV as [NHAN VIEN QUAN LY], count(*) as [SL SoDA]
from NHANVIEN as [NV]
join phongban as [PB] on NV.PHG = PB.MAPHG
    left join NHANVIEN as [NVQL] ON NV.MANQL = NVQL.MANV
    left join PHANCONG as [PC] on nv.manv = pc.MANV
group by NV.MANV, NV.HONV, NV.TENLOT, NV.TENNV, TENPHG, NVQL.TENNV

--A3. Create a report showing departments with their project count, including departments with zero projects.
SELECT PB.TENPHG, COUNT(DA.MADA) AS [SoLuongDeAn]
FROM PHONGBAN PB
LEFT JOIN DEAN DA ON PB.MAPHG = DA.PHONG
GROUP BY PB.TENPHG;

--A4. Find employees whose salary is higher than the average salary of their department.
SELECT HONV, TENNV, LUONG, PHG
FROM NHANVIEN NV
WHERE LUONG > (SELECT AVG(LUONG) FROM NHANVIEN WHERE PHG = NV.PHG)

--Task B: Division Operation and "For All" Queries (30 minutes)
--Exercise B1: Find employees who work on ALL projects managed by Department 5.
SELECT * FROM NHANVIEN NV
WHERE NOT EXISTS (
    SELECT MADA FROM DEAN WHERE PHONG = 5
    EXCEPT
    SELECT SODA FROM PHANCONG WHERE MANV = NV.MANV
)

--Exercise B2: List employees who have worked on ALL projects (regardless of department).
SELECT * FROM NHANVIEN NV
WHERE NOT EXISTS (
    SELECT MADA FROM DEAN
    EXCEPT
    SELECT SODA FROM PHANCONG WHERE MANV = NV.MANV
)

--Exercise B3: Find departments where ALL employees earn more than 30,000.
select HONV, TENLOT, TENNV, NV.LUONG, NV.PHG from phongban as [pb]
join nhanvien as [nv]
on pb.MAPHG = nv.phg
where luong > 30000

--Exercise B4: Identify employees who have dependents of ALL relationship types present in the database.
SELECT * FROM NHANVIEN NV
WHERE NOT EXISTS (
    SELECT DISTINCT QUANHE FROM THANNHAN
    EXCEPT
    SELECT QUANHE FROM THANNHAN WHERE MANVIEN = NV.MANV
)

--Exercise B5: Find projects that have team members from ALL departments.
SELECT * FROM DEAN DA
WHERE NOT EXISTS (
    SELECT MAPHG FROM PHONGBAN
    EXCEPT
    SELECT NV.PHG 
    FROM PHANCONG PC 
    JOIN NHANVIEN NV ON PC.MANV = NV.MANV 
    WHERE PC.SODA = DA.MADA
);

--Task C: Nested Queries and Subqueries (35 minutes)
--Exercise C1: Find employees whose salary is higher than the average salary of their own department.

SELECT NV.HONV, NV.TENNV, NV.LUONG
FROM NHANVIEN NV
WHERE NV.LUONG > (SELECT AVG(LUONG) FROM NHANVIEN WHERE PHG = NV.PHG);

--Exercise C2: List projects that have more assigned employees than the average number of employees per project.

SELECT SODA, COUNT(MANV) AS SoNV
FROM PHANCONG
GROUP BY SODA
HAVING COUNT(MANV) > (
    SELECT AVG(CAST(CountNV AS FLOAT))
    FROM (SELECT COUNT(MANV) AS CountNV FROM PHANCONG GROUP BY SODA) AS Temp
);

--Exercise C3: Find employees who work on at least one project in a different city than their department location.

SELECT DISTINCT NV.*
FROM NHANVIEN NV
JOIN PHANCONG PC ON NV.MANV = PC.MANV
JOIN DEAN DA ON PC.SODA = DA.MADA
WHERE DA.DIADIEMDA NOT IN (SELECT DIADIEM FROM DIADIEMPHG WHERE MAPHG = NV.PHG);

--Exercise C4: Identify departments where the manager's salary is lower than at least one employee in that department.

SELECT PB.MAPHG, PB.TENPHG
FROM PHONGBAN PB
JOIN NHANVIEN MGR ON PB.TRUONGPHG = MGR.MANV
WHERE MGR.LUONG < ANY (SELECT LUONG FROM NHANVIEN WHERE PHG = PB.MAPHG);

--Exercise C5: Find the employee(s) with the second-highest salary in the company.

SELECT * FROM NHANVIEN
WHERE LUONG = (SELECT MAX(LUONG) FROM NHANVIEN 
               WHERE LUONG < (SELECT MAX(LUONG) FROM NHANVIEN));

--Exercise C6: List employees who have more dependents than their direct supervisor.

SELECT NV.MANV, NV.TENNV
FROM NHANVIEN NV
WHERE (SELECT COUNT(*) FROM THANNHAN WHERE MANVIEN = NV.MANV) >
      (SELECT COUNT(*) FROM THANNHAN WHERE MANVIEN = NV.MANQL);

--Exercise C7: Find projects that have NO employees from Department 1 working on them.

SELECT * FROM DEAN
WHERE MADA NOT IN (
    SELECT DISTINCT SODA FROM PHANCONG PC 
    JOIN NHANVIEN NV ON PC.MANV = NV.MANV 
    WHERE NV.PHG = 1
);

--Task D: Advanced SQL Functions (35 minutes)
--Exercise D1: Display employee full names in uppercase format: "LAST, FIRST MIDDLE".

SELECT UPPER(HONV + ', ' + TENLOT + ' ' + TENNV) AS FullName FROM NHANVIEN;

--Exercise D2: Calculate each employee's age and years of service (from NG_NHANCHUC for managers).

SELECT NV.TENNV, 
       DATEDIFF(YEAR, NV.NGAYSINH, GETDATE()) AS Tuoi,
       DATEDIFF(YEAR, PB.NGAYNHAMCHUC, GETDATE()) AS NamCongTac
FROM NHANVIEN NV
JOIN PHONGBAN PB ON NV.MANV = PB.TRUONGPHG;

--Exercise D3: Extract the domain name from employee addresses (e.g., "TP.HCM" from full address).

SELECT DIACHI, 
       LTRIM(SUBSTRING(DIACHI, LEN(DIACHI) - CHARINDEX(',', REVERSE(DIACHI)) + 2, LEN(DIACHI))) AS TinhThanh
FROM NHANVIEN;

--Exercise D4: Calculate each project's total cost assuming each hour costs 500,000 VND.

SELECT SODA, SUM(THOIGIAN) * 500000 AS TongChiPhi
FROM PHANCONG
GROUP BY SODA;

--Exercise D5: Find employees whose birthday month is the current month.

SELECT * FROM NHANVIEN WHERE MONTH(NGAYSINH) = MONTH(GETDATE());

--Exercise D6: Round all employee salaries to the nearest thousand and display percentage above minimum wage (25,000).

SELECT 
    TENNV, 
    LUONG AS LuongGoc,
    ROUND(LUONG, -3) AS LuongLamTron,
    CAST(((LUONG - 25000) / 25000.0) * 100 AS DECIMAL(10,2)) AS PhanTramTrenMucToiThieu
FROM NHANVIEN;

--Exercise D7: Generate email addresses for all employees using format: firstname.lastname@company.vn (lowercase, no Vietnamese characters).

SELECT 
    TENNV, 
    HONV,
    LOWER(CONCAT(TENNV, '.', HONV, '@company.vn')) AS EmailGoiY
FROM NHANVIEN;

--Task E: Conditional Logic with CASE Expressions (25 minutes)
--Exercise E1: Categorize employees into salary grades: "Entry" (<30K), "Mid" (30-40K), "Senior" (40-50K), "Executive" (>50K).

SELECT TENNV, LUONG,
CASE 
    WHEN LUONG < 30000 THEN 'Entry'
    WHEN LUONG BETWEEN 30000 AND 40000 THEN 'Mid'
    WHEN LUONG BETWEEN 40001 AND 50000 THEN 'Senior'
    ELSE 'Executive'
END AS SalaryGrade
FROM NHANVIEN;

--Exercise E2: Display project workload status: "Underutilized" (<20 total hours), "Normal" (20-50), "Heavy" (>50).

SELECT SODA, SUM(THOIGIAN) AS TongGio,
CASE 
    WHEN SUM(THOIGIAN) < 20 THEN 'Underutilized'
    WHEN SUM(THOIGIAN) BETWEEN 20 AND 50 THEN 'Normal'
    ELSE 'Heavy'
END AS WorkloadStatus
FROM PHANCONG
GROUP BY SODA;

--Exercise E3: Create a seniority report showing retirement eligibility: "Eligible" (age ≥60 for men, ≥55 for women) or "Not Eligible".

SELECT TENNV, PHAI, DATEDIFF(YEAR, NGAYSINH, GETDATE()) AS Tuoi,
CASE 
    WHEN PHAI = N'NAM' AND DATEDIFF(YEAR, NGAYSINH, GETDATE()) >= 60 THEN 'Eligible'
    WHEN PHAI = N'NỮ' AND DATEDIFF(YEAR, NGAYSINH, GETDATE()) >= 55 THEN 'Eligible'
    ELSE 'Not Eligible'
END AS RetirementStatus
FROM NHANVIEN;

--Exercise E4: Calculate employee bonuses: 15% for those with >3 dependents, 10% for 1-3 dependents, 5% for no dependents.

SELECT NV.TENNV, 
CASE 
    WHEN COUNT(TN.TENTN) > 3 THEN NV.LUONG * 0.15
    WHEN COUNT(TN.TENTN) BETWEEN 1 AND 3 THEN NV.LUONG * 0.10
    ELSE NV.LUONG * 0.05
END AS BonusAmount
FROM NHANVIEN NV
LEFT JOIN THANNHAN TN ON NV.MANV = TN.MANVIEN
GROUP BY NV.MANV, NV.TENNV, NV.LUONG;

--Exercise E5: Classify projects by location: "Metropolitan" (TP.HCM, Hà Nội), "Coastal" (Nha Trang, Vũng Tàu), "Other".

SELECT TENDA, DIADIEMDA,
CASE 
    WHEN DIADIEMDA IN (N'TP HCM', N'Hà Nội') THEN 'Metropolitan'
    WHEN DIADIEMDA IN (N'Nha Trang', N'Vũng Tàu') THEN 'Coastal'
    ELSE 'Other'
END AS LocationClass
FROM DEAN;

--Exercise E6: Generate a performance summary: Show if employee hours exceed department average ("Above Average") or not.

SELECT NV.TENNV, SUM(PC.THOIGIAN) AS TongGio,
CASE 
    WHEN SUM(PC.THOIGIAN) > (
        SELECT AVG(TongGioPhong) 
        FROM (SELECT SUM(THOIGIAN) AS TongGioPhong FROM PHANCONG GROUP BY MANV) AS Temp
    ) THEN 'Above Average'
    ELSE 'Below/At Average'
END AS PerformanceRating
FROM NHANVIEN NV
LEFT JOIN PHANCONG PC ON NV.MANV = PC.MANV
GROUP BY NV.MANV, NV.TENNV;

--Task F: Comprehensive Challenge Problems (20 minutes)
--Exercise F1: Create a comprehensive employee performance report including: full name, department, age, years of service, number of projects, total hours worked, average hours per project, salary grade, and performance rating (based on hours vs department average).

SELECT 
    NV.HONV + ' ' + NV.TENLOT + ' ' + NV.TENNV AS HoTen,
    PB.TENPHG,
    DATEDIFF(YEAR, NV.NGAYSINH, GETDATE()) AS Tuoi,
    DATEDIFF(YEAR, NV.NGAYSINH, GETDATE()) - 20 AS NamCongTac, -- Giả định bắt đầu làm từ 20 tuổi
    COUNT(DISTINCT PC.SODA) AS SoDuAn,
    SUM(PC.THOIGIAN) AS TongGio,
    CASE 
        WHEN NV.LUONG < 30000 THEN 'Entry'
        WHEN NV.LUONG <= 50000 THEN 'Mid/Senior'
        ELSE 'Executive'
    END AS SalaryGrade,
    CASE 
        WHEN SUM(PC.THOIGIAN) > 30 THEN 'High Performer'
        ELSE 'Standard'
    END AS Performance
FROM NHANVIEN NV
JOIN PHONGBAN PB ON NV.PHG = PB.MAPHG
LEFT JOIN PHANCONG PC ON NV.MANV = PC.MANV
GROUP BY NV.MANV, NV.HONV, NV.TENLOT, NV.TENNV, PB.TENPHG, NV.NGAYSINH, NV.LUONG;

--Exercise F2: Find "universal employees" who work on ALL projects in their department AND have dependents of at least 2 different relationship types.

SELECT NV.MANV, NV.TENNV
FROM NHANVIEN NV
WHERE NOT EXISTS (
    SELECT MADA FROM DEAN WHERE PHONG = NV.PHG
    EXCEPT
    SELECT SODA FROM PHANCONG WHERE MANV = NV.MANV
)
AND NV.MANV IN (
    SELECT MANVIEN FROM THANNHAN 
    GROUP BY MANVIEN HAVING COUNT(DISTINCT QUANHE) >= 2
);

--Exercise F3: Generate a department comparison report showing: department name, number of employees, total projects, average employee salary, highest paid employee name, and budget utilization (total project hours * 500,000).

SELECT 
    TENPHG,
    COUNT(DISTINCT NV.MANV) AS SoNhanVien,
    COUNT(DISTINCT DA.MADA) AS SoDeAn,
    AVG(NV.LUONG) AS LuongTrungBinh
FROM PHONGBAN PB
LEFT JOIN NHANVIEN NV ON PB.MAPHG = NV.PHG
LEFT JOIN DEAN DA ON PB.MAPHG = DA.PHONG
GROUP BY PB.MAPHG, PB.TENPHG;

--Exercise F4: Identify potential project manager candidates: employees who (1) work on at least 3 projects, (2) have salary in top 25% of their department, (3) have at least 2 dependents (showing stability), and (4) are between 30-50 years old.

SELECT NV.MANV, NV.TENNV, NV.LUONG
FROM NHANVIEN NV
WHERE 
    (SELECT COUNT(*) FROM PHANCONG WHERE MANV = NV.MANV) >= 3
    AND NV.LUONG >= (SELECT AVG(LUONG) FROM NHANVIEN WHERE PHG = NV.PHG)
    AND (SELECT COUNT(*) FROM THANNHAN WHERE MANVIEN = NV.MANV) >= 2
    AND DATEDIFF(YEAR, NV.NGAYSINH, GETDATE()) BETWEEN 30 AND 50;