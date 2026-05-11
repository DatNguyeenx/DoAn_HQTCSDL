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
    class SachViewModel : BaseViewModel
    {
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); }
        }

        private ObservableCollection<Sach> _listSach;
        public ObservableCollection<Sach> ListSach
        {
            get { return _listSach; }
            set { _listSach = value; OnPropertyChanged(nameof(ListSach)); }
        }

        private ObservableCollection<NhaXuatBan> _listNXB;
        public ObservableCollection<NhaXuatBan> ListNXB
        {
            get { return _listNXB; }
            set { _listNXB = value; OnPropertyChanged(nameof(ListNXB)); }
        }

        private Sach _selectedSach;
        public Sach SelectedSach
        {
            get { return _selectedSach; }
            set
            {
                if (_selectedSach == value) return;

                _selectedSach = value;
                OnPropertyChanged(nameof(SelectedSach));

                if (_selectedSach != null)
                {
                    MaSach = _selectedSach.MaSach;
                    TenSach = _selectedSach.TenSach;
                    SelectedMaNXB = _selectedSach.MaNXB;
                    NamXB = _selectedSach.NamXB?.ToString();
                    IsEditingMode = false;
                }
                else
                {
                    ClearForm();
                }
            }
        }

        private string _maSach;
        public string MaSach
        {
            get { return _maSach; }
            set { _maSach = value; OnPropertyChanged(nameof(MaSach)); }
        }

        private string _tenSach;
        public string TenSach
        {
            get { return _tenSach; }
            set { _tenSach = value; OnPropertyChanged(nameof(TenSach)); }
        }

        private string _selectedMaNXB;
        public string SelectedMaNXB
        {
            get { return _selectedMaNXB; }
            set { _selectedMaNXB = value; OnPropertyChanged(nameof(SelectedMaNXB)); }
        }

        private string _namXB;
        public string NamXB
        {
            get { return _namXB; }
            set { _namXB = value; OnPropertyChanged(nameof(NamXB)); }
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

        public SachViewModel()
        {
            IsEditingMode = true;
            LoadData();

            SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteAlways);
            AddCommand = new RelayCommand(ExecuteAdd, CanExecuteAlways);
            UpdateCommand = new RelayCommand(ExecuteUpdate, CanExecuteAlways);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteAlways);
            ClearCommand = new RelayCommand(ExecuteClear,CanExecuteAlways);
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
                    ListNXB = new ObservableCollection<NhaXuatBan>(context.NhaXuatBans.ToList());
                    var query = context.Saches.Include("NhaXuatBan").ToList();
                    ListSach = new ObservableCollection<Sach>(query);
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
                    var query = context.Saches.Include("NhaXuatBan").AsQueryable();

                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    {
                        string keyword = SearchKeyword.ToLower();
                        query = query.Where(s => s.MaSach.ToLower().Contains(keyword) || s.TenSach.ToLower().Contains(keyword));
                    }

                    ListSach = new ObservableCollection<Sach>(query.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ExecuteAdd(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaSach) || string.IsNullOrWhiteSpace(TenSach) || string.IsNullOrWhiteSpace(SelectedMaNXB))
            {
                StatusMessage = "Vui lòng nhập đầy đủ thông tin!";
                return;
            }

            int? namXuatBan = null;
            if (!string.IsNullOrWhiteSpace(NamXB))
            {
                if (int.TryParse(NamXB, out int parsedNam))
                {
                    namXuatBan = parsedNam;
                }
                else
                {
                    StatusMessage = "Năm xuất bản phải là số nguyên!";
                    return;
                }
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    if (context.Saches.Any(s => s.MaSach == MaSach))
                    {
                        StatusMessage = "Mã sách đã tồn tại!";
                        return;
                    }

                    var newSach = new Sach
                    {
                        MaSach = MaSach,
                        TenSach = TenSach,
                        MaNXB = SelectedMaNXB,
                        NamXB = namXuatBan
                    };

                    context.Saches.Add(newSach);
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
            if (string.IsNullOrWhiteSpace(MaSach))
            {
                StatusMessage = "Vui lòng chọn sách để sửa!";
                return;
            }

            int? namXuatBan = null;
            if (!string.IsNullOrWhiteSpace(NamXB))
            {
                if (int.TryParse(NamXB, out int parsedNam))
                {
                    namXuatBan = parsedNam;
                }
                else
                {
                    StatusMessage = "Năm xuất bản phải là số nguyên!";
                    return;
                }
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var sach = context.Saches.FirstOrDefault(s => s.MaSach == MaSach);
                    if (sach != null)
                    {
                        sach.TenSach = TenSach;
                        sach.MaNXB = SelectedMaNXB;
                        sach.NamXB = namXuatBan;

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
            if (string.IsNullOrWhiteSpace(MaSach))
            {
                StatusMessage = "Vui lòng chọn sách để xóa!";
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa sách này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        var sach = context.Saches.FirstOrDefault(s => s.MaSach == MaSach);
                        if (sach != null)
                        {
                            context.Saches.Remove(sach);
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
            MaSach = string.Empty;
            TenSach = string.Empty;
            SelectedMaNXB = null;
            NamXB = string.Empty;
            IsEditingMode = true;

            _selectedSach = null;
            OnPropertyChanged(nameof(SelectedSach));
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
