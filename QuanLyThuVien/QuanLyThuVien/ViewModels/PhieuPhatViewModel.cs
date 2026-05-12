using QuanLyThuVien.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyThuVien.ViewModels
{
    class PhieuPhatViewModel : BaseViewModel
    {
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); }
        }

        private ObservableCollection<PhieuPhat> _listPhieuPhat;
        public ObservableCollection<PhieuPhat> ListPhieuPhat
        {
            get { return _listPhieuPhat; }
            set { _listPhieuPhat = value; OnPropertyChanged(nameof(ListPhieuPhat)); }
        }

        private PhieuPhat _selectedPhieuPhat;
        public PhieuPhat SelectedPhieuPhat
        {
            get { return _selectedPhieuPhat; }
            set
            {
                if (_selectedPhieuPhat == value) return;
                _selectedPhieuPhat = value;
                OnPropertyChanged(nameof(SelectedPhieuPhat));

                if (_selectedPhieuPhat != null)
                {
                    MaPhieuPhat = _selectedPhieuPhat.MaPhieuPhat.ToString();
                    MaPhieu = _selectedPhieuPhat.MaPhieu;
                    MaBS = _selectedPhieuPhat.MaBS;
                    SoTienPhat = _selectedPhieuPhat.SoTienPhat?.ToString("G0");
                    LyDo = _selectedPhieuPhat.LyDo;
                    IsEditingMode = false;
                }
                else
                {
                    ClearForm();
                }
            }
        }

        private string _maPhieuPhat;
        public string MaPhieuPhat
        {
            get { return _maPhieuPhat; }
            set { _maPhieuPhat = value; OnPropertyChanged(nameof(MaPhieuPhat)); }
        }

        private string _maPhieu;
        public string MaPhieu
        {
            get { return _maPhieu; }
            set { _maPhieu = value; OnPropertyChanged(nameof(MaPhieu)); }
        }

        private string _maBS;
        public string MaBS
        {
            get { return _maBS; }
            set { _maBS = value; OnPropertyChanged(nameof(MaBS)); }
        }

        private string _soTienPhat;
        public string SoTienPhat
        {
            get { return _soTienPhat; }
            set { _soTienPhat = value; OnPropertyChanged(nameof(SoTienPhat)); }
        }

        private string _lyDo;
        public string LyDo
        {
            get { return _lyDo; }
            set { _lyDo = value; OnPropertyChanged(nameof(LyDo)); }
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

        public PhieuPhatViewModel()
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
                    ListPhieuPhat = new ObservableCollection<PhieuPhat>(context.PhieuPhats.ToList());
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
                    var query = context.PhieuPhats.AsQueryable();

                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    {
                        string keyword = SearchKeyword.ToLower();
                        query = query.Where(p => p.MaPhieuPhat.ToString().Contains(keyword) ||
                                                 p.MaPhieu.ToLower().Contains(keyword) ||
                                                 p.MaBS.ToLower().Contains(keyword));
                    }

                    ListPhieuPhat = new ObservableCollection<PhieuPhat>(query.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi tìm kiếm: " + ex.Message;
            }
        }

        private void ExecuteAdd(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaPhieu) || string.IsNullOrWhiteSpace(MaBS) || string.IsNullOrWhiteSpace(SoTienPhat))
            {
                StatusMessage = "Vui lòng nhập Mã phiếu, Mã bản sao và Số tiền phạt!";
                return;
            }

            if (!decimal.TryParse(SoTienPhat, out decimal parsedTienPhat) || parsedTienPhat < 0)
            {
                StatusMessage = "Số tiền phạt không hợp lệ!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    bool phieuMuonExists = context.ChiTietPhieuMuons.Any(ct => ct.MaPhieu == MaPhieu && ct.MaBS == MaBS);
                    if (!phieuMuonExists)
                    {
                        StatusMessage = "Không tìm thấy chi tiết phiếu mượn với Mã phiếu và Mã bản sao này!";
                        return;
                    }

                    var newPhieuPhat = new PhieuPhat
                    {
                        MaPhieu = MaPhieu,
                        MaBS = MaBS,
                        SoTienPhat = parsedTienPhat,
                        LyDo = LyDo
                    };

                    context.PhieuPhats.Add(newPhieuPhat);
                    context.SaveChanges();

                    StatusMessage = "Thêm phiếu phạt thành công!";
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
            if (string.IsNullOrWhiteSpace(MaPhieuPhat))
            {
                StatusMessage = "Vui lòng chọn phiếu phạt để sửa!";
                return;
            }

            if (!decimal.TryParse(SoTienPhat, out decimal parsedTienPhat) || parsedTienPhat < 0)
            {
                StatusMessage = "Số tiền phạt không hợp lệ!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    if (int.TryParse(MaPhieuPhat, out int parsedMaPhieuPhat))
                    {
                        var phieuPhat = context.PhieuPhats.FirstOrDefault(p => p.MaPhieuPhat == parsedMaPhieuPhat);
                        if (phieuPhat != null)
                        {
                            phieuPhat.SoTienPhat = parsedTienPhat;
                            phieuPhat.LyDo = LyDo;

                            context.SaveChanges();

                            StatusMessage = "Cập nhật thành công!";
                            ClearForm();
                            LoadData();
                        }
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
            if (string.IsNullOrWhiteSpace(MaPhieuPhat))
            {
                StatusMessage = "Vui lòng chọn phiếu phạt để xóa!";
                return;
            }

            MessageBoxResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu phạt này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        if (int.TryParse(MaPhieuPhat, out int parsedMaPhieuPhat))
                        {
                            var phieuPhat = context.PhieuPhats.FirstOrDefault(p => p.MaPhieuPhat == parsedMaPhieuPhat);
                            if (phieuPhat != null)
                            {
                                context.PhieuPhats.Remove(phieuPhat);
                                context.SaveChanges();

                                StatusMessage = "Xóa thành công!";
                                ClearForm();
                                LoadData();
                            }
                        }
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
            ClearForm();
            StatusMessage = string.Empty;
            SearchKeyword = string.Empty;
            LoadData();
        }

        private void ClearForm()
        {
            MaPhieuPhat = string.Empty;
            MaPhieu = string.Empty;
            MaBS = string.Empty;
            SoTienPhat = string.Empty;
            LyDo = string.Empty;
            IsEditingMode = true;

            _selectedPhieuPhat = null;
            OnPropertyChanged(nameof(SelectedPhieuPhat));
        }
    }
}