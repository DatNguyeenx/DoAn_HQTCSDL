using QuanLyThuVien.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace QuanLyThuVien.ViewModels
{
    public class PhieuMuonTraDTO
    {
        public string MaPhieu { get; set; }
        public string MaBS { get; set; }
        public string MaDG { get; set; }
        public string TenDocGia { get; set; }
        public string TenSach { get; set; }
        public DateTime? HanTra { get; set; }
    }

    class TraSachViewModel : BaseViewModel
    {
        private string _searchKeyword;
        public string SearchKeyword
        {
            get { return _searchKeyword; }
            set { _searchKeyword = value; OnPropertyChanged(nameof(SearchKeyword)); }
        }

        private ObservableCollection<PhieuMuonTraDTO> _listDangMuon;
        public ObservableCollection<PhieuMuonTraDTO> ListDangMuon
        {
            get { return _listDangMuon; }
            set { _listDangMuon = value; OnPropertyChanged(nameof(ListDangMuon)); }
        }

        private PhieuMuonTraDTO _selectedPhieu;
        public PhieuMuonTraDTO SelectedPhieu
        {
            get { return _selectedPhieu; }
            set
            {
                _selectedPhieu = value;
                OnPropertyChanged(nameof(SelectedPhieu));
                if (_selectedPhieu != null)
                {
                    NgayTraThucTe = DateTime.Now.Date;
                }
                else
                {
                    NgayTraThucTe = null;
                    TienPhatDuKien = 0;
                    IsLate = false;
                }
            }
        }

        private DateTime? _ngayTraThucTe;
        public DateTime? NgayTraThucTe
        {
            get { return _ngayTraThucTe; }
            set
            {
                _ngayTraThucTe = value;
                OnPropertyChanged(nameof(NgayTraThucTe));
                CalculateFine();
            }
        }

        private decimal _tienPhatDuKien;
        public decimal TienPhatDuKien
        {
            get { return _tienPhatDuKien; }
            set { _tienPhatDuKien = value; OnPropertyChanged(nameof(TienPhatDuKien)); }
        }

        private bool _isLate;
        public bool IsLate
        {
            get { return _isLate; }
            set { _isLate = value; OnPropertyChanged(nameof(IsLate)); }
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

        public bool HasMessage => !string.IsNullOrEmpty(StatusMessage);

        public ICommand SearchCommand { get; set; }
        public ICommand TraSachCommand { get; set; }
        public ICommand ClearSelectionCommand { get; set; }

        public TraSachViewModel()
        {
            SearchCommand = new RelayCommand(ExecuteSearch, CanExecuteAlways);
            TraSachCommand = new RelayCommand(ExecuteTraSach, CanExecuteAlways);
            ClearSelectionCommand = new RelayCommand(ExecuteClearSelection, CanExecuteAlways);

            LoadData();
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
                    var query = from ct in context.ChiTietPhieuMuons
                                join pm in context.PhieuMuons on ct.MaPhieu equals pm.MaPhieu
                                join dg in context.DocGias on pm.MaDG equals dg.MaDG
                                join bs in context.BanSaoSaches on ct.MaBS equals bs.MaBS
                                join s in context.Saches on bs.MaSach equals s.MaSach
                                where ct.TrangThaiTra == "Đang mượn"
                                select new PhieuMuonTraDTO
                                {
                                    MaPhieu = ct.MaPhieu,
                                    MaBS = ct.MaBS,
                                    MaDG = dg.MaDG,
                                    TenDocGia = dg.HoTen,
                                    TenSach = s.TenSach,
                                    HanTra = pm.HanTra
                                };

                    ListDangMuon = new ObservableCollection<PhieuMuonTraDTO>(query.ToList());
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
                    var query = from ct in context.ChiTietPhieuMuons
                                join pm in context.PhieuMuons on ct.MaPhieu equals pm.MaPhieu
                                join dg in context.DocGias on pm.MaDG equals dg.MaDG
                                join bs in context.BanSaoSaches on ct.MaBS equals bs.MaBS
                                join s in context.Saches on bs.MaSach equals s.MaSach
                                where ct.TrangThaiTra == "Đang mượn"
                                select new PhieuMuonTraDTO
                                {
                                    MaPhieu = ct.MaPhieu,
                                    MaBS = ct.MaBS,
                                    MaDG = dg.MaDG,
                                    TenDocGia = dg.HoTen,
                                    TenSach = s.TenSach,
                                    HanTra = pm.HanTra
                                };

                    if (!string.IsNullOrWhiteSpace(SearchKeyword))
                    {
                        string keyword = SearchKeyword.ToLower();
                        query = query.Where(q => q.MaPhieu.ToLower().Contains(keyword) ||
                                                 q.MaBS.ToLower().Contains(keyword) ||
                                                 q.MaDG.ToLower().Contains(keyword));
                    }

                    ListDangMuon = new ObservableCollection<PhieuMuonTraDTO>(query.ToList());
                }
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
            }
        }

        private void CalculateFine()
        {
            if (SelectedPhieu != null && SelectedPhieu.HanTra.HasValue && NgayTraThucTe.HasValue)
            {
                int delayDays = (NgayTraThucTe.Value.Date - SelectedPhieu.HanTra.Value.Date).Days;
                if (delayDays > 0)
                {
                    IsLate = true;
                    TienPhatDuKien = delayDays * 5000;
                }
                else
                {
                    IsLate = false;
                    TienPhatDuKien = 0;
                }
            }
            else
            {
                IsLate = false;
                TienPhatDuKien = 0;
            }
        }

        private void ExecuteTraSach(object parameter)
        {
            if (SelectedPhieu == null || !NgayTraThucTe.HasValue)
            {
                StatusMessage = "Vui lòng chọn phiếu và chọn ngày trả thực tế.";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    using (var transaction = context.Database.BeginTransaction())
                    {
                        try
                        {
                            var chiTiet = context.ChiTietPhieuMuons.FirstOrDefault(c =>
                                c.MaPhieu == SelectedPhieu.MaPhieu && c.MaBS == SelectedPhieu.MaBS);

                            if (chiTiet != null)
                            {
                                chiTiet.NgayTraThucTe = NgayTraThucTe;
                                chiTiet.TrangThaiTra = "Đã trả";
                            }

                            var banSao = context.BanSaoSaches.FirstOrDefault(b => b.MaBS == SelectedPhieu.MaBS);
                            if (banSao != null)
                            {
                                banSao.TinhTrang = "Sẵn sàng";
                            }

                            if (IsLate && TienPhatDuKien > 0)
                            {
                                int delayDays = (NgayTraThucTe.Value.Date - SelectedPhieu.HanTra.Value.Date).Days;

                                var phieuPhat = new PhieuPhat();
                                phieuPhat.MaPhieu = SelectedPhieu.MaPhieu;
                                phieuPhat.MaBS = SelectedPhieu.MaBS;
                                phieuPhat.LyDo = $"Trễ hạn {delayDays} ngày";

                                var prop = typeof(PhieuPhat).GetProperty("SoTienPhat");
                                if (prop != null)
                                {
                                    Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                    prop.SetValue(phieuPhat, Convert.ChangeType(TienPhatDuKien, targetType));
                                }

                                context.PhieuPhats.Add(phieuPhat);
                            }

                            context.SaveChanges();
                            transaction.Commit();

                            StatusMessage = "Ghi nhận trả sách thành công!";
                            SelectedPhieu = null;
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            StatusMessage = "Lỗi xử lý giao tác: " + ex.Message;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "Lỗi hệ thống: " + ex.Message;
            }
        }

        private void ExecuteClearSelection(object parameter)
        {
            SelectedPhieu = null;
            StatusMessage = string.Empty;
            SearchKeyword = string.Empty;
            LoadData();
        }
    }
}