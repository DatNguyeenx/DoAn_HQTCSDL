using QuanLyThuVien.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuanLyThuVien.ViewModels
{
    public class PhieuNhapDTO
    {
        public string MaPhieuNhap { get; set; }
        public DateTime? NgayNhap { get; set; }
        public string MaNV { get; set; }
        public decimal? TongTien { get; set; }
    }
    public class ChiTietPhieuNhapDTO
    {
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public int? SoLuong { get; set; }
        public decimal? DonGia { get; set; }
        public decimal? ThanhTien { get; set; }
    }

    class PhieuNhapViewModel : BaseViewModel
    {
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); }
        }

        private ObservableCollection<PhieuNhapDTO> _listPhieuNhap;
        public ObservableCollection<PhieuNhapDTO> ListPhieuNhap
        {
            get { return _listPhieuNhap; }
            set { _listPhieuNhap = value; OnPropertyChanged(nameof(ListPhieuNhap)); }
        }

        private PhieuNhapDTO _selectedPhieuNhap;
        public PhieuNhapDTO SelectedPhieuNhap
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
                    TongTien = _selectedPhieuNhap.TongTien;
                    IsEditingPhieuMode = false;

                    LoadChiTietPhieu();
                }
                else
                {
                    ClearPhieuForm();
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

        private decimal? _tongTien;
        public decimal? TongTien
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

        private int? _soLuong;
        public int? SoLuong
        {
            get { return _soLuong; }
            set { _soLuong = value; OnPropertyChanged(nameof(SoLuong)); }
        }

        private decimal? _donGia;
        public decimal? DonGia
        {
            get { return _donGia; }
            set { _donGia = value; OnPropertyChanged(nameof(DonGia)); }
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
            LoadData();

            SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteAlways);
            AddCommand = new RelayCommand(ExecuteAddPhieu, CanExecuteAlways);
            UpdateCommand = new RelayCommand(ExecuteUpdatePhieu, CanExecuteAlways);
            DeleteCommand = new RelayCommand(ExecuteDeletePhieu, CanExecuteAlways);
            ClearCommand = new RelayCommand(ExecuteClearPhieu, CanExecuteAlways);

            AddChiTietCommand = new RelayCommand(ExecuteAddChiTiet, CanExecuteAlways);
            DeleteChiTietCommand = new RelayCommand(ExecuteDeleteChiTiet, CanExecuteAlways);
        }

        private bool CanExecuteAlways(object parameter) => true;

        private void LoadData()
        {
            try
            {
                using (var context = new QuanLyThuVienEntities())
                { 
                    var queryPhieu = context.PhieuNhaps.Select(p => new PhieuNhapDTO
                    {
                        MaPhieuNhap = p.MaPhieuNhap, 
                        NgayNhap = p.NgayNhap,
                        MaNV = p.MaNV,
                        TongTien = p.TongTien
                    }).ToList();
                    ListPhieuNhap = new ObservableCollection<PhieuNhapDTO>(queryPhieu);

                
                    ListSach = new ObservableCollection<Sach>(context.Saches.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tải dữ liệu: " + ex.Message;
            }
        }

        private void LoadChiTietPhieu()
        {
            if (string.IsNullOrWhiteSpace(MaPhieuNhap))
            {
                ListChiTietPhieu = new ObservableCollection<ChiTietPhieuNhapDTO>();
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var queryChiTiet = (from ct in context.ChiTietPhieuNhaps
                                        join s in context.Saches on ct.MaSach equals s.MaSach
                                        where ct.MaPhieuNhap == MaPhieuNhap 
                                        select new ChiTietPhieuNhapDTO
                                        {
                                            MaSach = ct.MaSach,
                                            TenSach = s.TenSach,
                                            SoLuong = ct.SoLuong,
                                            DonGia = ct.DonGia,
                                            ThanhTien = ct.SoLuong * ct.DonGia
                                        }).ToList();

                    ListChiTietPhieu = new ObservableCollection<ChiTietPhieuNhapDTO>(queryChiTiet);
                }
            }
            catch (Exception ex)
            {
                ChiTietStatusMessage = "Lỗi tải chi tiết: " + ex.Message;
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

                    var result = query.Select(p => new PhieuNhapDTO
                    {
                        MaPhieuNhap = p.MaPhieuNhap,
                        NgayNhap = p.NgayNhap,
                        MaNV = p.MaNV,
                        TongTien = p.TongTien
                    }).ToList();

                    ListPhieuNhap = new ObservableCollection<PhieuNhapDTO>(result);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tìm kiếm: " + ex.Message;
            }
        }

        private void ExecuteAddPhieu(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaPhieuNhap))
            {
                StatusMessage = "Vui lòng nhập Mã phiếu nhập!";
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

                    string currentMaNV = GlobalStore.CurrentAccount != null ? GlobalStore.CurrentAccount.MaNV : "NV001";

                    var newPhieu = new PhieuNhap
                    {
                        MaPhieuNhap = MaPhieuNhap,
                        NgayNhap = NgayNhap ?? DateTime.Now.Date,
                        MaNV = currentMaNV,
                        TongTien = 0
                    };

                    context.PhieuNhaps.Add(newPhieu);
                    context.SaveChanges();

                    StatusMessage = "Thêm phiếu nhập thành công!";
                    LoadData();           
                    SelectedPhieuNhap = ListPhieuNhap.FirstOrDefault(p => p.MaPhieuNhap == MaPhieuNhap);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi: " + ex.Message;
            }
        }

        private void ExecuteUpdatePhieu(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaPhieuNhap) || IsEditingPhieuMode)
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

                        StatusMessage = "Cập nhật ngày nhập thành công!";
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi: " + ex.Message;
            }
        }

        private void ExecuteDeletePhieu(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaPhieuNhap) || IsEditingPhieuMode)
            {
                StatusMessage = "Vui lòng chọn phiếu nhập để xóa!";
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu nhập này cùng với toàn bộ chi tiết sách bên trong?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        var listChiTiet = context.ChiTietPhieuNhaps.Where(c => c.MaPhieuNhap == MaPhieuNhap).ToList();
                        if (listChiTiet.Any())
                        {
                            context.ChiTietPhieuNhaps.RemoveRange(listChiTiet);
                        }

                        var phieu = context.PhieuNhaps.FirstOrDefault(p => p.MaPhieuNhap == MaPhieuNhap);
                        if (phieu != null)
                        {
                            context.PhieuNhaps.Remove(phieu);
                            context.SaveChanges();

                            StatusMessage = "Xóa phiếu nhập thành công!";
                            ExecuteClearPhieu(null);
                        }
                    }
                }
                catch (Exception)
                {
                    StatusMessage = "Lỗi: Dữ liệu đang được sử dụng, không thể xóa!";
                }
            }
        }

        private void ExecuteClearPhieu(object parameter)
        {
            ClearPhieuForm();
            SearchKeyword = string.Empty;
            LoadData();
        }

        private void ClearPhieuForm()
        {
            MaPhieuNhap = string.Empty;
            NgayNhap = DateTime.Now;
            TongTien = 0;
            IsEditingPhieuMode = true;
            StatusMessage = string.Empty;
            ChiTietStatusMessage = string.Empty;

            _selectedPhieuNhap = null;
            OnPropertyChanged(nameof(SelectedPhieuNhap));

            ListChiTietPhieu = new ObservableCollection<ChiTietPhieuNhapDTO>();
            ClearChiTietForm();
        }

        private void ExecuteAddChiTiet(object parameter)
        {
            if (IsEditingPhieuMode || string.IsNullOrWhiteSpace(MaPhieuNhap))
            {
                ChiTietStatusMessage = "Vui lòng chọn hoặc THÊM MỚI phiếu nhập trước khi thêm sách!";
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedMaSach) || SoLuong == null || SoLuong <= 0 || DonGia == null || DonGia < 0)
            {
                ChiTietStatusMessage = "Vui lòng nhập đầy đủ Đầu sách, Số lượng (>0) và Đơn giá!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var chiTiet = context.ChiTietPhieuNhaps.FirstOrDefault(c => c.MaPhieuNhap == MaPhieuNhap && c.MaSach == SelectedMaSach);

                    if (chiTiet != null)
                    {
                        chiTiet.SoLuong += SoLuong;
                        chiTiet.DonGia = DonGia;
                    }
                    else
                    {
                        var newChiTiet = new ChiTietPhieuNhap
                        {
                            MaPhieuNhap = MaPhieuNhap,
                            MaSach = SelectedMaSach,
                            SoLuong = SoLuong,
                            DonGia = DonGia
                        };
                        context.ChiTietPhieuNhaps.Add(newChiTiet);
                    }
                    context.SaveChanges();

                    UpdateTongTienPhieu(context);

                    ChiTietStatusMessage = "Đã thêm sách vào phiếu!";
                    LoadChiTietPhieu();
                    LoadData();      
                }
            }
            catch (Exception ex)
            {
                ChiTietStatusMessage = "Lỗi thêm chi tiết: " + ex.Message;
            }
        }

        private void ExecuteDeleteChiTiet(object parameter)
        {
            if (SelectedChiTiet == null)
            {
                ChiTietStatusMessage = "Vui lòng Click chuột phải vào một dòng sách và chọn 'Xóa khỏi phiếu'!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var chiTiet = context.ChiTietPhieuNhaps.FirstOrDefault(c => c.MaPhieuNhap == MaPhieuNhap && c.MaSach == SelectedChiTiet.MaSach);
                    if (chiTiet != null)
                    {
                        context.ChiTietPhieuNhaps.Remove(chiTiet);
                        context.SaveChanges();

                        UpdateTongTienPhieu(context);

                        ChiTietStatusMessage = "Đã xóa sách khỏi phiếu!";
                        LoadChiTietPhieu();
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                ChiTietStatusMessage = "Lỗi xóa chi tiết: " + ex.Message;
            }
        }

        private void UpdateTongTienPhieu(QuanLyThuVienEntities context)
        {
            var phieu = context.PhieuNhaps.FirstOrDefault(p => p.MaPhieuNhap == MaPhieuNhap);
            if (phieu != null)
            {
                decimal? total = context.ChiTietPhieuNhaps
                                        .Where(c => c.MaPhieuNhap == MaPhieuNhap)
                                        .Sum(c => (decimal?)(c.SoLuong * c.DonGia));

                phieu.TongTien = total ?? 0;
                TongTien = phieu.TongTien; 
                context.SaveChanges();
            }
        }

        private void ClearChiTietForm()
        {
            SelectedMaSach = null;
            SoLuong = null;
            DonGia = null;
        }

    }
}
