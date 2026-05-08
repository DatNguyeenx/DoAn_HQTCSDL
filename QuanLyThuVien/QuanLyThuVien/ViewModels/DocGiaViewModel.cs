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
    class DocGiaViewModel : BaseViewModel
    {
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); }
        }

        private ComboBoxItem _filterTrangThai;
        public ComboBoxItem FilterTrangThai
        {
            get { return _filterTrangThai; }
            set { _filterTrangThai = value; OnPropertyChanged(nameof(FilterTrangThai)); }
        }

        private ObservableCollection<DocGia> _listDocGia;
        public ObservableCollection<DocGia> ListDocGia
        {
            get { return _listDocGia; }
            set { _listDocGia = value; OnPropertyChanged(nameof(ListDocGia)); }
        }

        private DocGia _selectedDocGia;
        public DocGia SelectedDocGia
        {
            get { return _selectedDocGia; }
            set
            {
                if (_selectedDocGia == value) return;

                _selectedDocGia = value;
                OnPropertyChanged(nameof(SelectedDocGia));

                if (_selectedDocGia != null)
                {
                    MaDG = _selectedDocGia.MaDG;
                    HoTen = _selectedDocGia.HoTen;
                    NgaySinh = _selectedDocGia.NgaySinh;
                    SoDT = _selectedDocGia.SoDT;
                    DiaChi = _selectedDocGia.DiaChi;
                    NgayLapThe = _selectedDocGia.NgayLapThe;
                    NgayHetHan = _selectedDocGia.NgayHetHan;
                    TrangThaiThe = _selectedDocGia.TrangThaiThe;
                    IsEditingMode = false;
                }
                else
                {
                    ClearForm();
                }
            }
        }

        private string _maDG;
        public string MaDG
        {
            get { return _maDG; }
            set { _maDG = value; OnPropertyChanged(nameof(MaDG)); }
        }

        private string _hoTen;
        public string HoTen
        {
            get { return _hoTen; }
            set { _hoTen = value; OnPropertyChanged(nameof(HoTen)); }
        }

        private DateTime? _ngaySinh;
        public DateTime? NgaySinh
        {
            get { return _ngaySinh; }
            set { _ngaySinh = value; OnPropertyChanged(nameof(NgaySinh)); }
        }

        private string _soDT;
        public string SoDT
        {
            get { return _soDT; }
            set { _soDT = value; OnPropertyChanged(nameof(SoDT)); }
        }

        private string _diaChi;
        public string DiaChi
        {
            get { return _diaChi; }
            set { _diaChi = value; OnPropertyChanged(nameof(DiaChi)); }
        }

        private DateTime? _ngayLapThe;
        public DateTime? NgayLapThe
        {
            get { return _ngayLapThe; }
            set { _ngayLapThe = value; OnPropertyChanged(nameof(NgayLapThe)); }
        }

        private DateTime? _ngayHetHan;
        public DateTime? NgayHetHan
        {
            get { return _ngayHetHan; }
            set { _ngayHetHan = value; OnPropertyChanged(nameof(NgayHetHan)); }
        }

        private string _trangThaiThe;
        public string TrangThaiThe
        {
            get { return _trangThaiThe; }
            set { _trangThaiThe = value; OnPropertyChanged(nameof(TrangThaiThe)); }
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

        public DocGiaViewModel()
        {
            IsEditingMode = true;
            LoadData();

            SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteAlways);
            AddCommand = new RelayCommand(ExecuteAdd, CanExecuteAlways);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecuteAlways);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteAlways);
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
                    ListDocGia = new ObservableCollection<DocGia>(context.DocGias.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tải dữ liệu: " + ex.Message;
            }
        }

        private void ExecuteSearch(object parameter)
        {
            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var query = context.DocGias.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    {
                        string keyword = SearchKeyword.ToLower();
                        query = query.Where(d => d.MaDG.ToLower().Contains(keyword) ||
                                                 d.HoTen.ToLower().Contains(keyword) ||
                                                 d.SoDT.Contains(keyword));
                    }

                    if (FilterTrangThai != null && FilterTrangThai.Content.ToString() != "Tất cả")
                    {
                        string status = FilterTrangThai.Content.ToString();
                        query = query.Where(d => d.TrangThaiThe == status);
                    }

                    ListDocGia = new ObservableCollection<DocGia>(query.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tìm kiếm: " + ex.Message;
            }
        }

        private void ExecuteAdd(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaDG) || string.IsNullOrWhiteSpace(HoTen))
            {
                StatusMessage = "Vui lòng nhập Mã độc giả và Họ tên!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    if (context.DocGias.Any(d => d.MaDG == MaDG))
                    {
                        StatusMessage = "Mã độc giả đã tồn tại!";
                        return;
                    }

                    var newDocGia = new DocGia
                    {
                        MaDG = MaDG,
                        HoTen = HoTen,
                        NgaySinh = NgaySinh,
                        SoDT = SoDT,
                        DiaChi = DiaChi,
                        NgayLapThe = NgayLapThe ?? DateTime.Now,
                        NgayHetHan = NgayHetHan ?? DateTime.Now.AddYears(1),
                        TrangThaiThe = string.IsNullOrWhiteSpace(TrangThaiThe) ? "Bình thường" : TrangThaiThe
                    };

                    context.DocGias.Add(newDocGia);
                    context.SaveChanges();

                    StatusMessage = "Thêm mới thành công!";
                    ClearForm();
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi thêm mới: " + ex.Message;
            }
        }

        private void ExecuteUpdate(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaDG) || string.IsNullOrWhiteSpace(HoTen))
            {
                StatusMessage = "Vui lòng chọn độc giả và nhập Họ tên!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var docGia = context.DocGias.FirstOrDefault(d => d.MaDG == MaDG);
                    if (docGia != null)
                    {
                        docGia.HoTen = HoTen;
                        docGia.NgaySinh = NgaySinh;
                        docGia.SoDT = SoDT;
                        docGia.DiaChi = DiaChi;
                        docGia.NgayLapThe = NgayLapThe;
                        docGia.NgayHetHan = NgayHetHan;
                        docGia.TrangThaiThe = string.IsNullOrWhiteSpace(TrangThaiThe) ? "Bình thường" : TrangThaiThe;

                        context.SaveChanges();

                        StatusMessage = "Cập nhật thành công!";
                        ClearForm();
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi cập nhật: " + ex.Message;
            }
        }

        private void ExecuteDelete(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaDG))
            {
                StatusMessage = "Vui lòng chọn độc giả để xóa!";
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa độc giả này?", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        var docGia = context.DocGias.FirstOrDefault(d => d.MaDG == MaDG);
                        if (docGia != null)
                        {
                            context.DocGias.Remove(docGia);
                            context.SaveChanges();

                            StatusMessage = "Xóa thành công!";
                            ClearForm();
                            LoadData();
                        }
                    }
                }
                catch (Exception)
                {
                    StatusMessage = "Không thể xóa. Độc giả này đang có dữ liệu phiếu mượn/phạt!";
                }
            }
        }

        private void ClearForm()
        {
            MaDG = string.Empty;
            HoTen = string.Empty;
            NgaySinh = null;
            SoDT = string.Empty;
            DiaChi = string.Empty;
            NgayLapThe = null;
            NgayHetHan = null;
            TrangThaiThe = null;
            IsEditingMode = true;

            _selectedDocGia = null;
            OnPropertyChanged(nameof(SelectedDocGia));
        }
    }
}