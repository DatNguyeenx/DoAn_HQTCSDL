using QuanLyThuVien.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyThuVien.ViewModels
{
    public class ChiTietPhieuNhapDTO : BaseViewModel
    {
        public string MaPhieuNhap { get; set; }
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public int SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien => SoLuong * DonGia;
    }

    public class PhieuNhapViewModel : BaseViewModel
    {
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); }
        }

        private ObservableCollection<PhieuNhap> _listPhieuNhap;
        public ObservableCollection<PhieuNhap> ListPhieuNhap
        {
            get { return _listPhieuNhap; }
            set { _listPhieuNhap = value; OnPropertyChanged(nameof(ListPhieuNhap)); }
        }

        private PhieuNhap _selectedPhieuNhap;
        public PhieuNhap SelectedPhieuNhap
        {
            get { return _selectedPhieuNhap; }
            set
            {
                if (_selectedPhieuNhap == value) return;
                _selectedPhieuNhap = value;
                OnPropertyChanged(nameof(SelectedPhieuNhap));

                if (_selectedPhieuNhap != null)
                {
                    MaPhieuNhap = _selectedPhieuNhap.MaPhieuNhap;
                    NgayNhap = _selectedPhieuNhap.NgayNhap;
                    TongTien = _selectedPhieuNhap.TongTien ?? 0;
                    IsEditingPhieuMode = false;
                    LoadChiTietPhieuNhap(MaPhieuNhap);
                }
                else
                {
                    ClearPhieuNhapForm();
                }
            }
        }

        private string _maPhieuNhap;
        public string MaPhieuNhap
        {
            get { return _maPhieuNhap; }
            set { _maPhieuNhap = value; OnPropertyChanged(nameof(MaPhieuNhap)); }
        }

        private DateTime? _ngayNhap;
        public DateTime? NgayNhap
        {
            get { return _ngayNhap; }
            set { _ngayNhap = value; OnPropertyChanged(nameof(NgayNhap)); }
        }

        private decimal _tongTien;
        public decimal TongTien
        {
            get { return _tongTien; }
            set { _tongTien = value; OnPropertyChanged(nameof(TongTien)); }
        }

        private bool _isEditingPhieuMode;
        public bool IsEditingPhieuMode
        {
            get { return _isEditingPhieuMode; }
            set { _isEditingPhieuMode = value; OnPropertyChanged(nameof(IsEditingPhieuMode)); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        private ObservableCollection<Sach> _listSach;
        public ObservableCollection<Sach> ListSach
        {
            get { return _listSach; }
            set { _listSach = value; OnPropertyChanged(nameof(ListSach)); }
        }

        private string _selectedMaSach;
        public string SelectedMaSach
        {
            get { return _selectedMaSach; }
            set { _selectedMaSach = value; OnPropertyChanged(nameof(SelectedMaSach)); }
        }

        private string _soLuong;
        public string SoLuong
        {
            get { return _soLuong; }
            set { _soLuong = value; OnPropertyChanged(nameof(SoLuong)); }
        }

        private string _donGia;
        public string DonGia
        {
            get { return _donGia; }
            set { _donGia = value; OnPropertyChanged(nameof(DonGia)); }
        }

        private ObservableCollection<ChiTietPhieuNhapDTO> _listChiTietPhieu;
        public ObservableCollection<ChiTietPhieuNhapDTO> ListChiTietPhieu
        {
            get { return _listChiTietPhieu; }
            set { _listChiTietPhieu = value; OnPropertyChanged(nameof(ListChiTietPhieu)); }
        }

        private ChiTietPhieuNhapDTO _selectedChiTiet;
        public ChiTietPhieuNhapDTO SelectedChiTiet
        {
            get { return _selectedChiTiet; }
            set { _selectedChiTiet = value; OnPropertyChanged(nameof(SelectedChiTiet)); }
        }

        private string _chiTietStatusMessage;
        public string ChiTietStatusMessage
        {
            get { return _chiTietStatusMessage; }
            set { _chiTietStatusMessage = value; OnPropertyChanged(nameof(ChiTietStatusMessage)); }
        }

        public ICommand SearchCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand AddChiTietCommand { get; set; }
        public ICommand DeleteChiTietCommand { get; set; }

        public PhieuNhapViewModel()
        {
            IsEditingPhieuMode = true;
            NgayNhap = DateTime.Now;
            ListChiTietPhieu = new ObservableCollection<ChiTietPhieuNhapDTO>();

            LoadData();

            SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteAlways);
            AddCommand = new RelayCommand(ExecuteAdd, CanExecuteAlways);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecuteAlways);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteAlways);
            ClearCommand = new RelayCommand(ExecuteClear, CanExecuteAlways);
            AddChiTietCommand = new RelayCommand(ExecuteAddChiTiet, CanExecuteAlways);
            DeleteChiTietCommand = new RelayCommand(ExecuteDeleteChiTiet, CanExecuteAlways);
        }

        private bool CanExecuteAlways(object parameter)
        {
            return true;
        }

        private void LoadData()
        {
            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    ListPhieuNhap = new ObservableCollection<PhieuNhap>(context.PhieuNhaps.ToList());
                    ListSach = new ObservableCollection<Sach>(context.Saches.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void LoadChiTietPhieuNhap(string maPhieu)
        {
            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var query = from ct in context.ChiTietPhieuNhaps
                                join s in context.Saches on ct.MaSach equals s.MaSach
                                where ct.MaPhieuNhap == maPhieu
                                select new ChiTietPhieuNhapDTO
                                {
                                    MaPhieuNhap = ct.MaPhieuNhap,
                                    MaSach = ct.MaSach,
                                    TenSach = s.TenSach,
                                    SoLuong = ct.SoLuong ?? 0,
                                    DonGia = ct.DonGia ?? 0
                                };
                    ListChiTietPhieu = new ObservableCollection<ChiTietPhieuNhapDTO>(query.ToList());
                }
            }
            catch (Exception ex)
            {
                ChiTietStatusMessage = ex.Message;
            }
        }

        private void ExecuteSearch(object parameter)
        {
            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var query = context.PhieuNhaps.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    {
                        string keyword = SearchKeyword.ToLower();
                        query = query.Where(p => p.MaPhieuNhap.ToLower().Contains(keyword));
                    }

                    ListPhieuNhap = new ObservableCollection<PhieuNhap>(query.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ExecuteAdd(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaPhieuNhap))
            {
                StatusMessage = "Vui lòng nhập Mã phiếu nhập!";
                return;
            }

            if (GlobalStore.CurrentAccount == null)
            {
                StatusMessage = "Lỗi xác thực. Vui lòng đăng nhập lại!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    if (context.PhieuNhaps.Any(p => p.MaPhieuNhap == MaPhieuNhap))
                    {
                        StatusMessage = "Mã phiếu nhập đã tồn tại!";
                        return;
                    }

                    var newPhieuNhap = new PhieuNhap
                    {
                        MaPhieuNhap = MaPhieuNhap,
                        NgayNhap = NgayNhap ?? DateTime.Now,
                        MaNV = GlobalStore.CurrentAccount.MaNV,
                        TongTien = 0
                    };

                    context.PhieuNhaps.Add(newPhieuNhap);
                    context.SaveChanges();

                    StatusMessage = "Thêm phiếu nhập thành công!";
                    LoadData();
                    SelectedPhieuNhap = ListPhieuNhap.FirstOrDefault(p => p.MaPhieuNhap == MaPhieuNhap);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ExecuteUpdate(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaPhieuNhap))
            {
                StatusMessage = "Vui lòng chọn phiếu nhập để sửa!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var phieu = context.PhieuNhaps.FirstOrDefault(p => p.MaPhieuNhap == MaPhieuNhap);
                    if (phieu != null)
                    {
                        phieu.NgayNhap = NgayNhap;
                        context.SaveChanges();

                        StatusMessage = "Cập nhật thành công!";
                        LoadData();
                        SelectedPhieuNhap = ListPhieuNhap.FirstOrDefault(p => p.MaPhieuNhap == MaPhieuNhap);
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ExecuteDelete(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaPhieuNhap))
            {
                StatusMessage = "Vui lòng chọn phiếu nhập để xóa!";
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu nhập và toàn bộ chi tiết?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        var chiTiets = context.ChiTietPhieuNhaps.Where(c => c.MaPhieuNhap == MaPhieuNhap);
                        context.ChiTietPhieuNhaps.RemoveRange(chiTiets);

                        var phieu = context.PhieuNhaps.FirstOrDefault(p => p.MaPhieuNhap == MaPhieuNhap);
                        if (phieu != null)
                        {
                            context.PhieuNhaps.Remove(phieu);
                        }

                        context.SaveChanges();

                        StatusMessage = "Xóa thành công!";
                        ExecuteClear(null);
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = "Lỗi khi xóa: " + ex.Message;
                }
            }
        }

        private void ExecuteClear(object parameter)
        {
            ClearPhieuNhapForm();
            StatusMessage = string.Empty;
            SearchKeyword = string.Empty;
            LoadData();
        }

        private void ClearPhieuNhapForm()
        {
            MaPhieuNhap = string.Empty;
            NgayNhap = DateTime.Now;
            TongTien = 0;
            IsEditingPhieuMode = true;

            SelectedMaSach = null;
            SoLuong = string.Empty;
            DonGia = string.Empty;
            ChiTietStatusMessage = string.Empty;

            ListChiTietPhieu.Clear();
            _selectedPhieuNhap = null;
            OnPropertyChanged(nameof(SelectedPhieuNhap));
        }

        private void ExecuteAddChiTiet(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaPhieuNhap) || IsEditingPhieuMode)
            {
                ChiTietStatusMessage = "Vui lòng chọn hoặc lưu một Phiếu Nhập trước!";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedMaSach))
            {
                ChiTietStatusMessage = "Vui lòng chọn Sách!";
                return;
            }

            if (!int.TryParse(SoLuong, out int parsedSoLuong) || parsedSoLuong <= 0)
            {
                ChiTietStatusMessage = "Số lượng phải là số nguyên lớn hơn 0!";
                return;
            }

            if (!decimal.TryParse(DonGia, out decimal parsedDonGia) || parsedDonGia < 0)
            {
                ChiTietStatusMessage = "Đơn giá không hợp lệ!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var phieu = context.PhieuNhaps.FirstOrDefault(p => p.MaPhieuNhap == MaPhieuNhap);
                    if (phieu == null)
                    {
                        ChiTietStatusMessage = "Phiếu nhập không tồn tại trong CSDL!";
                        return;
                    }

                    var existingChiTiet = context.ChiTietPhieuNhaps.FirstOrDefault(c => c.MaPhieuNhap == MaPhieuNhap && c.MaSach == SelectedMaSach);
                    if (existingChiTiet != null)
                    {
                        existingChiTiet.SoLuong += parsedSoLuong;
                        existingChiTiet.DonGia = parsedDonGia;
                    }
                    else
                    {
                        var newChiTiet = new ChiTietPhieuNhap
                        {
                            MaPhieuNhap = MaPhieuNhap,
                            MaSach = SelectedMaSach,
                            SoLuong = parsedSoLuong,
                            DonGia = parsedDonGia
                        };
                        context.ChiTietPhieuNhaps.Add(newChiTiet);
                    }

                    context.SaveChanges();
                    UpdateTongTienAndUpdateUI(context, MaPhieuNhap);

                    ChiTietStatusMessage = "Thêm sách vào phiếu thành công!";
                    SelectedMaSach = null;
                    SoLuong = string.Empty;
                    DonGia = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ChiTietStatusMessage = ex.Message;
            }
        }

        private void ExecuteDeleteChiTiet(object parameter)
        {
            if (SelectedChiTiet == null)
            {
                ChiTietStatusMessage = "Vui lòng chọn chi tiết sách cần xóa!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var chiTiet = context.ChiTietPhieuNhaps.FirstOrDefault(c => c.MaPhieuNhap == SelectedChiTiet.MaPhieuNhap && c.MaSach == SelectedChiTiet.MaSach);
                    if (chiTiet != null)
                    {
                        context.ChiTietPhieuNhaps.Remove(chiTiet);
                        context.SaveChanges();

                        string currentMaPhieu = SelectedChiTiet.MaPhieuNhap;
                        UpdateTongTienAndUpdateUI(context, currentMaPhieu);
                        ChiTietStatusMessage = "Đã xóa sách khỏi phiếu!";
                    }
                }
            }
            catch (Exception ex)
            {
                ChiTietStatusMessage = ex.Message;
            }
        }

        private void UpdateTongTienAndUpdateUI(QuanLyThuVienEntities context, string maPhieu)
        {
            var phieu = context.PhieuNhaps.FirstOrDefault(p => p.MaPhieuNhap == maPhieu);
            if (phieu != null)
            {
                decimal total = context.ChiTietPhieuNhaps
                                      .Where(c => c.MaPhieuNhap == maPhieu)
                                      .Select(c => (c.SoLuong ?? 0) * (c.DonGia ?? 0))
                                      .DefaultIfEmpty(0)
                                      .Sum();
                phieu.TongTien = total;
                context.SaveChanges();

                TongTien = total;

                var phieuInList = ListPhieuNhap.FirstOrDefault(p => p.MaPhieuNhap == maPhieu);
                if (phieuInList != null)
                {
                    int index = ListPhieuNhap.IndexOf(phieuInList);
                    ListPhieuNhap[index] = phieu;
                    SelectedPhieuNhap = phieu;
                }

                LoadChiTietPhieuNhap(maPhieu);
            }
        }
    }
}