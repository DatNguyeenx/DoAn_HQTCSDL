using QuanLyThuVien.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyThuVien.ViewModels
{
    public class ChiTietNoDTO
    {
        public string HoTen { get; set; }
        public string TenSach { get; set; }
        public DateTime? HanTra { get; set; }
        public string TrangThaiTra { get; set; }
    }

    class DashboardViewModel : BaseViewModel
    {
        private int _tongSach;
        public int TongSach
        {
            get { return _tongSach; }
            set { _tongSach = value; OnPropertyChanged(nameof(TongSach)); }
        }

        private int _sachDangMuon;
        public int SachDangMuon
        {
            get { return _sachDangMuon; }
            set { _sachDangMuon = value; OnPropertyChanged(nameof(SachDangMuon)); }
        }

        private int _tongDocGia;
        public int TongDocGia
        {
            get { return _tongDocGia; }
            set { _tongDocGia = value; OnPropertyChanged(nameof(TongDocGia)); }
        }

        private decimal _tienPhatThuDuoc;
        public decimal TienPhatThuDuoc
        {
            get { return _tienPhatThuDuoc; }
            set { _tienPhatThuDuoc = value; OnPropertyChanged(nameof(TienPhatThuDuoc)); }
        }

        private ObservableCollection<ChiTietNoDTO> _listQuahan;
        public ObservableCollection<ChiTietNoDTO> ListQuahan
        {
            get { return _listQuahan; }
            set { _listQuahan = value; OnPropertyChanged(nameof(ListQuahan)); }
        }

        public DashboardViewModel()
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    TongSach = context.BanSaoSaches.Count();

                    SachDangMuon = context.ChiTietPhieuMuons.Count(x => x.TrangThaiTra == "Đang mượn");

                    TongDocGia = context.DocGias.Count();

                    TienPhatThuDuoc = context.PhieuPhats.Sum(x => (decimal?)x.SoTienPhat) ?? 0;

                    var today = DateTime.Now.Date;

                    var query = (from ct in context.ChiTietPhieuMuons
                                 join pm in context.PhieuMuons on ct.MaPhieu equals pm.MaPhieu
                                 join dg in context.DocGias on pm.MaDG equals dg.MaDG
                                 join bs in context.BanSaoSaches on ct.MaBS equals bs.MaBS
                                 join s in context.Saches on bs.MaSach equals s.MaSach
                                 where ct.TrangThaiTra == "Đang mượn" && pm.HanTra < today
                                 select new ChiTietNoDTO
                                 {
                                     HoTen = dg.HoTen,
                                     TenSach = s.TenSach,
                                     HanTra = pm.HanTra,
                                     TrangThaiTra = "Quá hạn"
                                 }).ToList();

                    ListQuahan = new ObservableCollection<ChiTietNoDTO>(query);
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
