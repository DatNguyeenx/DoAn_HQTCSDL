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
    class DocGiaViewModel : BaseViewModel
    {
        private ObservableCollection<DocGia> _listDocGia;
        public ObservableCollection<DocGia> ListDocGia
        {
            get => _listDocGia;
            set { _listDocGia = value; OnPropertyChanged(nameof(ListDocGia)); }
        }

        private DocGia _selectedDocGia;
        public DocGia SelectedDocGia
        {
            get => _selectedDocGia;
            set
            {
                _selectedDocGia = value;
                OnPropertyChanged(nameof(SelectedDocGia));
                if (SelectedDocGia != null)
                {
                    HoTen = SelectedDocGia.HoTen;
                    DiaChi = SelectedDocGia.DiaChi;
                    SoDT = SelectedDocGia.SoDT;
                }
            }
        }

        private string _hoTen;
        public string HoTen { get => _hoTen; set { _hoTen = value; OnPropertyChanged(nameof(HoTen)); } }

        private string _diaChi;
        public string DiaChi { get => _diaChi; set { _diaChi = value; OnPropertyChanged(nameof(DiaChi)); } }

        private string _soDT;
        public string SoDT { get => _soDT; set { _soDT = value; OnPropertyChanged(nameof(SoDT)); } }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public DocGiaViewModel()
        {
            LoadData();

            AddCommand = new RelayCommand((p) =>
            {
                using (var db = new QuanLyThuVienEntities())
                {
                    var dgMoi = new DocGia() { HoTen = HoTen, DiaChi = DiaChi, SoDT = SoDT };
                    db.DocGias.Add(dgMoi);
                    db.SaveChanges();
                    ListDocGia.Add(dgMoi);
                    MessageBox.Show("Thêm độc giả thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }, (p) => !string.IsNullOrEmpty(HoTen));

            EditCommand = new RelayCommand((p) =>
            {
                using (var db = new QuanLyThuVienEntities())
                {
                    var dg = db.DocGias.FirstOrDefault(x => x.MaDG == SelectedDocGia.MaDG);
                    if (dg != null)
                    {
                        dg.HoTen = HoTen;
                        dg.DiaChi = DiaChi;
                        dg.SoDT = SoDT;
                        db.SaveChanges();
                        LoadData();
                        MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }, (p) => SelectedDocGia != null);

            DeleteCommand = new RelayCommand((p) =>
            {
                if (MessageBox.Show("Xóa độc giả này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    using (var db = new QuanLyThuVienEntities())
                    {
                        var dg = db.DocGias.FirstOrDefault(x => x.MaDG == SelectedDocGia.MaDG);
                        if (dg != null)
                        {
                            db.DocGias.Remove(dg);
                            db.SaveChanges();
                            ListDocGia.Remove(SelectedDocGia);
                        }
                    }
                }
            }, (p) => SelectedDocGia != null);
        }

        private void LoadData()
        {
            using (var db = new QuanLyThuVienEntities())
            {
                ListDocGia = new ObservableCollection<DocGia>(db.DocGias.AsNoTracking().ToList());
            }
        }
    }
}