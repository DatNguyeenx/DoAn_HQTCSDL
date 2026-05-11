using QuanLyThuVien.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyThuVien.ViewModels
{
    class NhanVienViewModel : BaseViewModel
    {
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); }
        }

        private ComboBoxItem _filterChucVu;
        public ComboBoxItem FilterChucVu
        {
            get { return _filterChucVu; }
            set { _filterChucVu = value; OnPropertyChanged(nameof(FilterChucVu)); }
        }

        private ObservableCollection<NhanVien> _listNhanVien;
        public ObservableCollection<NhanVien> ListNhanVien
        {
            get { return _listNhanVien; }
            set { _listNhanVien = value; OnPropertyChanged(nameof(ListNhanVien)); }
        }

        private NhanVien _selectedNhanVien;
        public NhanVien SelectedNhanVien
        {
            get { return _selectedNhanVien; }
            set
            {
                if (_selectedNhanVien == value) return;

                _selectedNhanVien = value;
                OnPropertyChanged(nameof(SelectedNhanVien));

                if (_selectedNhanVien != null)
                {
                    MaNV = _selectedNhanVien.MaNV;
                    HoTen = _selectedNhanVien.HoTen;
                    TenDN = _selectedNhanVien.TenDN;
                    MatKhau = _selectedNhanVien.MatKhau;
                    ChucVu = _selectedNhanVien.ChucVu;
                    IsEditingMode = false;
                }
                else
                {
                    ClearForm();
                }
            }
        }

        private string _maNV;
        public string MaNV
        {
            get { return _maNV; }
            set { _maNV = value; OnPropertyChanged(nameof(MaNV)); }
        }

        private string _hoTen;
        public string HoTen
        {
            get { return _hoTen; }
            set { _hoTen = value; OnPropertyChanged(nameof(HoTen)); }
        }

        private string _tenDN;
        public string TenDN
        {
            get { return _tenDN; }
            set { _tenDN = value; OnPropertyChanged(nameof(TenDN)); }
        }

        private string _matKhau;
        public string MatKhau
        {
            get { return _matKhau; }
            set { _matKhau = value; OnPropertyChanged(nameof(MatKhau)); }
        }

        private string _chucVu;
        public string ChucVu
        {
            get { return _chucVu; }
            set { _chucVu = value; OnPropertyChanged(nameof(ChucVu)); }
        }

        private bool _isEditingMode;
        public bool IsEditingMode
        {
            get { return _isEditingMode; }
            set { _isEditingMode = value; OnPropertyChanged(nameof(IsEditingMode)); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; OnPropertyChanged(nameof(StatusMessage)); }
        }

        public ICommand SearchCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public NhanVienViewModel()
        {
            IsEditingMode = true;
            LoadData();

            SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteAlways);
            AddCommand = new RelayCommand(ExecuteAdd, CanExecuteAlways);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecuteAlways);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteAlways);
            ClearCommand = new RelayCommand(ExecuteClear, CanExecuteAlways);
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
                    ListNhanVien = new ObservableCollection<NhanVien>(context.NhanViens.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ExecuteSearch(object parameter)
        {
            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var query = context.NhanViens.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    {
                        string keyword = SearchKeyword.ToLower();
                        query = query.Where(n => n.MaNV.ToLower().Contains(keyword) ||
                                                 n.HoTen.ToLower().Contains(keyword) ||
                                                 n.TenDN.ToLower().Contains(keyword));
                    }

                    if (FilterChucVu != null && FilterChucVu.Content.ToString() != "Tất cả")
                    {
                        string status = FilterChucVu.Content.ToString();
                        query = query.Where(n => n.ChucVu == status);
                    }

                    ListNhanVien = new ObservableCollection<NhanVien>(query.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ExecuteAdd(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaNV) || string.IsNullOrWhiteSpace(HoTen) ||
                string.IsNullOrWhiteSpace(TenDN) || string.IsNullOrWhiteSpace(MatKhau) || string.IsNullOrWhiteSpace(ChucVu))
            {
                StatusMessage = "Vui lòng nhập đầy đủ thông tin!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    if (context.NhanViens.Any(n => n.MaNV == MaNV))
                    {
                        StatusMessage = "Mã nhân viên đã tồn tại!";
                        return;
                    }

                    if (context.NhanViens.Any(n => n.TenDN == TenDN))
                    {
                        StatusMessage = "Tên đăng nhập đã tồn tại!";
                        return;
                    }

                    var newNhanVien = new NhanVien
                    {
                        MaNV = MaNV,
                        HoTen = HoTen,
                        TenDN = TenDN,
                        MatKhau = MatKhau,
                        ChucVu = ChucVu
                    };

                    context.NhanViens.Add(newNhanVien);
                    context.SaveChanges();

                    StatusMessage = "Thêm mới thành công!";
                    ClearForm();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ExecuteUpdate(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaNV))
            {
                StatusMessage = "Vui lòng chọn nhân viên để sửa!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var nhanVien = context.NhanViens.FirstOrDefault(n => n.MaNV == MaNV);
                    if (nhanVien != null)
                    {
                        if (nhanVien.TenDN != TenDN && context.NhanViens.Any(n => n.TenDN == TenDN))
                        {
                            StatusMessage = "Tên đăng nhập đã tồn tại!";
                            return;
                        }

                        nhanVien.HoTen = HoTen;
                        nhanVien.TenDN = TenDN;
                        nhanVien.MatKhau = MatKhau;
                        nhanVien.ChucVu = ChucVu;

                        context.SaveChanges();

                        StatusMessage = "Cập nhật thành công!";
                        ClearForm();
                        LoadData();
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
            if (string.IsNullOrWhiteSpace(MaNV))
            {
                StatusMessage = "Vui lòng chọn nhân viên để xóa!";
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        var nhanVien = context.NhanViens.FirstOrDefault(n => n.MaNV == MaNV);
                        if (nhanVien != null)
                        {
                            context.NhanViens.Remove(nhanVien);
                            context.SaveChanges();

                            StatusMessage = "Xóa thành công!";
                            ClearForm();
                            LoadData();
                        }
                    }
                }
                catch (Exception)
                {
                    StatusMessage = "Dữ liệu đang được sử dụng, không thể xóa!";
                }
            }
        }

        private void ClearForm()
        {
            MaNV = string.Empty;
            HoTen = string.Empty;
            TenDN = string.Empty;
            MatKhau = string.Empty;
            ChucVu = null;
            IsEditingMode = true;

            _selectedNhanVien = null;
            OnPropertyChanged(nameof(SelectedNhanVien));
        }

        private void ExecuteClear(object parameter)
        {
            ClearForm();
            StatusMessage = string.Empty;
            SearchKeyword = string.Empty;
            LoadData();
        }
    }
}
