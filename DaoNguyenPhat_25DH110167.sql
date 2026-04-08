/*
	Họ tên: Đào Nguyên Phát
	Mã sinh viên: 25DH110167
*/
CREATE DATABASE DB_MSSV;
GO
USE DB_MSSV;
CREATE TABLE KHACHHANG (
    MaKH CHAR(8) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE,
    DienThoai CHAR(10),
    DiaChi NVARCHAR(200),
    NgayDangKy DATE DEFAULT GETDATE()
);

CREATE TABLE DANHMUC (
    MaDM CHAR(5) PRIMARY KEY,
    TenDM NVARCHAR(100) NOT NULL UNIQUE,
    MoTa NVARCHAR(500)
);
CREATE TABLE SANPHAM (
    MaSP CHAR(8) PRIMARY KEY,
    TenSP NVARCHAR(200) NOT NULL,
    MaDM CHAR(5),
    GiaBan DECIMAL(15,0) CHECK (GiaBan > 0),
    TonKho INT DEFAULT 0 CHECK (TonKho >= 0),
    TrangThai NVARCHAR(20) DEFAULT 'Dang ban'
        CHECK (TrangThai IN (N'Dang ban', N'Ngung ban', N'Het hang')),
    FOREIGN KEY (MaDM) REFERENCES DANHMUC(MaDM)
);

CREATE TABLE DONHANG (
    MaDH CHAR(10) PRIMARY KEY,
    MaKH CHAR(8) NOT NULL,
    MaSP CHAR(8) NOT NULL,
    SoLuong INT CHECK (SoLuong >= 1),
    DonGia DECIMAL(15,0),
    NgayDat DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(20) DEFAULT N'Cho xac nhan'
        CHECK (TrangThai IN (N'Cho xac nhan', N'Dang giao', N'Da giao', N'Da huy')),
    FOREIGN KEY (MaKH) REFERENCES KHACHHANG(MaKH),
    FOREIGN KEY (MaSP) REFERENCES SANPHAM(MaSP)
);
-- DANHMUC
INSERT INTO DANHMUC VALUES
('DM001', N'Dien thoai', NULL),
('DM002', N'Laptop', NULL),
('DM003', N'Phu kien', NULL);

-- KHACHHANG
INSERT INTO KHACHHANG VALUES
('KH000001', N'Nguyen Van A', 'a@gmail.com', '0123456789', N'HCM', GETDATE()),
('KH000002', N'Tran Van B', 'b@gmail.com', '0123456790', N'HN', GETDATE()),
('KH000003', N'Le Van C', 'c@yahoo.com', '0123456791', N'DN', GETDATE()),
('KH000004', N'Pham Van D', 'd@gmail.com', '0123456792', N'HCM', GETDATE()),
('KH000005', N'Hoang Van E', 'e@gmail.com', '0123456793', N'HN', GETDATE());

-- SANPHAM
INSERT INTO SANPHAM VALUES
('SP000001', N'iPhone', 'DM001', 20000000, 10, N'Dang ban'),
('SP000002', N'Samsung', 'DM001', 15000000, 5, N'Dang ban'),
('SP000003', N'Macbook', 'DM002', 30000000, 3, N'Dang ban'),
('SP000004', N'Dell', 'DM002', 20000000, 2, N'Ngung ban'),
('SP000005', N'Chuot', 'DM003', 500000, 20, N'Dang ban'),
('SP000006', N'Ban phim', 'DM003', 700000, 15, N'Ngung ban'),
('SP000007', N'Tai nghe', 'DM003', 1000000, 0, N'Het hang'),
('SP000008', N'Asus', 'DM002', 18000000, 4, N'Dang ban');

-- DONHANG
INSERT INTO DONHANG VALUES
('DH00000001','KH000001','SP000001',1,20000000,GETDATE(),N'Da giao'),
('DH00000002','KH000002','SP000002',2,15000000,GETDATE(),N'Da huy'),
('DH00000003','KH000003','SP000003',1,30000000,GETDATE(),N'Da giao'),
('DH00000004','KH000004','SP000004',1,20000000,GETDATE(),N'Dang giao'),
('DH00000005','KH000005','SP000005',3,500000,GETDATE(),N'Da giao'),
('DH00000006','KH000001','SP000006',2,700000,GETDATE(),N'Da huy'),
('DH00000007','KH000002','SP000007',1,1000000,GETDATE(),N'Da giao'),
('DH00000008','KH000003','SP000008',1,18000000,GETDATE(),N'Dang giao'),
('DH00000009','KH000004','SP000001',1,20000000,GETDATE(),N'Da giao'),
('DH00000010','KH000005','SP000002',1,15000000,GETDATE(),N'Da giao');
--

select * from KHACHHANG
select * from DANHMUC
select * from SANPHAM
select * from DONHANG
/*1.Cập nhật giá bán của tất cả sản phẩm thuộc danh mục 'Laptop' tăng thêm 5%.*/
--0.5 điểm

update sanpham
set giaban = giaban * 1.05
where madm in (select madm from danhmuc where tendm = 'Laptop')

/*2.Hiển thị MaSP, TenSP, GiaBan, TonKho của tất cả sản phẩm có TrangThai là 'Dang ban'. 
Sắp xếp kết quả theo GiaBan giảm dần.*/
--0.5 điểm

select MaSP, TenSP, GiaBan, TonKho from SANPHAM
where TrangThai = 'Dang ban'

/*3.Hiển thị HoTen và Email của những khách hàng có địa chỉ email kết thúc bằng '@gmail.com'. 
Sắp xếp theo HoTen tăng dần.*/
--0.5 điểm

SELECT HoTen, Email
from khachhang
where email like '%@gmail.com'
order by hoten asc


/*4.Đếm số lượng sản phẩm theo từng trạng thái (TrangThai). 
Hiển thị TrangThai và số lượng tương ứng. Sắp xếp theo số lượng giảm dần.*/
--0.5 điểm

select trangthai, count(masp) as [sl]
from sanpham
group by trangthai
order by count(masp) desc



/*5.Hiển thị TenSP, GiaBan của các sản phẩm có giá từ 5.000.000 đến 20.000.000 VNĐ. 
Chỉ hiển thị sản phẩm đang có hàng trong kho (TonKho > 0). Sắp xếp theo GiaBan tăng dần.*/
--0.5 điểm

select tensp, giaban
from sanpham
where giaban between 5000000 and 20000000 and tonkho > 0
order by giaban asc;


/*6.Hiển thị MaDH, HoTen (khách hàng), TenSP, SoLuong, DonGia, NgayDat 
của tất cả đơn hàng. Sắp xếp theo NgayDat mới nhất trước.*/
--0.5 điểm


select d.MaDH, k.HoTen, s.TenSP, d.SoLuong, d.DonGia, d.NgayDat
from donhang d
join khachhang k on d.makh = k.makh
join sanpham s on d.masp = s.masp
order by d.ngaydat desc



/*7.Hiển thị MaDH, HoTen (khách hàng), TenSP, thành tiền (SoLuong * DonGia) 
của tất cả đơn hàng có trạng thái 'Da giao'. Sắp xếp theo thành tiền giảm dần.*/
--0.5 điểm


select k.hoten, sum(d.soluong * d.dongia) as [thành tiền]
from khachhang k
join donhang d on k.makh = d.makh
where d.trangthai = 'da giao'
group by k.hoten
order by sum(d.soluong * d.dongia) desc


/*8.Hiển thị TenSP, GiaBan, TonKho, TenDM của tất cả sản phẩm. 
Kết quả phải bao gồm cả sản phẩm chưa có danh mục (nếu có). 
Sắp xếp theo TenDM, sau đó theo TenSP.*/
--0.5 điểm

select s.tensp, s.giaban, s.tonkho, dm.tendm
from sanpham s
left join danhmuc dm on s.madm = dm.madm
order by dm.tendm asc, s.tensp asc;


/*9.Liệt kê những khách hàng (HoTen, Email) chưa có bất kỳ đơn hàng nào trong hệ thống. 
Sử dụng LEFT JOIN.*/
--0.5 điểm

select k.hoten, k.email
from khachhang k
left join donhang d on k.makh = d.makh
where d.madh is null



/*10.Tính tổng doanh thu (SUM của SoLuong * DonGia) của từng khách hàng từ các đơn hàng 
có trạng thái 'Da giao'. Hiển thị HoTen và tổng doanh thu. 
Chỉ hiển thị những khách hàng có tổng doanh thu từ 5.000.000 VNĐ trở lên. 
Sắp xếp theo tổng doanh thu giảm dần.*/
--0.5 điểm


select k.hoten, sum(d.soluong * d.dongia) as [tổng doanh thu]
from khachhang k
join donhang d on k.makh = d.makh
where d.trangthai = 'Da giao'
group by k.hoten
having sum(d.soluong * d.dongia) >= 5000000
order by sum(d.soluong * d.dongia) desc


/*11.Với mỗi danh mục sản phẩm, hiển thị TenDM, tổng số đơn hàng, 
tổng số lượng sản phẩm đã bán và doanh thu trung bình mỗi đơn. 
Chỉ tính đơn hàng có trạng thái 'Da giao'.*/
--1.0 điểm


select * from DONHANG
select * from SANPHAM
select * from DANHMUC

SELECT dm.TenDM, COUNT(d.MaDH) AS [tổng số đơn hàng], SUM(d.SoLuong) AS [tổng số lượng], AVG(d.SoLuong * d.DonGia) AS [doanh thu trung bình]
FROM DANHMUC dm
JOIN SANPHAM s ON dm.MaDM = s.MaDM
JOIN DONHANG d ON s.MaSP = d.MaSP
WHERE d.TrangThai = 'Da giao'
GROUP BY dm.TenDM

/*12.Danh sách MaKH, HoTen của những khách hàng đã có đơn hàng trạng thái 'Da giao' 
nhưng KHÔNG có đơn hàng nào bị 'Da huy' (dùng EXCEPT).*/
--1.0 điểm


select makh, hoten
from khachhang
where makh in
(
    select makh from donhang where trangthai = 'Da giao'
    EXCEPT
    select makh from donhang where trangthai = 'Da huy'
)


/*13.Danh sách MaKH, HoTen của những khách hàng có đơn hàng 'Da giao' 
hoặc đơn hàng 'Dang giao' (dùng UNION, không trùng lặp).*/
--1.0 điểm


select makh, hoten
from khachhang
where makh in
(
    select makh from donhang where trangthai = 'Da giao'
    UNION
    select makh from donhang where trangthai = 'Dang giao'
)



/*14.Dùng phép toán INTERSECT để tìm MaSP, TenSP của những sản phẩm vừa 
có đơn hàng đã giao thành công ('Da giao') vừa có đơn hàng bị hủy ('Da huy').*/
--1.0 điểm


select masp, tensp
from sanpham
where masp in
(
    select masp from donhang where trangthai = 'Da giao'
    INTERSECT
    select masp from donhang where trangthai = 'Da huy'
)



/*15.Viết truy vấn tìm danh sách HoTen và tổng chi tiêu (tổng SoLuong * DonGia từ các đơn hàng 'Da giao') 
của những khách hàng có tổng chi tiêu CAO HƠN mức chi tiêu trung bình của tất cả khách hàng. 
Kết quả sắp xếp theo tổng chi tiêu giảm dần.*/
--1.0 điểm






