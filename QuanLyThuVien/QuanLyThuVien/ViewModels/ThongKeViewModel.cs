using QuanLyThuVien.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyThuVien.ViewModels
{
    public class NoQuaHanDTO
    {
        public string HoTen { get; set; }
        public string TenSach { get; set; }
    }

    class ThongKeViewModel : BaseViewModel
    {
        private int _tongDauSach;
        public int TongDauSach
        {
            get { return _tongDauSach; }
            set { _tongDauSach = value; OnPropertyChanged(nameof(TongDauSach)); }
        }

        private int _tongSanSang;
        public int TongSanSang
        {
            get { return _tongSanSang; }
            set { _tongSanSang = value; OnPropertyChanged(nameof(TongSanSang)); }
        }

        private int _tongDangMuon;
        public int TongDangMuon
        {
            get { return _tongDangMuon; }
            set { _tongDangMuon = value; OnPropertyChanged(nameof(TongDangMuon)); }
        }

        private int _tongNoQuaHan;
        public int TongNoQuaHan
        {
            get { return _tongNoQuaHan; }
            set { _tongNoQuaHan = value; OnPropertyChanged(nameof(TongNoQuaHan)); }
        }

        private ObservableCollection<v_ThongKeSach> _listThongKeSach;
        public ObservableCollection<v_ThongKeSach> ListThongKeSach
        {
            get { return _listThongKeSach; }
            set { _listThongKeSach = value; OnPropertyChanged(nameof(ListThongKeSach)); }
        }

        private ObservableCollection<NoQuaHanDTO> _listNoQuaHan;
        public ObservableCollection<NoQuaHanDTO> ListNoQuaHan
        {
            get { return _listNoQuaHan; }
            set { _listNoQuaHan = value; OnPropertyChanged(nameof(ListNoQuaHan)); }
        }

        public ICommand RefreshCommand { get; set; }
        public ICommand ExportCommand { get; set; }

        public ThongKeViewModel()
        {
            RefreshCommand = new RelayCommand(ExecuteRefresh, CanExecuteAlways);
            ExportCommand = new RelayCommand(ExecuteExport, CanExecuteAlways);

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
                    TongDauSach = context.Saches.Count();
                    TongSanSang = context.BanSaoSaches.Count(b => b.TinhTrang == "Sẵn sàng");
                    TongDangMuon = context.ChiTietPhieuMuons.Count(c => c.TrangThaiTra == "Đang mượn");

                    var today = DateTime.Now.Date;

                    var queryNoQuaHan = (from ct in context.ChiTietPhieuMuons
                                         join pm in context.PhieuMuons on ct.MaPhieu equals pm.MaPhieu
                                         join dg in context.DocGias on pm.MaDG equals dg.MaDG
                                         join bs in context.BanSaoSaches on ct.MaBS equals bs.MaBS
                                         join s in context.Saches on bs.MaSach equals s.MaSach
                                         where ct.TrangThaiTra == "Đang mượn" && pm.HanTra < today
                                         select new NoQuaHanDTO
                                         {
                                             HoTen = dg.HoTen,
                                             TenSach = s.TenSach
                                         }).ToList();

                    TongNoQuaHan = queryNoQuaHan.Count;
                    ListNoQuaHan = new ObservableCollection<NoQuaHanDTO>(queryNoQuaHan);

                    ListThongKeSach = new ObservableCollection<v_ThongKeSach>(context.v_ThongKeSach.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteRefresh(object parameter)
        {
            LoadData();
        }

        private void ExecuteExport(object parameter)
        {
            MessageBox.Show("Tính năng xuất báo cáo đang được phát triển!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}