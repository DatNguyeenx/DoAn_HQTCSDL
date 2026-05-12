using QuanLyThuVien.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QuanLyThuVien.ViewModels
{
    public class DangMuonDTO : BaseViewModel
    {
        public string MaPhieu { get; set; }
        public string MaBS { get; set; }
        public string MaDG { get; set; }
        public string TenDocGia { get; set; }
        public string TenSach { get; set; }
        public DateTime? NgayMuon { get; set; }
        public DateTime? HanTra { get; set; }
        public string MaNV { get; set; }
    }

    public class MuonSachViewModel : BaseViewModel
    {
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); }
        }

        private ObservableCollection<DangMuonDTO> _listDangMuon;
        public ObservableCollection<DangMuonDTO> ListDangMuon
        {
            get { return _listDangMuon; }
            set { _listDangMuon = value; OnPropertyChanged(nameof(ListDangMuon)); }
        }

        private DangMuonDTO _selectedPhieu;
        public DangMuonDTO SelectedPhieu
        {
            get { return _selectedPhieu; }
            set
            {
                if (_selectedPhieu == value) return;
                _selectedPhieu = value;
                OnPropertyChanged(nameof(SelectedPhieu));
                if (_selectedPhieu != null)
                {
                    MaPhieu = _selectedPhieu.MaPhieu;
                    MaDG = _selectedPhieu.MaDG;
                    MaBS = _selectedPhieu.MaBS;

                    if (_selectedPhieu.NgayMuon.HasValue && _selectedPhieu.HanTra.HasValue)
                    {
                        TimeSpan diff = _selectedPhieu.HanTra.Value - _selectedPhieu.NgayMuon.Value;
                        SoNgayMuon = diff.Days.ToString();
                    }
                    StatusMessage = string.Empty;
                }
            }
        }

        private string _maPhieu;
        public string MaPhieu
        {
            get { return _maPhieu; }
            set { _maPhieu = value; OnPropertyChanged(nameof(MaPhieu)); }
        }

        private string _soNgayMuon;
        public string SoNgayMuon
        {
            get { return _soNgayMuon; }
            set { _soNgayMuon = value; OnPropertyChanged(nameof(SoNgayMuon)); }
        }

        private string _maDG;
        public string MaDG
        {
            get { return _maDG; }
            set
            {
                _maDG = value;
                OnPropertyChanged(nameof(MaDG));
                FetchDocGiaInfo();
            }
        }

        private string _tenDocGiaGoiY;
        public string TenDocGiaGoiY
        {
            get { return _tenDocGiaGoiY; }
            set { _tenDocGiaGoiY = value; OnPropertyChanged(nameof(TenDocGiaGoiY)); }
        }

        private bool _hasDocGia;
        public bool HasDocGia
        {
            get { return _hasDocGia; }
            set { _hasDocGia = value; OnPropertyChanged(nameof(HasDocGia)); }
        }

        private string _maBS;
        public string MaBS
        {
            get { return _maBS; }
            set
            {
                _maBS = value;
                OnPropertyChanged(nameof(MaBS));
                FetchSachInfo();
            }
        }

        private string _tenSachGoiY;
        public string TenSachGoiY
        {
            get { return _tenSachGoiY; }
            set { _tenSachGoiY = value; OnPropertyChanged(nameof(TenSachGoiY)); }
        }

        private bool _hasSach;
        public bool HasSach
        {
            get { return _hasSach; }
            set { _hasSach = value; OnPropertyChanged(nameof(HasSach)); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
                OnPropertyChanged(nameof(HasMessage));
            }
        }

        public bool HasMessage => !string.IsNullOrWhiteSpace(StatusMessage);

        // Các Command đồng bộ với giao diện XAML
        public ICommand SearchCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public MuonSachViewModel()
        {
            SoNgayMuon = "14";
            ListDangMuon = new ObservableCollection<DangMuonDTO>();

            LoadData();

            SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteAlways);
            AddCommand = new RelayCommand(ExecuteAdd, CanExecuteAlways);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecuteAlways);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteAlways);
            ClearCommand = new RelayCommand(ExecuteClearCommand, CanExecuteAlways);
        }

        private bool CanExecuteAlways(object parameter) => true;

        private void LoadData()
        {
            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var query = from ct in context.ChiTietPhieuMuons
                                join pm in context.PhieuMuons on ct.MaPhieu equals pm.MaPhieu
                                join dg in context.DocGias on pm.MaDG equals dg.MaDG
                                join bs in context.BanSaoSaches on ct.MaBS equals bs.MaBS
                                join s in context.Saches on bs.MaSach equals s.MaSach
                                where ct.TrangThaiTra == "Đang mượn"
                                orderby pm.NgayMuon descending
                                select new DangMuonDTO
                                {
                                    MaPhieu = pm.MaPhieu,
                                    MaBS = ct.MaBS,
                                    MaDG = pm.MaDG,
                                    TenDocGia = dg.HoTen,
                                    TenSach = s.TenSach,
                                    NgayMuon = pm.NgayMuon,
                                    HanTra = pm.HanTra,
                                    MaNV = pm.MaNV
                                };

                    ListDangMuon = new ObservableCollection<DangMuonDTO>(query.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tải dữ liệu: " + ex.GetBaseException().Message;
            }
        }

        private void ExecuteSearch(object parameter)
        {
            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var query = from ct in context.ChiTietPhieuMuons
                                join pm in context.PhieuMuons on ct.MaPhieu equals pm.MaPhieu
                                join dg in context.DocGias on pm.MaDG equals dg.MaDG
                                join bs in context.BanSaoSaches on ct.MaBS equals bs.MaBS
                                join s in context.Saches on bs.MaSach equals s.MaSach
                                where ct.TrangThaiTra == "Đang mượn"
                                select new DangMuonDTO
                                {
                                    MaPhieu = pm.MaPhieu,
                                    MaBS = ct.MaBS,
                                    MaDG = pm.MaDG,
                                    TenDocGia = dg.HoTen,
                                    TenSach = s.TenSach,
                                    NgayMuon = pm.NgayMuon,
                                    HanTra = pm.HanTra,
                                    MaNV = pm.MaNV
                                };

                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    {
                        string kw = SearchKeyword.ToLower();
                        query = query.Where(x => x.MaPhieu.ToLower().Contains(kw) ||
                                                 x.MaBS.ToLower().Contains(kw) ||
                                                 x.TenDocGia.ToLower().Contains(kw) ||
                                                 x.TenSach.ToLower().Contains(kw));
                    }

                    ListDangMuon = new ObservableCollection<DangMuonDTO>(query.OrderByDescending(x => x.NgayMuon).ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tìm kiếm: " + ex.GetBaseException().Message;
            }
        }

        private void FetchDocGiaInfo()
        {
            if (string.IsNullOrWhiteSpace(MaDG))
            {
                TenDocGiaGoiY = string.Empty;
                HasDocGia = false;
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var dg = context.DocGias.FirstOrDefault(x => x.MaDG == MaDG);
                    if (dg != null)
                    {
                        TenDocGiaGoiY = $"Độc giả: {dg.HoTen} - Trạng thái thẻ: {dg.TrangThaiThe}";
                        HasDocGia = true;
                    }
                    else
                    {
                        TenDocGiaGoiY = "Không tìm thấy độc giả!";
                        HasDocGia = true;
                    }
                }
            }
            catch
            {
                TenDocGiaGoiY = string.Empty;
                HasDocGia = false;
            }
        }

        private void FetchSachInfo()
        {
            if (string.IsNullOrWhiteSpace(MaBS))
            {
                TenSachGoiY = string.Empty;
                HasSach = false;
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var bs = context.BanSaoSaches.Include("Sach").FirstOrDefault(x => x.MaBS == MaBS);
                    if (bs != null)
                    {
                        TenSachGoiY = $"Sách: {bs.Sach.TenSach} - Tình trạng: {bs.TinhTrang}";
                        HasSach = true;
                    }
                    else
                    {
                        TenSachGoiY = "Không tìm thấy mã bản sao này!";
                        HasSach = true;
                    }
                }
            }
            catch
            {
                TenSachGoiY = string.Empty;
                HasSach = false;
            }
        }

        private void ExecuteAdd(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaPhieu) || string.IsNullOrWhiteSpace(MaDG) || string.IsNullOrWhiteSpace(MaBS))
            {
                StatusMessage = "Vui lòng nhập đầy đủ Mã phiếu, Mã độc giả và Mã bản sao!";
                return;
            }

            if (GlobalStore.CurrentAccount == null)
            {
                StatusMessage = "Lỗi: Không xác định được nhân viên đang đăng nhập!";
                return;
            }

            int soNgay = 14;
            if (!string.IsNullOrWhiteSpace(SoNgayMuon))
            {
                if (!int.TryParse(SoNgayMuon, out soNgay) || soNgay <= 0)
                {
                    StatusMessage = "Số ngày mượn phải là số nguyên dương!";
                    return;
                }
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    string currentNV = GlobalStore.CurrentAccount.MaNV;
                    context.sp_MuonSach(MaPhieu, MaDG, currentNV, MaBS, soNgay);

                    StatusMessage = $"Thêm mới mượn sách thành công! Mã bản sao: {MaBS}";

                    MaBS = string.Empty;
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.GetBaseException().Message;
            }
        }


        private void ExecuteClearCommand(object parameter)
        {
            MaPhieu = string.Empty;
            MaDG = string.Empty;
            MaBS = string.Empty;
            SoNgayMuon = "14";
            StatusMessage = string.Empty;
            SearchKeyword = string.Empty;
            SelectedPhieu = null;
            LoadData();
        }
        private void ExecuteUpdate(object parameter)
        {
            if (SelectedPhieu == null)
            {
                StatusMessage = "Vui lòng click chọn một phiếu mượn trong danh sách để sửa!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    // Tìm phiếu mượn gốc trong DB
                    var phieu = context.PhieuMuons.FirstOrDefault(x => x.MaPhieu == SelectedPhieu.MaPhieu);

                    if (phieu != null)
                    {
                        // Cập nhật lại số ngày mượn -> Tính lại Hạn trả mới
                        if (int.TryParse(SoNgayMuon, out int soNgay) && soNgay > 0 && phieu.NgayMuon.HasValue)
                        {
                            phieu.HanTra = phieu.NgayMuon.Value.AddDays(soNgay);
                        }

                        // Cập nhật thêm Mã Độc Giả nếu bạn có sửa nó trên form
                        if (!string.IsNullOrWhiteSpace(MaDG))
                        {
                            phieu.MaDG = MaDG;
                        }

                        context.SaveChanges();
                        StatusMessage = $"Đã cập nhật thành công Phiếu mượn: {phieu.MaPhieu}!";

                        // Tải lại danh sách để hiện dữ liệu mới
                        LoadData();
                    }
                    else
                    {
                        StatusMessage = "Không tìm thấy phiếu mượn này trong cơ sở dữ liệu!";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi khi sửa: " + ex.GetBaseException().Message;
            }
        }

        private void ExecuteDelete(object parameter)
        {
            if (SelectedPhieu == null)
            {
                StatusMessage = "Vui lòng click chọn một phiếu mượn trong danh sách để xóa!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    // 1. Tìm và xóa chi tiết phiếu mượn (cuốn sách cụ thể đang chọn)
                    var chiTiet = context.ChiTietPhieuMuons.FirstOrDefault(x =>
                        x.MaPhieu == SelectedPhieu.MaPhieu &&
                        x.MaBS == SelectedPhieu.MaBS);

                    if (chiTiet != null)
                    {
                        context.ChiTietPhieuMuons.Remove(chiTiet);

                        // 2. Kiểm tra xem Phiếu mượn này còn cuốn sách nào khác không?
                        bool conSachKhac = context.ChiTietPhieuMuons.Any(x =>
                            x.MaPhieu == SelectedPhieu.MaPhieu &&
                            x.MaBS != SelectedPhieu.MaBS);

                        // 3. Nếu không còn cuốn sách nào khác -> Xóa luôn phiếu mượn gốc
                        if (!conSachKhac)
                        {
                            var phieuGoc = context.PhieuMuons.FirstOrDefault(x => x.MaPhieu == SelectedPhieu.MaPhieu);
                            if (phieuGoc != null)
                            {
                                context.PhieuMuons.Remove(phieuGoc);
                            }
                        }

                        // Lưu thay đổi xuống DB
                        context.SaveChanges();

                        StatusMessage = $"Đã xóa thành công mã bản sao {SelectedPhieu.MaBS} khỏi phiếu {SelectedPhieu.MaPhieu}!";

                        // Reset lại form và load lại lưới dữ liệu
                        ExecuteClearCommand(null);
                    }
                    else
                    {
                        StatusMessage = "Không tìm thấy dữ liệu mượn sách này để xóa!";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi khi xóa: " + ex.GetBaseException().Message;
            }
        }
    }
}