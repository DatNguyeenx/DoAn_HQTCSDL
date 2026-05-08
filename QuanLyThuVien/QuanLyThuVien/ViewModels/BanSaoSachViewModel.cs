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
    public class BanSaoSachDTO
    {
        public string MaBS { get; set; }
        public string MaSach { get; set; }
        public string TenSach { get; set; }
        public string TinhTrang { get; set; }
    }

    class BanSaoSachViewModel : BaseViewModel
    {
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); }
        }

        private ComboBoxItem _filterTinhTrang;
        public ComboBoxItem FilterTinhTrang
        {
            get { return _filterTinhTrang; }
            set { _filterTinhTrang = value; OnPropertyChanged(nameof(FilterTinhTrang)); }
        }

        private ObservableCollection<BanSaoSachDTO> _listBanSao;
        public ObservableCollection<BanSaoSachDTO> ListBanSao
        {
            get { return _listBanSao; }
            set { _listBanSao = value; OnPropertyChanged(nameof(ListBanSao)); }
        }

        private ObservableCollection<Sach> _listSach;
        public ObservableCollection<Sach> ListSach
        {
            get { return _listSach; }
            set { _listSach = value; OnPropertyChanged(nameof(ListSach)); }
        }

        private BanSaoSachDTO _selectedBanSao;
        public BanSaoSachDTO SelectedBanSao
        {
            get { return _selectedBanSao; }
            set
            {
                if (_selectedBanSao == value) return;

                _selectedBanSao = value;
                OnPropertyChanged(nameof(SelectedBanSao));

                if (_selectedBanSao != null)
                {
                    MaBS = _selectedBanSao.MaBS;
                    SelectedMaSach = _selectedBanSao.MaSach;
                    TinhTrang = _selectedBanSao.TinhTrang;
                    IsEditingMode = false;
                }
                else
                {
                    ClearForm();
                }
            }
        }

        private string _maBS;
        public string MaBS
        {
            get { return _maBS; }
            set { _maBS = value; OnPropertyChanged(nameof(MaBS)); }
        }

        private string _selectedMaSach;
        public string SelectedMaSach
        {
            get { return _selectedMaSach; }
            set { _selectedMaSach = value; OnPropertyChanged(nameof(SelectedMaSach)); }
        }

        private string _tinhTrang;
        public string TinhTrang
        {
            get { return _tinhTrang; }
            set { _tinhTrang = value; OnPropertyChanged(nameof(TinhTrang)); }
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

        public BanSaoSachViewModel()
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
                    ListSach = new ObservableCollection<Sach>(context.Saches.ToList());

                    var query = context.BanSaoSaches.Select(b => new BanSaoSachDTO
                    {
                        MaBS = b.MaBS,
                        MaSach = b.MaSach,
                        TenSach = b.Sach.TenSach,
                        TinhTrang = b.TinhTrang
                    }).ToList();

                    ListBanSao = new ObservableCollection<BanSaoSachDTO>(query);
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
                    var query = context.BanSaoSaches.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    {
                        string keyword = SearchKeyword.ToLower();
                        query = query.Where(b => b.MaBS.ToLower().Contains(keyword) || b.Sach.TenSach.ToLower().Contains(keyword));
                    }

                    if (FilterTinhTrang != null && FilterTinhTrang.Content.ToString() != "Tất cả")
                    {
                        string status = FilterTinhTrang.Content.ToString();
                        query = query.Where(b => b.TinhTrang == status);
                    }

                    var result = query.Select(b => new BanSaoSachDTO
                    {
                        MaBS = b.MaBS,
                        MaSach = b.MaSach,
                        TenSach = b.Sach.TenSach,
                        TinhTrang = b.TinhTrang
                    }).ToList();

                    ListBanSao = new ObservableCollection<BanSaoSachDTO>(result);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void ExecuteAdd(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaBS) || string.IsNullOrWhiteSpace(SelectedMaSach) || string.IsNullOrWhiteSpace(TinhTrang))
            {
                StatusMessage = "Vui lòng nhập đầy đủ thông tin!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var exists = context.BanSaoSaches.Any(b => b.MaBS == MaBS);
                    if (exists)
                    {
                        StatusMessage = "Mã bản sao đã tồn tại!";
                        return;
                    }

                    var newBanSao = new BanSaoSach
                    {
                        MaBS = MaBS,
                        MaSach = SelectedMaSach,
                        TinhTrang = TinhTrang
                    };

                    context.BanSaoSaches.Add(newBanSao);
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
            if (string.IsNullOrWhiteSpace(MaBS))
            {
                StatusMessage = "Vui lòng chọn bản sao để sửa!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var bs = context.BanSaoSaches.FirstOrDefault(b => b.MaBS == MaBS);
                    if (bs != null)
                    {
                        bs.MaSach = SelectedMaSach;
                        bs.TinhTrang = TinhTrang;
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
            if (string.IsNullOrWhiteSpace(MaBS))
            {
                StatusMessage = "Vui lòng chọn bản sao để xóa!";
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa bản sao sách này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        var bs = context.BanSaoSaches.FirstOrDefault(b => b.MaBS == MaBS);
                        if (bs != null)
                        {
                            context.BanSaoSaches.Remove(bs);
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

        private void ExecuteClear(object parameter)
        {
            ClearForm();
            StatusMessage = string.Empty;
            SearchKeyword = string.Empty;
            LoadData();
        }

        private void ClearForm()
        {
            MaBS = string.Empty;
            SelectedMaSach = null;
            TinhTrang = null;
            IsEditingMode = true;

            _selectedBanSao = null;
            OnPropertyChanged(nameof(SelectedBanSao));
        }
    }
}
