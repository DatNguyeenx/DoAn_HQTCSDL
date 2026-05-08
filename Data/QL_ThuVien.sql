CREATE DATABASE QuanLyThuVien;
GO
USE QuanLyThuVien;
GO

-- 1. Thể loại sách
CREATE TABLE LoaiSach (
    MaLoai CHAR(5), -- VD: 'LS001'
    TenLoai NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_LoaiSach PRIMARY KEY (MaLoai)
);

-- 2. Nhà xuất bản
CREATE TABLE NhaXuatBan (
    MaNXB CHAR(5), -- VD: 'NB001'
    TenNXB NVARCHAR(200) NOT NULL,
    DiaChi NVARCHAR(255),
    SoDT VARCHAR(15),
    CONSTRAINT PK_NhaXuatBan PRIMARY KEY (MaNXB)
);

-- 3. Tác giả
CREATE TABLE TacGia (
    MaTG CHAR(5), -- VD: 'TG001'
    TenTG NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_TacGia PRIMARY KEY (MaTG)
);

-- 4. Bảng Sách (Đầu sách - Thông tin chung)
CREATE TABLE Sach (
    MaSach CHAR(5), -- VD: 'SA001'
    TenSach NVARCHAR(200) NOT NULL,
    MaNXB CHAR(5),
    NamXB INT,
    CONSTRAINT PK_Sach PRIMARY KEY (MaSach),
    CONSTRAINT FK_Sach_NXB FOREIGN KEY (MaNXB) REFERENCES NhaXuatBan(MaNXB)
);

-- 5. Bảng Chi tiết tác giả
CREATE TABLE ChiTietTacGia (
    MaSach CHAR(5),
    MaTG CHAR(5),
    VaiTro NVARCHAR(50),
    CONSTRAINT PK_ChiTietTacGia PRIMARY KEY (MaSach, MaTG),
    CONSTRAINT FK_CTTG_Sach FOREIGN KEY (MaSach) REFERENCES Sach(MaSach),
    CONSTRAINT FK_CTTG_TacGia FOREIGN KEY (MaTG) REFERENCES TacGia(MaTG)
);

-- 6. Bản sao sách (Từng cuốn cụ thể)
CREATE TABLE BanSaoSach (
    MaBS CHAR(5), -- VD: 'BS001'
    MaSach CHAR(5),
    TinhTrang NVARCHAR(50) DEFAULT N'Sẵn sàng',
    CONSTRAINT PK_BanSaoSach PRIMARY KEY (MaBS),
    CONSTRAINT FK_BanSaoSach_Sach FOREIGN KEY (MaSach) REFERENCES Sach(MaSach)
);

-- 7. Nhân viên
CREATE TABLE NhanVien (
    MaNV CHAR(5), -- VD: 'NV001'
    HoTen NVARCHAR(100),
    TenDN VARCHAR(50) NOT NULL,
    MatKhau VARCHAR(50),
    ChucVu NVARCHAR(50),
    CONSTRAINT PK_NhanVien PRIMARY KEY (MaNV),
    CONSTRAINT UQ_TenDN UNIQUE (TenDN)
);

-- 8. Độc giả
CREATE TABLE DocGia (
    MaDG CHAR(5), -- VD: 'DG001'
    HoTen NVARCHAR(100) NOT NULL,
    NgaySinh DATE,
    DiaChi NVARCHAR(255),
    SoDT VARCHAR(15),
    NgayLapThe DATE DEFAULT GETDATE(),
    NgayHetHan DATE,
    TrangThaiThe NVARCHAR(50) DEFAULT N'Bình thường',
    CONSTRAINT PK_DocGia PRIMARY KEY (MaDG)
);

-- 9. Phiếu mượn
CREATE TABLE PhieuMuon (
    MaPhieu CHAR(5), -- VD: 'PM001'
    MaDG CHAR(5),
    MaNV CHAR(5),
    NgayMuon DATE DEFAULT GETDATE(),
    HanTra DATE,
    CONSTRAINT PK_PhieuMuon PRIMARY KEY (MaPhieu),
    CONSTRAINT FK_PhieuMuon_DocGia FOREIGN KEY (MaDG) REFERENCES DocGia(MaDG),
    CONSTRAINT FK_PhieuMuon_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);

-- 10. Chi tiết phiếu mượn
CREATE TABLE ChiTietPhieuMuon (
    MaPhieu CHAR(5),
    MaBS CHAR(5),
    NgayTraThucTe DATE NULL,
    TrangThaiTra NVARCHAR(50),
    CONSTRAINT PK_ChiTietPhieuMuon PRIMARY KEY (MaPhieu, MaBS),
    CONSTRAINT FK_CTPM_PhieuMuon FOREIGN KEY (MaPhieu) REFERENCES PhieuMuon(MaPhieu),
    CONSTRAINT FK_CTPM_BanSao FOREIGN KEY (MaBS) REFERENCES BanSaoSach(MaBS)
);

-- 11. Phiếu phạt
CREATE TABLE PhieuPhat (
    MaPhieuPhat INT IDENTITY(1,1), 
    MaPhieu CHAR(5),
    MaBS CHAR(5),
    SoTienPhat DECIMAL(18,2),
    LyDo NVARCHAR(255),
    CONSTRAINT PK_PhieuPhat PRIMARY KEY (MaPhieuPhat),
    CONSTRAINT FK_PhieuPhat_CTPM FOREIGN KEY (MaPhieu, MaBS) REFERENCES ChiTietPhieuMuon(MaPhieu, MaBS)
);
GO

-- 12. Phiếu nhập
CREATE TABLE PhieuNhap (
    MaPhieuNhap CHAR(5), -- VD: 'PN001'
    MaNV CHAR(5),
    NgayNhap DATETIME DEFAULT GETDATE(),
    TongTien DECIMAL(18,2),
    CONSTRAINT PK_PhieuNhap PRIMARY KEY (MaPhieuNhap),
    CONSTRAINT FK_PhieuNhap_NhanVien FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);

-- 13. Chi tiết phiếu nhập
CREATE TABLE ChiTietPhieuNhap (
    MaPhieuNhap CHAR(5),
    MaSach CHAR(5),
    SoLuong INT,
    DonGia DECIMAL(18,2),
    CONSTRAINT PK_ChiTietPhieuNhap PRIMARY KEY (MaPhieuNhap, MaSach),
    CONSTRAINT FK_CTPN_PhieuNhap FOREIGN KEY (MaPhieuNhap) REFERENCES PhieuNhap(MaPhieuNhap),
    CONSTRAINT FK_CTPN_Sach FOREIGN KEY (MaSach) REFERENCES Sach(MaSach)
);

-- 14. Chi tiết thể loại
CREATE TABLE ChiTietTheLoai (
    MaSach CHAR(5),
    MaLoai CHAR(5),
    CONSTRAINT PK_ChiTietTheLoai PRIMARY KEY (MaSach, MaLoai),
    CONSTRAINT FK_CTTL_Sach FOREIGN KEY (MaSach) REFERENCES Sach(MaSach),
    CONSTRAINT FK_CTTL_Loai FOREIGN KEY (MaLoai) REFERENCES LoaiSach(MaLoai)
);

-- ==========================================
-- 1. NHÓM DANH MỤC (THỂ LOẠI, NXB, TÁC GIẢ)
-- ==========================================
-- 10 Thể loại sách
INSERT INTO LoaiSach (MaLoai, TenLoai) VALUES 
('LS001', N'Văn học'), ('LS002', N'Khoa học'), ('LS003', N'Kinh tế'), ('LS004', N'Kỹ năng'), ('LS005', N'Lịch sử'),
('LS006', N'Công nghệ'), ('LS007', N'Ngoại ngữ'), ('LS008', N'Tâm lý'), ('LS009', N'Thiếu nhi'), ('LS010', N'Trinh thám');

-- 20 Nhà xuất bản
INSERT INTO NhaXuatBan (MaNXB, TenNXB, DiaChi, SoDT) VALUES 
('NB001', N'NXB Trẻ', N'TP.HCM', '02839316289'), ('NB002', N'NXB Kim Đồng', N'Hà Nội', '02439434730'),
('NB003', N'NXB Giáo Dục', N'Hà Nội', '02438220801'), ('NB004', N'NXB Tổng hợp', N'TP.HCM', '02838225340'),
('NB005', N'NXB Chính trị', N'Hà Nội', '02437430200'), ('NB006', N'NXB Lao Động', N'Hà Nội', '02438515380'),
('NB007', N'NXB Phụ Nữ', N'Hà Nội', '02439420748'), ('NB008', N'NXB Hội Nhà Văn', N'Hà Nội', '02438222135'),
('NB009', N'NXB Thế Giới', N'Hà Nội', '02438253843'), ('NB010', N'NXB Khoa Học', N'Hà Nội', '02438254773'),
('NB011', N'Alpha Books', N'Hà Nội', '02437226234'), ('NB012', N'Nhã Nam', N'Hà Nội', '02435141086'),
('NB013', N'First News', N'TP.HCM', '02838224539'), ('NB014', N'NXB Thanh Niên', N'Hà Nội', '02439424009'),
('NB015', N'NXB Thông Tin', N'Hà Nội', '02435772139'), ('NB016', N'NXB Y Học', N'Hà Nội', '02438523234'),
('NB017', N'NXB Giao Thông', N'Hà Nội', '02439422431'), ('NB018', N'NXB Mỹ Thuật', N'Hà Nội', '02439424138'),
('NB019', N'NXB Xây Dựng', N'Hà Nội', '02438531435'), ('NB020', N'NXB ĐHQG', N'Hà Nội', '02437547736');

-- 50 Tác giả
INSERT INTO TacGia (MaTG, TenTG) VALUES 
('TG001', N'Nguyễn Nhật Ánh'), ('TG002', N'Tô Hoài'), ('TG003', N'Nam Cao'), ('TG004', N'Vũ Trọng Phụng'), ('TG005', N'Ngô Tất Tố'),
('TG006', N'Xuân Quỳnh'), ('TG007', N'Huy Cận'), ('TG008', N'Chế Lan Viên'), ('TG009', N'Hàn Mặc Tử'), ('TG010', N'Nguyễn Du'),
('TG011', N'Dale Carnegie'), ('TG012', N'Paulo Coelho'), ('TG013', N'Haruki Murakami'), ('TG014', N'Napoleon Hill'), ('TG015', N'Stephen King'),
('TG016', N'J.K. Rowling'), ('TG017', N'Ernest Hemingway'), ('TG018', N'Victor Hugo'), ('TG019', N'Lev Tolstoy'), ('TG020', N'Mark Twain'),
('TG021', N'Nguyễn Hiến Lê'), ('TG022', N'Phan Bội Châu'), ('TG023', N'Phan Chu Trinh'), ('TG024', N'Trần Trọng Kim'), ('TG025', N'Đặng Thai Mai'),
('TG026', N'Hoài Thanh'), ('TG027', N'Lưu Quang Vũ'), ('TG028', N'Nguyễn Huy Thiệp'), ('TG029', N'Bảo Ninh'), ('TG030', N'Nguyễn Khải'),
('TG031', N'Ma Văn Kháng'), ('TG032', N'Chu Văn'), ('TG033', N'Nguyễn Minh Châu'), ('TG034', N'Anh Đức'), ('TG035', N'Phan Tứ'),
('TG036', N'Hữu Thỉnh'), ('TG037', N'Thanh Thảo'), ('TG038', N'Nguyễn Khoa Điềm'), ('TG039', N'Phạm Tiến Duật'), ('TG040', N'Lê Anh Xuân'),
('TG041', N'Giang Nam'), ('TG042', N'Tố Hữu'), ('TG043', N'Quang Dũng'), ('TG044', N'Đoàn Giỏi'), ('TG045', N'Nguyễn Quang Sáng'),
('TG046', N'Nguyễn Thành Long'), ('TG047', N'Kim Lân'), ('TG048', N'Nguyên Ngọc'), ('TG049', N'Nguyễn Trung Thành'), ('TG050', N'Hồ Phương');

-- ==========================================
-- 2. NHÓM SÁCH (ĐẦU SÁCH, TÁC GIẢ SÁCH, BẢN SAO)
-- ==========================================
-- 100 Sách (Map lại khóa ngoại từ INT sang CHAR(5))
INSERT INTO Sach (MaSach, TenSach, MaNXB, NamXB) VALUES
('SA001', N'Cho tôi xin một vé đi tuổi thơ', 'NB001', 2020),
('SA002', N'Dế Mèn Phiêu Lưu Ký', 'NB002', 2018),
('SA003', N'Lão Hạc', 'NB001', 1943),
('SA004', N'Số Đỏ', 'NB008', 2015),
('SA005', N'Tắt Đèn', 'NB005', 2012),
('SA006', N'Đắc Nhân Tâm', 'NB013', 2021),
('SA007', N'Nhà Giả Kim', 'NB012', 2020),
('SA008', N'Rừng Na Uy', 'NB012', 2019),
('SA009', N'Nghĩ Giàu Làm Giàu', 'NB011', 2022),
('SA010', N'IT - Gã Hề Ma Quái', 'NB014', 2017),
('SA011', N'Harry Potter và Hòn Đá Phù Thủy', 'NB001', 2000),
('SA012', N'Ông Già Và Biển Cả', 'NB008', 2010),
('SA013', N'Những Người Khốn Khổ', 'NB009', 2014),
('SA014', N'Chiến Tranh Và Hòa Bình', 'NB010', 2011),
('SA015', N'Tom Sawyer', 'NB002', 2016),
('SA016', N'Quẳng Gánh Lo Đi', 'NB013', 2020),
('SA017', N'Súng, Vi trùng và Thép', 'NB020', 2021),
('SA018', N'Lược Sử Thời Gian', 'NB012', 2015),
('SA019', N'Cha Giàu Cha Nghèo', 'NB011', 2022),
('SA020', N'Thế Giới Phẳng', 'NB013', 2018),
('SA021', N'Bắt Trẻ Đồng Xanh', 'NB012', 2017),
('SA022', N'Tiếng Chim Hót Trong Bụi Mận Gai', 'NB014', 2015),
('SA023', N'Trăm Năm Cô Đơn', 'NB009', 2019),
('SA024', N'Kiêu Hãnh Và Định Kiến', 'NB012', 2020),
('SA025', N'Đồi Gió Hú', 'NB008', 2014),
('SA026', N'Sách Lập Trình C#', 'NB015', 2023),
('SA027', N'Java Core', 'NB015', 2022),
('SA028', N'Python cơ bản', 'NB015', 2023),
('SA029', N'Cấu trúc dữ liệu và giải thuật', 'NB020', 2021),
('SA030', N'Nhập môn AI', 'NB020', 2023),
('SA031', N'Lịch sử văn minh thế giới', 'NB010', 2018),
('SA032', N'Đại Việt Sử Ký Toàn Thư', 'NB005', 2015),
('SA033', N'Việt Nam Sử Lược', 'NB005', 2012),
('SA034', N'Tâm lý học đám đông', 'NB020', 2020),
('SA035', N'Sức mạnh của thói quen', 'NB011', 2021),
('SA036', N'Tư duy nhanh và chậm', 'NB011', 2022),
('SA037', N'Bàn về tự do', 'NB009', 2019),
('SA038', N'Cộng hòa - Plato', 'NB020', 2021),
('SA039', N'Thế giới như tôi thấy', 'NB012', 2020),
('SA040', N'Dạy con làm giàu - Tập 2', 'NB011', 2021),
('SA041', N'Tôi tự học', 'NB001', 2015),
('SA042', N'Kim Các Tự', 'NB012', 2018),
('SA043', N'Mắt Biếc', 'NB001', 2019),
('SA044', N'Tôi thấy hoa vàng trên cỏ xanh', 'NB001', 2015),
('SA045', N'Đất rừng phương Nam', 'NB002', 2014),
('SA046', N'Chiếc lược ngà', 'NB002', 2012),
('SA047', N'Bến quê', 'NB008', 2010),
('SA048', N'Mùa lá rụng trong vườn', 'NB008', 2011),
('SA049', N'Nỗi buồn chiến tranh', 'NB008', 2015),
('SA050', N'Mảnh trăng cuối rừng', 'NB008', 2013),
('SA051', N'Hoàng tử bé', 'NB001', 2018),
('SA052', N'Hai vạn dặm dưới đáy biển', 'NB002', 2017),
('SA053', N'Sherlock Holmes', 'NB003', 2019),
('SA054', N'Mật mã Da Vinci', 'NB004', 2020),
('SA055', N'Suối Nguồn', 'NB005', 2015),
('SA056', N'Cuốn theo chiều gió', 'NB006', 2014),
('SA057', N'Gatsby vĩ đại', 'NB007', 2018),
('SA058', N'Tội ác và hình phạt', 'NB008', 2016),
('SA059', N'Anh em nhà Karamazov', 'NB009', 2017),
('SA060', N'Ruồi trâu', 'NB010', 2010),
('SA061', N'Không gia đình', 'NB011', 2015),
('SA062', N'Những cuộc phiêu lưu của Huckleberry Finn', 'NB012', 2016),
('SA063', N'Cuộc đời của Pi', 'NB013', 2018),
('SA064', N'Người trộm bóng', 'NB014', 2019),
('SA065', N'Kẻ trộm sách', 'NB015', 2017),
('SA066', N'Sapiens - Lược sử loài người', 'NB016', 2021),
('SA067', N'Homo Deus - Lược sử tương lai', 'NB017', 2022),
('SA068', N'21 bài học cho thế kỷ 21', 'NB018', 2022),
('SA069', N'Nguồn cội', 'NB019', 2019),
('SA070', N'Thiên thần và Ác quỷ', 'NB020', 2018),
('SA071', N'Sự im lặng của bầy cừu', 'NB001', 2017),
('SA072', N'Cô gái có hình xăm rồng', 'NB002', 2015),
('SA073', N'Lời thú tội', 'NB003', 2019),
('SA074', N'Bạch dạ hành', 'NB004', 2020),
('SA075', N'Phía sau nghi can X', 'NB005', 2018),
('SA076', N'Kỳ án ánh trăng', 'NB006', 2016),
('SA077', N'Mười người da đen nhỏ', 'NB007', 2015),
('SA078', N'Án mạng trên chuyến tàu tốc hành phương Đông', 'NB008', 2017),
('SA079', N'Khởi nghiệp tinh gọn', 'NB009', 2021),
('SA080', N'Từ tốt đến vĩ đại', 'NB010', 2020),
('SA081', N'Những kẻ xuất chúng', 'NB011', 2019),
('SA082', N'Điểm bùng phát', 'NB012', 2018),
('SA083', N'Đọc vị bất kỳ ai', 'NB013', 2020),
('SA084', N'Ngôn ngữ cơ thể', 'NB014', 2019),
('SA085', N'Đàn ông sao Hỏa đàn bà sao Kim', 'NB015', 2015),
('SA086', N'Thay đổi câu hỏi thay đổi cuộc đời', 'NB016', 2021),
('SA087', N'Dám bị ghét', 'NB017', 2020),
('SA088', N'Tuổi trẻ đáng giá bao nhiêu', 'NB018', 2019),
('SA089', N'Bạn đắt giá bao nhiêu', 'NB019', 2020),
('SA090', N'Lối sống tối giản của người Nhật', 'NB020', 2018),
('SA091', N'Khí chất bao nhiêu hạnh phúc bấy nhiêu', 'NB001', 2021),
('SA092', N'Tôi tài giỏi bạn cũng thế', 'NB002', 2015),
('SA093', N'Người bán hàng vĩ đại nhất thế giới', 'NB003', 2017),
('SA094', N'Chiến tranh tiền tệ', 'NB004', 2016),
('SA095', N'Lãnh đạo không chức danh', 'NB005', 2019),
('SA096', N'Phi lý trí', 'NB006', 2020),
('SA097', N'Hiệu ứng cánh bướm', 'NB007', 2018),
('SA098', N'Mật mã', 'NB008', 2019),
('SA099', N'Lược sử internet', 'NB009', 2020),
('SA100', N'Hackers và Painters', 'NB010', 2018);

INSERT INTO ChiTietTheLoai (MaSach, MaLoai) VALUES
('SA001', 'LS001'), ('SA001', 'LS009'),
('SA002', 'LS001'), ('SA002', 'LS009'),
('SA003', 'LS001'),
('SA004', 'LS001'), ('SA004', 'LS008'),
('SA005', 'LS001'),
('SA006', 'LS004'), ('SA006', 'LS008'),
('SA007', 'LS001'), ('SA007', 'LS008'),
('SA008', 'LS001'), ('SA008', 'LS008'),
('SA009', 'LS003'), ('SA009', 'LS004'),
('SA010', 'LS001'), ('SA010', 'LS010'),
('SA011', 'LS001'), ('SA011', 'LS009'),
('SA012', 'LS001'),
('SA013', 'LS001'), ('SA013', 'LS005'),
('SA014', 'LS001'), ('SA014', 'LS005'),
('SA015', 'LS001'), ('SA015', 'LS009'),
('SA016', 'LS004'), ('SA016', 'LS008'),
('SA017', 'LS002'), ('SA017', 'LS005'),
('SA018', 'LS002'),
('SA019', 'LS003'), ('SA019', 'LS004'),
('SA020', 'LS003'), ('SA020', 'LS006'),
('SA021', 'LS001'), ('SA021', 'LS008'),
('SA022', 'LS001'),
('SA023', 'LS001'),
('SA024', 'LS001'),
('SA025', 'LS001'), ('SA025', 'LS008'),
('SA026', 'LS006'),
('SA027', 'LS006'),
('SA028', 'LS006'),
('SA029', 'LS006'), ('SA029', 'LS002'),
('SA030', 'LS006'), ('SA030', 'LS002'),
('SA031', 'LS005'), ('SA031', 'LS002'),
('SA032', 'LS005'),
('SA033', 'LS005'),
('SA034', 'LS008'), ('SA034', 'LS003'),
('SA035', 'LS004'), ('SA035', 'LS008'),
('SA036', 'LS008'), ('SA036', 'LS002'),
('SA037', 'LS005'), ('SA037', 'LS008'),
('SA038', 'LS005'),
('SA039', 'LS002'), ('SA039', 'LS008'),
('SA040', 'LS003'), ('SA040', 'LS004'),
('SA041', 'LS004'),
('SA042', 'LS001'),
('SA043', 'LS001'), ('SA043', 'LS008'),
('SA044', 'LS001'), ('SA044', 'LS009'),
('SA045', 'LS001'), ('SA045', 'LS009'),
('SA046', 'LS001'), ('SA046', 'LS005'),
('SA047', 'LS001'),
('SA048', 'LS001'),
('SA049', 'LS001'), ('SA049', 'LS005'),
('SA050', 'LS001'),
('SA051', 'LS001'), ('SA051', 'LS009'),
('SA052', 'LS001'), ('SA052', 'LS002'),
('SA053', 'LS001'), ('SA053', 'LS010'),
('SA054', 'LS001'), ('SA054', 'LS010'),
('SA055', 'LS001'),
('SA056', 'LS001'), ('SA056', 'LS005'),
('SA057', 'LS001'),
('SA058', 'LS001'), ('SA058', 'LS008'),
('SA059', 'LS001'),
('SA060', 'LS001'),
('SA061', 'LS001'), ('SA061', 'LS009'),
('SA062', 'LS001'), ('SA062', 'LS009'),
('SA063', 'LS001'),
('SA064', 'LS001'),
('SA065', 'LS001'), ('SA065', 'LS005'),
('SA066', 'LS002'), ('SA066', 'LS005'),
('SA067', 'LS002'), ('SA067', 'LS006'),
('SA068', 'LS004'), ('SA068', 'LS005'),
('SA069', 'LS001'), ('SA069', 'LS010'),
('SA070', 'LS001'), ('SA070', 'LS010'),
('SA071', 'LS008'), ('SA071', 'LS010'),
('SA072', 'LS010'),
('SA073', 'LS008'), ('SA073', 'LS010'),
('SA074', 'LS001'), ('SA074', 'LS010'),
('SA075', 'LS008'), ('SA075', 'LS010'),
('SA076', 'LS010'),
('SA077', 'LS010'),
('SA078', 'LS010'),
('SA079', 'LS003'), ('SA079', 'LS004'),
('SA080', 'LS003'), ('SA080', 'LS004'),
('SA081', 'LS004'), ('SA081', 'LS008'),
('SA082', 'LS003'), ('SA082', 'LS008'),
('SA083', 'LS004'), ('SA083', 'LS008'),
('SA084', 'LS004'), ('SA084', 'LS008'),
('SA085', 'LS008'),
('SA086', 'LS004'), ('SA086', 'LS008'),
('SA087', 'LS004'), ('SA087', 'LS008'),
('SA088', 'LS004'),
('SA089', 'LS004'),
('SA090', 'LS004'),
('SA091', 'LS004'),
('SA092', 'LS004'),
('SA093', 'LS003'), ('SA093', 'LS004'),
('SA094', 'LS003'), ('SA094', 'LS005'),
('SA095', 'LS003'), ('SA095', 'LS004'),
('SA096', 'LS003'), ('SA096', 'LS008'),
('SA097', 'LS002'), ('SA097', 'LS008'),
('SA098', 'LS006'),
('SA099', 'LS005'), ('SA099', 'LS006'),
('SA100', 'LS006');

-- BỔ SUNG: Chi Tiết Tác Giả (Liên kết sách và tác giả)
-- Ánh xạ đại diện cho 50 cuốn đầu tiên
INSERT INTO ChiTietTacGia (MaSach, MaTG, VaiTro) VALUES 
('SA001', 'TG001', N'Tác giả'), ('SA002', 'TG002', N'Tác giả'), ('SA003', 'TG003', N'Tác giả'), ('SA004', 'TG004', N'Tác giả'), ('SA005', 'TG005', N'Tác giả'),
('SA006', 'TG011', N'Tác giả'), ('SA007', 'TG012', N'Tác giả'), ('SA008', 'TG013', N'Tác giả'), ('SA009', 'TG014', N'Tác giả'), ('SA010', 'TG015', N'Tác giả'),
('SA011', 'TG016', N'Tác giả'), ('SA012', 'TG017', N'Tác giả'), ('SA013', 'TG018', N'Tác giả'), ('SA014', 'TG019', N'Tác giả'), ('SA015', 'TG020', N'Tác giả');

-- 100 Bản sao sách (Mỗi đầu sách có ít nhất 1 bản sao)
INSERT INTO BanSaoSach (MaBS, MaSach, TinhTrang) VALUES 
('BS001', 'SA001', N'Sẵn sàng'), ('BS002', 'SA002', N'Sẵn sàng'), ('BS003', 'SA003', N'Sẵn sàng'), ('BS004', 'SA004', N'Sẵn sàng'), ('BS005', 'SA005', N'Sẵn sàng'),
('BS006', 'SA006', N'Đang mượn'), ('BS007', 'SA007', N'Sẵn sàng'), ('BS008', 'SA008', N'Đang mượn'), ('BS009', 'SA009', N'Sẵn sàng'), ('BS010', 'SA010', N'Sẵn sàng'),
('BS011', 'SA011', N'Đang mượn'), ('BS012', 'SA012', N'Sẵn sàng'), ('BS013', 'SA013', N'Sẵn sàng'), ('BS014', 'SA014', N'Sẵn sàng'), ('BS015', 'SA015', N'Sẵn sàng'),
('BS016', 'SA016', N'Đang mượn'), ('BS017', 'SA017', N'Sẵn sàng'), ('BS018', 'SA018', N'Đang mượn'), ('BS019', 'SA019', N'Sẵn sàng'), ('BS020', 'SA020', N'Sẵn sàng'),
('BS021', 'SA021', N'Đang mượn'), ('BS022', 'SA022', N'Sẵn sàng'), ('BS023', 'SA023', N'Sẵn sàng'), ('BS024', 'SA024', N'Sẵn sàng'), ('BS025', 'SA025', N'Sẵn sàng'),
('BS026', 'SA026', N'Sẵn sàng'), ('BS027', 'SA027', N'Sẵn sàng'), ('BS028', 'SA028', N'Sẵn sàng'), ('BS029', 'SA029', N'Sẵn sàng'), ('BS030', 'SA030', N'Sẵn sàng'),
('BS031', 'SA031', N'Sẵn sàng'), ('BS032', 'SA032', N'Sẵn sàng'), ('BS033', 'SA033', N'Sẵn sàng'), ('BS034', 'SA034', N'Sẵn sàng'), ('BS035', 'SA035', N'Sẵn sàng'),
('BS036', 'SA036', N'Sẵn sàng'), ('BS037', 'SA037', N'Sẵn sàng'), ('BS038', 'SA038', N'Sẵn sàng'), ('BS039', 'SA039', N'Sẵn sàng'), ('BS040', 'SA040', N'Sẵn sàng'),
('BS041', 'SA041', N'Sẵn sàng'), ('BS042', 'SA042', N'Sẵn sàng'), ('BS043', 'SA043', N'Sẵn sàng'), ('BS044', 'SA044', N'Sẵn sàng'), ('BS045', 'SA045', N'Sẵn sàng'),
('BS046', 'SA046', N'Sẵn sàng'), ('BS047', 'SA047', N'Sẵn sàng'), ('BS048', 'SA048', N'Sẵn sàng'), ('BS049', 'SA049', N'Sẵn sàng'), ('BS050', 'SA050', N'Sẵn sàng'),
('BS051', 'SA051', N'Sẵn sàng'), ('BS052', 'SA052', N'Sẵn sàng'), ('BS053', 'SA053', N'Sẵn sàng'), ('BS054', 'SA054', N'Sẵn sàng'), ('BS055', 'SA055', N'Sẵn sàng'),
('BS056', 'SA056', N'Sẵn sàng'), ('BS057', 'SA057', N'Sẵn sàng'), ('BS058', 'SA058', N'Sẵn sàng'), ('BS059', 'SA059', N'Sẵn sàng'), ('BS060', 'SA060', N'Sẵn sàng'),
('BS061', 'SA061', N'Sẵn sàng'), ('BS062', 'SA062', N'Sẵn sàng'), ('BS063', 'SA063', N'Sẵn sàng'), ('BS064', 'SA064', N'Sẵn sàng'), ('BS065', 'SA065', N'Sẵn sàng'),
('BS066', 'SA066', N'Sẵn sàng'), ('BS067', 'SA067', N'Sẵn sàng'), ('BS068', 'SA068', N'Sẵn sàng'), ('BS069', 'SA069', N'Sẵn sàng'), ('BS070', 'SA070', N'Sẵn sàng'),
('BS071', 'SA071', N'Sẵn sàng'), ('BS072', 'SA072', N'Sẵn sàng'), ('BS073', 'SA073', N'Sẵn sàng'), ('BS074', 'SA074', N'Sẵn sàng'), ('BS075', 'SA075', N'Sẵn sàng'),
('BS076', 'SA076', N'Sẵn sàng'), ('BS077', 'SA077', N'Sẵn sàng'), ('BS078', 'SA078', N'Sẵn sàng'), ('BS079', 'SA079', N'Sẵn sàng'), ('BS080', 'SA080', N'Sẵn sàng'),
('BS081', 'SA081', N'Sẵn sàng'), ('BS082', 'SA082', N'Sẵn sàng'), ('BS083', 'SA083', N'Sẵn sàng'), ('BS084', 'SA084', N'Sẵn sàng'), ('BS085', 'SA085', N'Sẵn sàng'),
('BS086', 'SA086', N'Sẵn sàng'), ('BS087', 'SA087', N'Sẵn sàng'), ('BS088', 'SA088', N'Sẵn sàng'), ('BS089', 'SA089', N'Sẵn sàng'), ('BS090', 'SA090', N'Sẵn sàng'),
('BS091', 'SA091', N'Sẵn sàng'), ('BS092', 'SA092', N'Sẵn sàng'), ('BS093', 'SA093', N'Sẵn sàng'), ('BS094', 'SA094', N'Sẵn sàng'), ('BS095', 'SA095', N'Sẵn sàng'),
('BS096', 'SA096', N'Sẵn sàng'), ('BS097', 'SA097', N'Sẵn sàng'), ('BS098', 'SA098', N'Sẵn sàng'), ('BS099', 'SA099', N'Sẵn sàng'), ('BS100', 'SA100', N'Sẵn sàng'),
('BS101', 'SA001', N'Đang mượn'), ('BS102', 'SA002', N'Sẵn sàng'), ('BS103', 'SA003', N'Sẵn sàng'), ('BS104', 'SA004', N'Sẵn sàng'), ('BS105', 'SA005', N'Sẵn sàng'),
('BS106', 'SA006', N'Sẵn sàng'), ('BS107', 'SA007', N'Sẵn sàng'), ('BS108', 'SA008', N'Sẵn sàng'), ('BS109', 'SA009', N'Sẵn sàng'), ('BS110', 'SA010', N'Sẵn sàng'),
('BS111', 'SA011', N'Sẵn sàng'), ('BS112', 'SA012', N'Sẵn sàng'), ('BS113', 'SA013', N'Sẵn sàng'), ('BS114', 'SA014', N'Sẵn sàng'), ('BS115', 'SA015', N'Sẵn sàng'),
('BS116', 'SA016', N'Sẵn sàng'), ('BS117', 'SA017', N'Sẵn sàng'), ('BS118', 'SA018', N'Sẵn sàng'), ('BS119', 'SA019', N'Sẵn sàng'), ('BS120', 'SA020', N'Sẵn sàng'),
('BS121', 'SA021', N'Sẵn sàng'), ('BS122', 'SA022', N'Sẵn sàng'), ('BS123', 'SA023', N'Sẵn sàng'), ('BS124', 'SA024', N'Sẵn sàng'), ('BS125', 'SA025', N'Sẵn sàng'),
('BS126', 'SA026', N'Sẵn sàng'), ('BS127', 'SA027', N'Sẵn sàng'), ('BS128', 'SA028', N'Sẵn sàng'), ('BS129', 'SA029', N'Sẵn sàng'), ('BS130', 'SA030', N'Sẵn sàng'),
('BS131', 'SA031', N'Sẵn sàng'), ('BS132', 'SA032', N'Sẵn sàng'), ('BS133', 'SA033', N'Sẵn sàng'), ('BS134', 'SA034', N'Sẵn sàng'), ('BS135', 'SA035', N'Sẵn sàng'),
('BS136', 'SA036', N'Sẵn sàng'), ('BS137', 'SA037', N'Sẵn sàng'), ('BS138', 'SA038', N'Sẵn sàng'), ('BS139', 'SA039', N'Sẵn sàng'), ('BS140', 'SA040', N'Sẵn sàng'),
('BS141', 'SA041', N'Sẵn sàng'), ('BS142', 'SA042', N'Sẵn sàng'), ('BS143', 'SA043', N'Sẵn sàng'), ('BS144', 'SA044', N'Sẵn sàng'), ('BS145', 'SA045', N'Sẵn sàng'),
('BS146', 'SA046', N'Sẵn sàng'), ('BS147', 'SA047', N'Sẵn sàng'), ('BS148', 'SA048', N'Sẵn sàng'), ('BS149', 'SA049', N'Sẵn sàng'), ('BS150', 'SA050', N'Sẵn sàng'),
('BS151', 'SA051', N'Sẵn sàng'), ('BS152', 'SA052', N'Sẵn sàng'), ('BS153', 'SA053', N'Đang mượn'), ('BS154', 'SA054', N'Sẵn sàng'), ('BS155', 'SA055', N'Sẵn sàng'),
('BS156', 'SA056', N'Sẵn sàng'), ('BS157', 'SA057', N'Sẵn sàng'), ('BS158', 'SA058', N'Sẵn sàng'), ('BS159', 'SA059', N'Sẵn sàng'), ('BS160', 'SA060', N'Sẵn sàng'),
('BS161', 'SA061', N'Sẵn sàng'), ('BS162', 'SA062', N'Sẵn sàng'), ('BS163', 'SA063', N'Sẵn sàng'), ('BS164', 'SA064', N'Sẵn sàng'), ('BS165', 'SA065', N'Sẵn sàng'),
('BS166', 'SA066', N'Sẵn sàng'), ('BS167', 'SA067', N'Sẵn sàng'), ('BS168', 'SA068', N'Sẵn sàng'), ('BS169', 'SA069', N'Sẵn sàng'), ('BS170', 'SA070', N'Sẵn sàng'),
('BS171', 'SA071', N'Sẵn sàng'), ('BS172', 'SA072', N'Sẵn sàng'), ('BS173', 'SA073', N'Sẵn sàng'), ('BS174', 'SA074', N'Sẵn sàng'), ('BS175', 'SA075', N'Sẵn sàng'),
('BS176', 'SA076', N'Sẵn sàng'), ('BS177', 'SA077', N'Sẵn sàng'), ('BS178', 'SA078', N'Sẵn sàng'), ('BS179', 'SA079', N'Sẵn sàng'), ('BS180', 'SA080', N'Sẵn sàng'),
('BS181', 'SA081', N'Sẵn sàng'), ('BS182', 'SA082', N'Sẵn sàng'), ('BS183', 'SA083', N'Sẵn sàng'), ('BS184', 'SA084', N'Sẵn sàng'), ('BS185', 'SA085', N'Sẵn sàng'),
('BS186', 'SA086', N'Sẵn sàng'), ('BS187', 'SA087', N'Sẵn sàng'), ('BS188', 'SA088', N'Sẵn sàng'), ('BS189', 'SA089', N'Sẵn sàng'), ('BS190', 'SA090', N'Sẵn sàng'),
('BS191', 'SA091', N'Sẵn sàng'), ('BS192', 'SA092', N'Sẵn sàng'), ('BS193', 'SA093', N'Sẵn sàng'), ('BS194', 'SA094', N'Sẵn sàng'), ('BS195', 'SA095', N'Sẵn sàng'),
('BS196', 'SA096', N'Sẵn sàng'), ('BS197', 'SA097', N'Sẵn sàng'), ('BS198', 'SA098', N'Sẵn sàng'), ('BS199', 'SA099', N'Sẵn sàng'), ('BS200', 'SA100', N'Sẵn sàng'),
('BS201', 'SA001', N'Sẵn sàng'), ('BS202', 'SA002', N'Sẵn sàng'), ('BS203', 'SA003', N'Sẵn sàng'), ('BS204', 'SA004', N'Sẵn sàng'), ('BS205', 'SA005', N'Sẵn sàng'),
('BS206', 'SA006', N'Sẵn sàng'), ('BS207', 'SA007', N'Sẵn sàng'), ('BS208', 'SA008', N'Sẵn sàng'), ('BS209', 'SA009', N'Sẵn sàng'), ('BS210', 'SA010', N'Sẵn sàng'),
('BS211', 'SA011', N'Sẵn sàng'), ('BS212', 'SA012', N'Sẵn sàng'), ('BS213', 'SA013', N'Sẵn sàng'), ('BS214', 'SA014', N'Sẵn sàng'), ('BS215', 'SA015', N'Sẵn sàng'),
('BS216', 'SA016', N'Sẵn sàng'), ('BS217', 'SA017', N'Sẵn sàng'), ('BS218', 'SA018', N'Sẵn sàng'), ('BS219', 'SA019', N'Sẵn sàng'), ('BS220', 'SA020', N'Sẵn sàng'),
('BS221', 'SA021', N'Sẵn sàng'), ('BS222', 'SA022', N'Sẵn sàng'), ('BS223', 'SA023', N'Sẵn sàng'), ('BS224', 'SA024', N'Sẵn sàng'), ('BS225', 'SA025', N'Sẵn sàng'),
('BS226', 'SA026', N'Sẵn sàng'), ('BS227', 'SA027', N'Sẵn sàng'), ('BS228', 'SA028', N'Sẵn sàng'), ('BS229', 'SA029', N'Sẵn sàng'), ('BS230', 'SA030', N'Sẵn sàng'),
('BS231', 'SA031', N'Sẵn sàng'), ('BS232', 'SA032', N'Sẵn sàng'), ('BS233', 'SA033', N'Sẵn sàng'), ('BS234', 'SA034', N'Sẵn sàng'), ('BS235', 'SA035', N'Sẵn sàng'),
('BS236', 'SA036', N'Sẵn sàng'), ('BS237', 'SA037', N'Sẵn sàng'), ('BS238', 'SA038', N'Sẵn sàng'), ('BS239', 'SA039', N'Sẵn sàng'), ('BS240', 'SA040', N'Sẵn sàng'),
('BS241', 'SA041', N'Sẵn sàng'), ('BS242', 'SA042', N'Sẵn sàng'), ('BS243', 'SA043', N'Sẵn sàng'), ('BS244', 'SA044', N'Sẵn sàng'), ('BS245', 'SA045', N'Sẵn sàng'),
('BS246', 'SA046', N'Sẵn sàng'), ('BS247', 'SA047', N'Sẵn sàng'), ('BS248', 'SA048', N'Sẵn sàng'), ('BS249', 'SA049', N'Sẵn sàng'), ('BS250', 'SA050', N'Sẵn sàng'),
('BS251', 'SA051', N'Sẵn sàng'), ('BS252', 'SA052', N'Sẵn sàng'), ('BS253', 'SA053', N'Sẵn sàng'), ('BS254', 'SA054', N'Sẵn sàng'), ('BS255', 'SA055', N'Sẵn sàng'),
('BS256', 'SA056', N'Sẵn sàng'), ('BS257', 'SA057', N'Sẵn sàng'), ('BS258', 'SA058', N'Sẵn sàng'), ('BS259', 'SA059', N'Sẵn sàng'), ('BS260', 'SA060', N'Sẵn sàng'),
('BS261', 'SA061', N'Sẵn sàng'), ('BS262', 'SA062', N'Sẵn sàng'), ('BS263', 'SA063', N'Sẵn sàng'), ('BS264', 'SA064', N'Sẵn sàng'), ('BS265', 'SA065', N'Sẵn sàng'),
('BS266', 'SA066', N'Sẵn sàng'), ('BS267', 'SA067', N'Sẵn sàng'), ('BS268', 'SA068', N'Sẵn sàng'), ('BS269', 'SA069', N'Sẵn sàng'), ('BS270', 'SA070', N'Sẵn sàng'),
('BS271', 'SA071', N'Sẵn sàng'), ('BS272', 'SA072', N'Sẵn sàng'), ('BS273', 'SA073', N'Sẵn sàng'), ('BS274', 'SA074', N'Sẵn sàng'), ('BS275', 'SA075', N'Sẵn sàng'),
('BS276', 'SA076', N'Sẵn sàng'), ('BS277', 'SA077', N'Sẵn sàng'), ('BS278', 'SA078', N'Sẵn sàng'), ('BS279', 'SA079', N'Sẵn sàng'), ('BS280', 'SA080', N'Sẵn sàng'),
('BS281', 'SA081', N'Sẵn sàng'), ('BS282', 'SA082', N'Sẵn sàng'), ('BS283', 'SA083', N'Sẵn sàng'), ('BS284', 'SA084', N'Sẵn sàng'), ('BS285', 'SA085', N'Sẵn sàng'),
('BS286', 'SA086', N'Sẵn sàng'), ('BS287', 'SA087', N'Sẵn sàng'), ('BS288', 'SA088', N'Sẵn sàng'), ('BS289', 'SA089', N'Sẵn sàng'), ('BS290', 'SA090', N'Sẵn sàng'),
('BS291', 'SA091', N'Sẵn sàng'), ('BS292', 'SA092', N'Sẵn sàng'), ('BS293', 'SA093', N'Sẵn sàng'), ('BS294', 'SA094', N'Sẵn sàng'), ('BS295', 'SA095', N'Sẵn sàng'),
('BS296', 'SA096', N'Sẵn sàng'), ('BS297', 'SA097', N'Sẵn sàng'), ('BS298', 'SA098', N'Sẵn sàng'), ('BS299', 'SA099', N'Sẵn sàng'), ('BS300', 'SA100', N'Sẵn sàng'),
('BS301', 'SA001', N'Sẵn sàng'), ('BS302', 'SA002', N'Sẵn sàng'), ('BS303', 'SA003', N'Sẵn sàng'), ('BS304', 'SA004', N'Sẵn sàng'), ('BS305', 'SA005', N'Sẵn sàng'),
('BS306', 'SA006', N'Sẵn sàng'), ('BS307', 'SA007', N'Sẵn sàng'), ('BS308', 'SA008', N'Sẵn sàng'), ('BS309', 'SA009', N'Sẵn sàng'), ('BS310', 'SA010', N'Sẵn sàng'),
('BS311', 'SA011', N'Sẵn sàng'), ('BS312', 'SA012', N'Sẵn sàng'), ('BS313', 'SA013', N'Sẵn sàng'), ('BS314', 'SA014', N'Sẵn sàng'), ('BS315', 'SA015', N'Sẵn sàng'),
('BS316', 'SA016', N'Sẵn sàng'), ('BS317', 'SA017', N'Sẵn sàng'), ('BS318', 'SA018', N'Sẵn sàng'), ('BS319', 'SA019', N'Sẵn sàng'), ('BS320', 'SA020', N'Sẵn sàng'),
('BS321', 'SA021', N'Sẵn sàng'), ('BS322', 'SA022', N'Sẵn sàng'), ('BS323', 'SA023', N'Sẵn sàng'), ('BS324', 'SA024', N'Sẵn sàng'), ('BS325', 'SA025', N'Sẵn sàng'),
('BS326', 'SA026', N'Sẵn sàng'), ('BS327', 'SA027', N'Sẵn sàng'), ('BS328', 'SA028', N'Sẵn sàng'), ('BS329', 'SA029', N'Sẵn sàng'), ('BS330', 'SA030', N'Sẵn sàng'),
('BS331', 'SA031', N'Sẵn sàng'), ('BS332', 'SA032', N'Sẵn sàng'), ('BS333', 'SA033', N'Sẵn sàng'), ('BS334', 'SA034', N'Sẵn sàng'), ('BS335', 'SA035', N'Sẵn sàng'),
('BS336', 'SA036', N'Sẵn sàng'), ('BS337', 'SA037', N'Sẵn sàng'), ('BS338', 'SA038', N'Sẵn sàng'), ('BS339', 'SA039', N'Sẵn sàng'), ('BS340', 'SA040', N'Sẵn sàng'),
('BS341', 'SA041', N'Sẵn sàng'), ('BS342', 'SA042', N'Sẵn sàng'), ('BS343', 'SA043', N'Sẵn sàng'), ('BS344', 'SA044', N'Sẵn sàng'), ('BS345', 'SA045', N'Sẵn sàng'),
('BS346', 'SA046', N'Sẵn sàng'), ('BS347', 'SA047', N'Sẵn sàng'), ('BS348', 'SA048', N'Sẵn sàng'), ('BS349', 'SA049', N'Sẵn sàng'), ('BS350', 'SA050', N'Sẵn sàng');

-- ==========================================
-- 3. NHÓM CON NGƯỜI (NHÂN VIÊN, ĐỘC GIẢ)
-- ==========================================
-- 20 Nhân viên
INSERT INTO NhanVien (MaNV, HoTen, TenDN, MatKhau, ChucVu) VALUES 
('NV001', N'Trần Văn An', 'an', '123', N'Quản lý'), 
('NV002', N'Lê Thị Bình', 'binh', '123', N'Thủ thư'),
('NV003', N'Phạm Văn Cường', 'cuong', '123', N'Thủ thư'), 
('NV004', N'Nguyễn Thị Dung', 'dung', '123', N'Thủ thư'),
('NV005', N'Hoàng Văn Em', 'em', '123', N'Thủ thư'), 
('NV006', N'Vũ Thị Giang', 'giang', '123', N'Thủ thư'),
('NV007', N'Đỗ Văn Hùng', 'hung', '123', N'Quản lý'), 
('NV008', N'Bùi Thị Hoa', 'hoa', '123', N'Thủ thư'),
('NV009', N'Lý Văn I', 'i', '123', N'Thủ thư'), 
('NV010', N'Ngô Thị Khanh', 'khanh', '123', N'Thủ thư'),
('NV011', N'Phan Văn Lâm', 'lam', '123', N'Thủ thư'), 
('NV012', N'Trịnh Thị Mai', 'mai', '123', N'Thủ thư'),
('NV013', N'Đặng Văn Nam', 'nam', '123', N'Thủ thư'), 
('NV014', N'Cao Thị Nga', 'nga', '123', N'Thủ thư'),
('NV015', N'Mai Văn Oanh', 'oanh', '123', N'Quản lý'), 
('NV016', N'Lương Thị Phương', 'phuong', '123', N'Thủ thư'),
('NV017', N'Hồ Văn Quân', 'quan', '123', N'Thủ thư'), 
('NV018', N'Dương Thị Sâm', 'sam', '123', N'Thủ thư'),
('NV019', N'Tạ Văn Tú', 'tu', '123', N'Thủ thư'), 
('NV020', N'Vương Thị Uyên', 'uyen', '123', N'Thủ thư');

-- BỔ SUNG: 50 Độc giả (Để làm khóa ngoại cho bảng Phiếu mượn)
-- Chèn theo mẫu 5 người, sau đó nhân bản tên (bạn có thể đổi tên sau nếu muốn)
INSERT INTO DocGia (MaDG, HoTen, NgaySinh, DiaChi, SoDT, TrangThaiThe) VALUES
('DG001', N'Nguyễn Văn An', '1990-05-14', N'12 Chùa Bộc, Đống Đa, Hà Nội', '0981234001', N'Bình thường'),
('DG002', N'Trần Thị Bích', '1995-08-22', N'45 Xuân Thủy, Cầu Giấy, Hà Nội', '0981234002', N'Bình thường'),
('DG003', N'Lê Hoàng Cường', '2001-01-10', N'102 Thái Hà, Đống Đa, Hà Nội', '0981234003', N'Bình thường'),
('DG004', N'Phạm Thu Dung', '1988-11-05', N'89 Nguyễn Trãi, Thanh Xuân, Hà Nội', '0981234004', N'Bình thường'),
('DG005', N'Hoàng Văn Đạt', '1992-04-30', N'25 Quang Trung, Hà Đông, Hà Nội', '0981234005', N'Bình thường'),
('DG006', N'Vũ Thị Giang', '1998-07-15', N'18 Hoàng Quốc Việt, Cầu Giấy, Hà Nội', '0981234006', N'Bình thường'),
('DG007', N'Đặng Văn Hải', '1985-09-02', N'56 Lò Đúc, Hai Bà Trưng, Hà Nội', '0981234007', N'Bình thường'),
('DG008', N'Bùi Thị Hoa', '1994-12-25', N'22 Tôn Đức Thắng, Đống Đa, Hà Nội', '0981234008', N'Bình thường'),
('DG009', N'Đỗ Quang Huy', '2000-03-18', N'14 Cát Linh, Ba Đình, Hà Nội', '0981234009', N'Bình thường'),
('DG010', N'Hồ Thị Lan', '1991-06-08', N'90 Kim Mã, Ba Đình, Hà Nội', '0981234010', N'Bình thường'),
('DG011', N'Ngô Văn Long', '1989-10-10', N'33 Nguyễn Chí Thanh, Đống Đa, Hà Nội', '0981234011', N'Bình thường'),
('DG012', N'Dương Thị Mai', '1997-02-28', N'15 Trần Phú, Hà Đông, Hà Nội', '0981234012', N'Bình thường'),
('DG013', N'Lý Văn Nam', '1993-05-19', N'78 Giải Phóng, Hoàng Mai, Hà Nội', '0981234013', N'Bình thường'),
('DG014', N'Nguyễn Thu Nga', '1996-08-04', N'101 Lê Thanh Nghị, Hai Bà Trưng, Hà Nội', '0981234014', N'Bình thường'),
('DG015', N'Trần Quang Ngọc', '1987-11-11', N'45 Nguyễn Văn Cừ, Long Biên, Hà Nội', '0981234015', N'Bình thường'),
('DG016', N'Lê Thị Oanh', '1999-12-01', N'67 Láng Hạ, Đống Đa, Hà Nội', '0981234016', N'Bình thường'),
('DG017', N'Phạm Văn Phong', '1986-04-20', N'12 Đào Tấn, Ba Đình, Hà Nội', '0981234017', N'Bình thường'),
('DG018', N'Hoàng Thị Quỳnh', '1995-01-15', N'88 Bà Triệu, Hoàn Kiếm, Hà Nội', '0981234018', N'Bình thường'),
('DG019', N'Vũ Văn Sơn', '1990-07-30', N'21 Lê Duẩn, Hoàn Kiếm, Hà Nội', '0981234019', N'Bình thường'),
('DG020', N'Đặng Thị Thu', '1998-09-09', N'34 Đại Cồ Việt, Hai Bà Trưng, Hà Nội', '0981234020', N'Bình thường'),
('DG021', N'Bùi Văn Tuấn', '1985-02-14', N'55 Minh Khai, Hai Bà Trưng, Hà Nội', '0981234021', N'Bình thường'),
('DG022', N'Đỗ Thị Uyên', '2001-05-25', N'76 Nguyễn Thái Học, Ba Đình, Hà Nội', '0981234022', N'Bình thường'),
('DG023', N'Hồ Văn Vinh', '1992-08-18', N'92 Trần Duy Hưng, Cầu Giấy, Hà Nội', '0981234023', N'Bình thường'),
('DG024', N'Ngô Thị Xuân', '1994-11-20', N'11 Khâm Thiên, Đống Đa, Hà Nội', '0981234024', N'Bình thường'),
('DG025', N'Dương Văn Yến', '1988-03-12', N'40 Phố Huế, Hai Bà Trưng, Hà Nội', '0981234025', N'Bình thường'),
('DG026', N'Nguyễn Quang Minh', '1997-06-05', N'50 Liễu Giai, Ba Đình, Hà Nội', '0981234026', N'Bình thường'),
('DG027', N'Trần Thu Hà', '1991-09-28', N'28 Trần Khát Chân, Hai Bà Trưng, Hà Nội', '0981234027', N'Bình thường'),
('DG028', N'Lê Văn Hùng', '1989-12-22', N'19 Thụy Khuê, Tây Hồ, Hà Nội', '0981234028', N'Bình thường'),
('DG029', N'Phạm Thị Tuyết', '1996-04-10', N'81 Lạc Long Quân, Tây Hồ, Hà Nội', '0981234029', N'Bình thường'),
('DG030', N'Hoàng Văn Kiên', '1993-07-07', N'36 Tràng Thi, Hoàn Kiếm, Hà Nội', '0981234030', N'Bình thường'),
('DG031', N'Vũ Thị Hương', '1990-10-31', N'14 Lý Thường Kiệt, Hoàn Kiếm, Hà Nội', '0981234031', N'Bình thường'),
('DG032', N'Đặng Văn Tiến', '1987-01-25', N'62 Phan Đình Phùng, Ba Đình, Hà Nội', '0981234032', N'Bình thường'),
('DG033', N'Bùi Thị Ngọc', '1999-05-08', N'99 Đội Cấn, Ba Đình, Hà Nội', '0981234033', N'Bình thường'),
('DG034', N'Đỗ Văn Lâm', '1986-08-14', N'27 Quán Sứ, Hoàn Kiếm, Hà Nội', '0981234034', N'Bình thường'),
('DG035', N'Hồ Thị Bích', '1995-11-02', N'44 Hàng Bún, Ba Đình, Hà Nội', '0981234035', N'Bình thường'),
('DG036', N'Ngô Văn Tùng', '1992-02-19', N'73 Trương Định, Hoàng Mai, Hà Nội', '0981234036', N'Bình thường'),
('DG037', N'Dương Thị Loan', '1988-06-26', N'16 Thanh Nhàn, Hai Bà Trưng, Hà Nội', '0981234037', N'Bình thường'),
('DG038', N'Lý Văn Đức', '1994-09-15', N'85 Lĩnh Nam, Hoàng Mai, Hà Nội', '0981234038', N'Bình thường'),
('DG039', N'Nguyễn Thị Phương', '1997-12-05', N'30 Ngọc Hồi, Thanh Trì, Hà Nội', '0981234039', N'Bình thường'),
('DG040', N'Trần Văn Thái', '1985-03-30', N'52 Nguyễn Khoái, Hai Bà Trưng, Hà Nội', '0981234040', N'Bình thường'),
('DG041', N'Lê Thị Hiền', '1991-07-21', N'23 Trần Đăng Ninh, Cầu Giấy, Hà Nội', '0981234041', N'Bình thường'),
('DG042', N'Phạm Văn Thành', '1989-10-08', N'68 Phạm Hùng, Nam Từ Liêm, Hà Nội', '0981234042', N'Bình thường'),
('DG043', N'Hoàng Thị Trang', '1998-01-12', N'17 Lê Đức Thọ, Nam Từ Liêm, Hà Nội', '0981234043', N'Bình thường'),
('DG044', N'Vũ Văn Đăng', '1993-04-04', N'49 Hồ Tùng Mậu, Bắc Từ Liêm, Hà Nội', '0981234044', N'Bình thường'),
('DG045', N'Đặng Thị Mỹ', '1996-08-27', N'91 Cầu Diễn, Bắc Từ Liêm, Hà Nội', '0981234045', N'Bình thường'),
('DG046', N'Bùi Văn Khang', '1987-11-19', N'38 Nguyễn Khánh Toàn, Cầu Giấy, Hà Nội', '0981234046', N'Bình thường'),
('DG047', N'Đỗ Thị Ngân', '1999-02-10', N'57 Tô Hiệu, Cầu Giấy, Hà Nội', '0981234047', N'Bình thường'),
('DG048', N'Hồ Văn Quyết', '1986-05-28', N'82 Duy Tân, Cầu Giấy, Hà Nội', '0981234048', N'Bình thường'),
('DG049', N'Ngô Thị Trâm', '1995-09-14', N'29 Tôn Thất Thuyết, Nam Từ Liêm, Hà Nội', '0981234049', N'Bình thường'),
('DG050', N'Dương Văn Thiện', '1990-12-31', N'64 Trung Kính, Cầu Giấy, Hà Nội', '0981234050', N'Bình thường');
-- ==========================================
-- 4. NHÓM NGHIỆP VỤ (NHẬP, MƯỢN TRẢ, PHẠT)
-- ==========================================
INSERT INTO PhieuNhap (MaPhieuNhap, MaNV, NgayNhap, TongTien) VALUES 
('PN001', 'NV001', '2023-01-10', 5800000.00),
('PN002', 'NV007', '2023-02-15', 11000000.00),
('PN003', 'NV015', '2023-03-20', 5700000.00),
('PN004', 'NV001', '2023-05-12', 9400000.00),
('PN005', 'NV007', '2023-07-08', 6300000.00),
('PN006', 'NV015', '2023-09-25', 6300000.00),
('PN007', 'NV001', '2023-11-11', 8200000.00),
('PN008', 'NV007', '2024-01-05', 8950000.00),
('PN009', 'NV015', '2024-02-28', 7800000.00),
('PN010', 'NV001', '2024-04-10', 7200000.00),
('PN011', 'NV007', '2024-05-05', 5000000.00),
('PN012', 'NV015', '2024-06-12', 7000000.00),
('PN013', 'NV001', '2024-07-20', 5700000.00),
('PN014', 'NV007', '2024-08-15', 8600000.00),
('PN015', 'NV015', '2024-09-10', 8500000.00);

INSERT INTO ChiTietPhieuNhap (MaPhieuNhap, MaSach, SoLuong, DonGia) VALUES 
('PN001', 'SA001', 10, 100000.00),
('PN001', 'SA002', 20, 150000.00),
('PN001', 'SA003', 15, 120000.00),

('PN002', 'SA010', 50, 100000.00),
('PN002', 'SA011', 30, 200000.00),

('PN003', 'SA020', 25, 140000.00),
('PN003', 'SA021', 20, 110000.00),

('PN004', 'SA030', 40, 130000.00),
('PN004', 'SA031', 10, 180000.00),
('PN004', 'SA032', 15, 160000.00),

('PN005', 'SA040', 30, 150000.00),
('PN005', 'SA045', 20, 90000.00),

('PN006', 'SA050', 10, 250000.00),
('PN006', 'SA055', 15, 120000.00),
('PN006', 'SA060', 20, 100000.00),

('PN007', 'SA070', 50, 80000.00),
('PN007', 'SA075', 30, 140000.00),

('PN008', 'SA080', 20, 210000.00),
('PN008', 'SA085', 25, 130000.00),
('PN008', 'SA090', 10, 150000.00),

('PN009', 'SA095', 40, 110000.00),
('PN009', 'SA098', 20, 170000.00),

('PN010', 'SA100', 30, 160000.00),
('PN010', 'SA005', 20, 120000.00),

('PN011', 'SA015', 20, 150000.00),
('PN011', 'SA025', 20, 100000.00),

('PN012', 'SA035', 25, 100000.00),
('PN012', 'SA042', 30, 150000.00),

('PN013', 'SA052', 30, 120000.00),
('PN013', 'SA062', 15, 140000.00),

('PN014', 'SA072', 20, 200000.00),
('PN014', 'SA082', 10, 160000.00),
('PN014', 'SA092', 20, 150000.00),

('PN015', 'SA008', 40, 125000.00),
('PN015', 'SA018', 25, 140000.00);

-- 50 Phiếu Mượn (Map với Độc Giả và Nhân Viên)
INSERT INTO PhieuMuon (MaPhieu, MaDG, MaNV, NgayMuon, HanTra) VALUES 
('PM001', 'DG001', 'NV001', '2024-04-01', '2024-04-10'), ('PM002', 'DG002', 'NV002', '2024-04-02', '2024-04-12'), ('PM003', 'DG003', 'NV003', '2024-04-03', '2024-04-13'),
('PM004', 'DG004', 'NV004', '2024-04-04', '2024-04-14'), ('PM005', 'DG005', 'NV005', '2024-04-05', '2024-04-15'), ('PM006', 'DG006', 'NV006', '2024-04-06', '2024-04-16'),
('PM007', 'DG007', 'NV007', '2024-04-07', '2024-04-17'), ('PM008', 'DG008', 'NV008', '2024-04-08', '2024-04-18'), ('PM009', 'DG009', 'NV009', '2024-04-09', '2024-04-19'),
('PM010', 'DG010', 'NV010', '2024-04-10', '2024-04-20'), ('PM011', 'DG011', 'NV011', '2024-04-11', '2024-04-21'), ('PM012', 'DG012', 'NV012', '2024-04-12', '2024-04-22'),
('PM013', 'DG013', 'NV013', '2024-04-13', '2024-04-23'), ('PM014', 'DG014', 'NV014', '2024-04-14', '2024-04-24'), ('PM015', 'DG015', 'NV015', '2024-04-15', '2024-04-25'),
('PM016', 'DG016', 'NV016', '2024-04-16', '2024-04-26'), ('PM017', 'DG017', 'NV017', '2024-04-17', '2024-04-27'), ('PM018', 'DG018', 'NV018', '2024-04-18', '2024-04-28'),
('PM019', 'DG019', 'NV019', '2024-04-19', '2024-04-29'), ('PM020', 'DG020', 'NV020', '2024-04-20', '2024-04-30'), ('PM021', 'DG021', 'NV001', '2024-04-21', '2024-05-01'),
('PM022', 'DG022', 'NV002', '2024-04-22', '2024-05-02'), ('PM023', 'DG023', 'NV003', '2024-04-23', '2024-05-03'), ('PM024', 'DG024', 'NV004', '2024-04-24', '2024-05-04'),
('PM025', 'DG025', 'NV005', '2024-04-25', '2024-05-05'), ('PM026', 'DG026', 'NV006', '2024-04-26', '2024-05-06'), ('PM027', 'DG027', 'NV007', '2024-04-27', '2024-05-07'),
('PM028', 'DG028', 'NV008', '2024-04-28', '2024-05-08'), ('PM029', 'DG029', 'NV009', '2024-04-29', '2024-05-09'), ('PM030', 'DG030', 'NV010', '2024-04-30', '2024-05-10'),
('PM031', 'DG031', 'NV011', '2024-05-01', '2024-05-11'), ('PM032', 'DG032', 'NV012', '2024-05-02', '2024-05-12'), ('PM033', 'DG033', 'NV013', '2024-05-03', '2024-05-13'),
('PM034', 'DG034', 'NV014', '2024-05-04', '2024-05-14'), ('PM035', 'DG035', 'NV015', '2024-05-05', '2024-05-15'), ('PM036', 'DG036', 'NV016', '2024-05-06', '2024-05-16'),
('PM037', 'DG037', 'NV017', '2024-05-07', '2024-05-17'), ('PM038', 'DG038', 'NV018', '2024-05-08', '2024-05-18'), ('PM039', 'DG039', 'NV019', '2024-05-09', '2024-05-19'),
('PM040', 'DG040', 'NV020', '2024-05-10', '2024-05-20'), ('PM041', 'DG041', 'NV001', '2024-05-11', '2024-05-21'), ('PM042', 'DG042', 'NV002', '2024-05-12', '2024-05-22'),
('PM043', 'DG043', 'NV003', '2024-05-13', '2024-05-23'), ('PM044', 'DG044', 'NV004', '2024-05-14', '2024-05-24'), ('PM045', 'DG045', 'NV005', '2024-05-15', '2024-05-25'),
('PM046', 'DG046', 'NV006', '2024-05-16', '2024-05-26'), ('PM047', 'DG047', 'NV007', '2024-05-17', '2024-05-27'), ('PM048', 'DG048', 'NV008', '2024-05-18', '2024-05-28'),
('PM049', 'DG049', 'NV009', '2024-05-19', '2024-05-29'), ('PM050', 'DG050', 'NV010', '2024-05-20', '2024-05-30');

-- 50 Chi tiết phiếu mượn (Map với Mã bản sao BSxxx)
INSERT INTO ChiTietPhieuMuon (MaPhieu, MaBS, NgayTraThucTe, TrangThaiTra) VALUES 
('PM001', 'BS001', '2024-04-12', N'Đã trả'), ('PM002', 'BS002', '2024-04-12', N'Đã trả'), ('PM003', 'BS003', '2024-04-15', N'Đã trả'),
('PM004', 'BS004', '2024-04-14', N'Đã trả'), ('PM005', 'BS005', '2024-04-18', N'Đã trả'), ('PM006', 'BS006', NULL, N'Đang mượn'),
('PM007', 'BS007', '2024-04-17', N'Đã trả'), ('PM008', 'BS008', NULL, N'Đang mượn'), ('PM009', 'BS009', '2024-04-25', N'Đã trả'),
('PM010', 'BS010', '2024-04-20', N'Đã trả'), ('PM011', 'BS011', NULL, N'Đang mượn'), ('PM012', 'BS012', '2024-04-22', N'Đã trả'),
('PM013', 'BS013', '2024-04-23', N'Đã trả'), ('PM014', 'BS014', '2024-04-30', N'Đã trả'), ('PM015', 'BS015', '2024-04-25', N'Đã trả'),
('PM016', 'BS016', NULL, N'Đang mượn'), ('PM017', 'BS017', '2024-04-27', N'Đã trả'), ('PM018', 'BS018', NULL, N'Đang mượn'),
('PM019', 'BS019', '2024-04-29', N'Đã trả'), ('PM020', 'BS020', '2024-04-30', N'Đã trả'), ('PM021', 'BS021', NULL, N'Đang mượn'),
('PM022', 'BS022', '2024-05-02', N'Đã trả'), ('PM023', 'BS023', '2024-05-03', N'Đã trả'), ('PM024', 'BS024', '2024-05-04', N'Đã trả'),
('PM025', 'BS025', '2024-05-05', N'Đã trả'), ('PM026', 'BS026', '2024-05-06', N'Đã trả'), ('PM027', 'BS027', '2024-05-07', N'Đã trả'),
('PM028', 'BS028', '2024-05-08', N'Đã trả'), ('PM029', 'BS029', '2024-05-09', N'Đã trả'), ('PM030', 'BS030', '2024-05-10', N'Đã trả'),
('PM031', 'BS031', '2024-05-11', N'Đã trả'), ('PM032', 'BS032', '2024-05-12', N'Đã trả'), ('PM033', 'BS033', '2024-05-13', N'Đã trả'),
('PM034', 'BS034', '2024-05-14', N'Đã trả'), ('PM035', 'BS035', '2024-05-15', N'Đã trả'), ('PM036', 'BS036', '2024-05-16', N'Đã trả'),
('PM037', 'BS037', '2024-05-17', N'Đã trả'), ('PM038', 'BS038', '2024-05-18', N'Đã trả'), ('PM039', 'BS039', '2024-05-19', N'Đã trả'),
('PM040', 'BS040', '2024-05-20', N'Đã trả'), ('PM041', 'BS041', '2024-05-21', N'Đã trả'), ('PM042', 'BS042', '2024-05-22', N'Đã trả'),
('PM043', 'BS043', '2024-05-23', N'Đã trả'), ('PM044', 'BS044', '2024-05-24', N'Đã trả'), ('PM045', 'BS045', '2024-05-25', N'Đã trả'),
('PM046', 'BS046', '2024-05-26', N'Đã trả'), ('PM047', 'BS047', '2024-05-27', N'Đã trả'), ('PM048', 'BS048', '2024-05-28', N'Đã trả'),
('PM049', 'BS049', '2024-05-29', N'Đã trả'), ('PM050', 'BS050', '2024-05-30', N'Đã trả');

INSERT INTO PhieuPhat (MaPhieu, MaBS, SoTienPhat, LyDo) VALUES 
('PM001', 'BS001', 10000, N'Trả trễ hạn 2 ngày'),
('PM003', 'BS003', 10000, N'Trả trễ hạn 2 ngày'),
('PM005', 'BS005', 15000, N'Trả trễ hạn 3 ngày'),
('PM009', 'BS009', 30000, N'Trả trễ hạn 6 ngày');
GO

-- ===================================================
-- 1. FUNCTION: Tính tiền phạt
-- ===================================================
GO
CREATE FUNCTION fn_TinhTienPhat (
    @HanTra DATE,
    @NgayTraThucTe DATE
)
RETURNS DECIMAL(18,2)
AS
BEGIN
    DECLARE @TienPhat DECIMAL(18,2) = 0;
    IF (@NgayTraThucTe > @HanTra)
    BEGIN
        -- Phạt 10.000đ cho mỗi ngày trễ
        SET @TienPhat = DATEDIFF(DAY, @HanTra, @NgayTraThucTe) * 10000;
    END
    RETURN @TienPhat;
END;
GO

-- ===================================================
-- 2. PROCEDURE & TRANSACTION: Xử lý trả sách
-- ===================================================
CREATE PROCEDURE sp_TraSach
    @MaPhieu CHAR(5),
    @MaBS CHAR(5),
    @NgayTra DATE
AS
BEGIN
    -- Thiết lập mức độ cô lập để tránh xung đột dữ liệu (Tiêu chí 3: Quản lý giao tác)
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 
    
    BEGIN TRANSACTION
    BEGIN TRY
        DECLARE @HanTra DATE = (SELECT HanTra FROM PhieuMuon WHERE MaPhieu = @MaPhieu);
        DECLARE @TienPhat DECIMAL(18,2) = dbo.fn_TinhTienPhat(@HanTra, @NgayTra);

        -- Cập nhật chi tiết phiếu mượn
        UPDATE ChiTietPhieuMuon 
        SET NgayTraThucTe = @NgayTra, 
            TrangThaiTra = N'Đã trả'
        WHERE MaPhieu = @MaPhieu AND MaBS = @MaBS;

        -- Cập nhật sách về lại kho
        UPDATE BanSaoSach 
        SET TinhTrang = N'Sẵn sàng' 
        WHERE MaBS = @MaBS;

        -- Nếu có phạt thì tự động sinh Phiếu Phạt
        IF (@TienPhat > 0)
        BEGIN
            INSERT INTO PhieuPhat (MaPhieu, MaBS, SoTienPhat, LyDo)
            VALUES (@MaPhieu, @MaBS, @TienPhat, N'Trả trễ hạn');
        END

        COMMIT TRANSACTION;
        PRINT N'Trả sách thành công!';
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        DECLARE @ErrorMsg NVARCHAR(MAX) = ERROR_MESSAGE();
        RAISERROR(@ErrorMsg, 16, 1);
    END CATCH
END;
GO
-- ===================================================
-- PROCEDURE & TRANSACTION: Xử lý mượn sách
-- ===================================================
CREATE PROCEDURE sp_MuonSach
    @MaPhieu CHAR(5),     
    @MaDG CHAR(5),      
    @MaNV CHAR(5),        
    @MaBS CHAR(5),         
    @SoNgayMuon INT = 14   
AS
BEGIN
 
    SET TRANSACTION ISOLATION LEVEL READ COMMITTED; 
    
    BEGIN TRANSACTION;
    BEGIN TRY
     
        DECLARE @TinhTrang NVARCHAR(50);
        SELECT @TinhTrang = TinhTrang FROM BanSaoSach WITH (UPDLOCK) WHERE MaBS = @MaBS;

        IF @TinhTrang IS NULL
        BEGIN
            RAISERROR(N'Lỗi: Bản sao sách không tồn tại trong hệ thống!', 16, 1);
        END
        ELSE IF @TinhTrang <> N'Sẵn sàng'
        BEGIN
            RAISERROR(N'Lỗi: Sách này hiện đang được mượn hoặc không sẵn sàng!', 16, 1);
        END

    
        IF NOT EXISTS (SELECT 1 FROM PhieuMuon WHERE MaPhieu = @MaPhieu)
        BEGIN
            INSERT INTO PhieuMuon (MaPhieu, MaDG, MaNV, NgayMuon, HanTra)
            VALUES (@MaPhieu, @MaDG, @MaNV, GETDATE(), DATEADD(DAY, @SoNgayMuon, GETDATE()));
        END

        INSERT INTO ChiTietPhieuMuon (MaPhieu, MaBS, NgayTraThucTe, TrangThaiTra)
        VALUES (@MaPhieu, @MaBS, NULL, N'Đang mượn');

        UPDATE BanSaoSach
        SET TinhTrang = N'Đang mượn'
        WHERE MaBS = @MaBS;

        COMMIT TRANSACTION;
        PRINT N'Mượn sách thành công! Mã bản sao: ' + @MaBS;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
   
        DECLARE @ErrorMsg NVARCHAR(MAX) = ERROR_MESSAGE();
        RAISERROR(@ErrorMsg, 16, 1);
    END CATCH
END;
GO
-- ===================================================
-- 3. TRIGGER: Kiểm tra số sách mượn tối đa
-- ===================================================
CREATE TRIGGER trg_KiemTraMuonSach
ON ChiTietPhieuMuon
FOR INSERT
AS
BEGIN
    IF EXISTS (
        SELECT P.MaDG
        FROM PhieuMuon P
        JOIN ChiTietPhieuMuon CT ON P.MaPhieu = CT.MaPhieu
        WHERE CT.TrangThaiTra = N'Đang mượn'
          AND P.MaDG IN (
  
              SELECT DISTINCT PM.MaDG 
              FROM inserted I
              JOIN PhieuMuon PM ON I.MaPhieu = PM.MaPhieu
          )
        GROUP BY P.MaDG
        HAVING COUNT(CT.MaBS) > 3
    )
    BEGIN
        RAISERROR(N'Lỗi: Giao dịch thất bại! Có độc giả trong phiên giao dịch này đang mượn vượt quá 3 cuốn sách.', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- ===================================================
-- 4. VIEW: Thống kê sách 
-- ===================================================
CREATE VIEW v_ThongKeSach AS
SELECT 
    S.MaSach, 
    S.TenSach, 
    -- Dùng STRING_AGG để gộp nhiều thể loại thành 1 chuỗi
    (SELECT STRING_AGG(L.TenLoai, ', ') 
     FROM ChiTietTheLoai CT, LoaiSach L 
     WHERE CT.MaLoai = L.MaLoai 
       AND CT.MaSach = S.MaSach) AS CacTheLoai,
    (SELECT COUNT(*) 
     FROM BanSaoSach 
     WHERE MaSach = S.MaSach AND TinhTrang = N'Sẵn sàng') AS SoLuongSanSang
FROM Sach S;
GO

-- ===================================================
-- 5. CURSOR: Duyệt độc giả nợ sách quá hạn
-- ===================================================
CREATE PROCEDURE sp_DanhSachNoQuaHan
AS
BEGIN
    DECLARE @TenDG NVARCHAR(100), @TenS NVARCHAR(200);
    
    DECLARE cur_DocGiaNoSach CURSOR FOR

        SELECT DG.HoTen, S.TenSach
        FROM DocGia DG, PhieuMuon PM, ChiTietPhieuMuon CTPM, BanSaoSach BS, Sach S
        WHERE DG.MaDG = PM.MaDG 
          AND PM.MaPhieu = CTPM.MaPhieu 
          AND CTPM.MaBS = BS.MaBS 
          AND BS.MaSach = S.MaSach
          AND CTPM.TrangThaiTra = N'Đang mượn' 
          AND PM.HanTra < GETDATE();
    
    OPEN cur_DocGiaNoSach;
    FETCH NEXT FROM cur_DocGiaNoSach INTO @TenDG, @TenS;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        PRINT N'Độc giả: ' + @TenDG + N' đang nợ cuốn: ' + @TenS;
        FETCH NEXT FROM cur_DocGiaNoSach INTO @TenDG, @TenS;
    END;
    
    CLOSE cur_DocGiaNoSach;
    DEALLOCATE cur_DocGiaNoSach;
END;
GO
-- =========================================================
-- PHẦN A. QUẢN TRỊ NGƯỜI DÙNG VÀ PHÂN QUYỀN (SECURITY)
-- =========================================================

-- 1. Tạo các nhóm quyền (Roles) theo nghiệp vụ thực tế
CREATE ROLE Role_QuanLy;
CREATE ROLE Role_ThuThu;
GO

-- 2. Thiết lập quyền cho Nhóm Quản Lý (Role_QuanLy)
-- Quản lý được phép làm mọi thao tác (Thêm, Sửa, Xóa, Xem) trên tất cả các bảng nghiệp vụ và danh mục
GRANT SELECT, INSERT, UPDATE, DELETE ON LoaiSach TO Role_QuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON NhaXuatBan TO Role_QuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON TacGia TO Role_QuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON Sach TO Role_QuanLy;
GRANT SELECT, INSERT, UPDATE, DELETE ON NhanVien TO Role_QuanLy;
GRANT EXECUTE TO Role_QuanLy; -- Được phép chạy tất cả các Procedure/Function
GO

-- 3. Thiết lập quyền cho Nhóm Thủ Thư (Role_ThuThu)
-- Thủ thư chỉ được thao tác các nghiệp vụ mượn, trả, lập thẻ độc giả.
-- TUYỆT ĐỐI KHÔNG cấp quyền DELETE để tránh gian lận xóa dữ liệu phiếu mượn/phạt.
GRANT SELECT, INSERT, UPDATE ON DocGia TO Role_ThuThu;
GRANT SELECT, INSERT, UPDATE ON PhieuMuon TO Role_ThuThu;
GRANT SELECT, INSERT, UPDATE ON ChiTietPhieuMuon TO Role_ThuThu;
GRANT SELECT, INSERT ON PhieuPhat TO Role_ThuThu;
GRANT SELECT ON Sach TO Role_ThuThu;
GRANT SELECT ON BanSaoSach TO Role_ThuThu;
GRANT SELECT ON v_ThongKeSach TO Role_ThuThu;

-- Cấp quyền chạy Procedure trả sách cho thủ thư
GRANT EXECUTE ON OBJECT::sp_TraSach TO Role_ThuThu;

-- Từ chối (DENY) thủ thư không được xem bảng Nhân viên (Bảo mật thông tin lương, mật khẩu nội bộ)
DENY SELECT ON NhanVien TO Role_ThuThu;
GO

-- 4. Tạo tài khoản đăng nhập (Login) cấp Server
-- Lưu ý: Đổi mật khẩu theo chính sách bảo mật (có chữ hoa, số, ký tự đặc biệt)
CREATE LOGIN login_QuanLy_NV001 WITH PASSWORD = 'PasswordQuanLy@2024';
CREATE LOGIN login_ThuThu_NV002 WITH PASSWORD = 'PasswordThuThu@2024';
GO

-- 5. Tạo Người dùng (User) cấp Database và ánh xạ vào Login
CREATE USER user_QuanLy_NV001 FOR LOGIN login_QuanLy_NV001;
CREATE USER user_ThuThu_NV002 FOR LOGIN login_ThuThu_NV002;
GO

-- 6. Gán User vào Role tương ứng
ALTER ROLE Role_QuanLy ADD MEMBER user_QuanLy_NV001;
ALTER ROLE Role_ThuThu ADD MEMBER user_ThuThu_NV002;
GO


-- =========================================================
-- PHẦN B. SAO LƯU VÀ PHỤC HỒI DỮ LIỆU (BACKUP & RESTORE)
-- LƯU Ý KHI CHẠY: Hãy đảm bảo ổ đĩa D: có thư mục tên là "Backup" (D:\Backup\)
-- =========================================================
ALTER DATABASE QuanLyThuVien SET RECOVERY FULL;
GO
-- Kịch bản: Quản trị viên thiết lập lịch sao lưu định kỳ để chống mất dữ liệu.

-- 1. SAO LƯU TOÀN PHẦN (FULL BACKUP)
-- Thực hiện mỗi cuối tuần (ví dụ: Chủ Nhật) để sao lưu toàn bộ cấu trúc và dữ liệu.
BACKUP DATABASE QuanLyThuVien 
TO DISK = 'D:\Backup\QuanLyThuVien_Full.bak'
WITH FORMAT, 
     INIT, 
     NAME = 'Quản Lý Thư Viện - Bản sao lưu toàn phần',
     STATS = 10;
GO

-- 2. SAO LƯU KHÁC BIỆT (DIFFERENTIAL BACKUP)
-- Thực hiện mỗi cuối ngày. Chỉ sao lưu những dữ liệu bị thay đổi kể từ bản Full Backup gần nhất (giúp tiết kiệm dung lượng).
BACKUP DATABASE QuanLyThuVien 
TO DISK = 'D:\Backup\QuanLyThuVien_Diff.bak'
WITH DIFFERENTIAL,
     NAME = 'Quản Lý Thư Viện - Bản sao lưu khác biệt',
     STATS = 10;
GO

-- 3. SAO LƯU NHẬT KÝ GIAO DỊCH (TRANSACTION LOG BACKUP)
-- Thực hiện mỗi giờ. Giúp khôi phục dữ liệu tới từng phút giây trước khi xảy ra sự cố (chỉ áp dụng nếu Recovery Model là Full).
BACKUP LOG QuanLyThuVien 
TO DISK = 'D:\Backup\QuanLyThuVien_Log.trn'
WITH NAME = 'Quản Lý Thư Viện - Bản sao lưu nhật ký giao dịch',
     STATS = 10;
GO

-- =========================================================
-- PHẦN C. KỊCH BẢN PHỤC HỒI DỮ LIỆU (RESTORE)
-- CHÚ Ý: Chạy riêng từng lệnh dưới đây khi CSDL gặp sự cố
-- =========================================================
/*
USE master;
GO

-- BƯỚC 1: Khôi phục bản Full Backup gốc (sử dụng NORECOVERY để cho phép khôi phục tiếp Diff/Log)
RESTORE DATABASE QuanLyThuVien 
FROM DISK = 'D:\Backup\QuanLyThuVien_Full.bak' 
WITH REPLACE, NORECOVERY, STATS = 10;

-- BƯỚC 2: Khôi phục bản thay đổi gần nhất (Diff Backup)
RESTORE DATABASE QuanLyThuVien 
FROM DISK = 'D:\Backup\QuanLyThuVien_Diff.bak' 
WITH NORECOVERY, STATS = 10;

-- BƯỚC 3: Khôi phục Log Backup mới nhất và mở database (RECOVERY) cho người dùng sử dụng
RESTORE LOG QuanLyThuVien 
FROM DISK = 'D:\Backup\QuanLyThuVien_Log.trn' 
WITH RECOVERY, STATS = 10;
GO
*/