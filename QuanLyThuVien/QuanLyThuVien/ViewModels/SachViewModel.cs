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
        // Danh sách sách hiển thị lên DataGrid
        private ObservableCollection<Sach> _listSach;
        public ObservableCollection<Sach> ListSach
        {
            get => _listSach;
            set { _listSach = value; OnPropertyChanged(nameof(ListSach)); }
        }

        // Đối tượng Sách đang được chọn trên DataGrid
        private Sach _selectedSach;
        public Sach SelectedSach
        {
            get => _selectedSach;
            set
            {
                _selectedSach = value;
                OnPropertyChanged(nameof(SelectedSach));
                if (SelectedSach != null)
                {
                    // Khi click vào 1 dòng trên DataGrid, đưa dữ liệu lên các TextBox
                    TenSach = SelectedSach.TenSach;
                    TacGia = SelectedSach.TacGia;
                    SoLuongTon = SelectedSach.SoLuongTon;
                    NamXB = SelectedSach.NamXB;
                }
            }
        }

        // --- Khai báo các thuộc tính Binding cho View (TextBox) ---
        private string _tenSach;
        public string TenSach { get => _tenSach; set { _tenSach = value; OnPropertyChanged(nameof(TenSach)); } }

        private string _tacGia;
        public string TacGia { get => _tacGia; set { _tacGia = value; OnPropertyChanged(nameof(TacGia)); } }

        private int? _soLuongTon;
        public int? SoLuongTon { get => _soLuongTon; set { _soLuongTon = value; OnPropertyChanged(nameof(SoLuongTon)); } }

        private int? _namXB;
        public int? NamXB { get => _namXB; set { _namXB = value; OnPropertyChanged(nameof(NamXB)); } }

        // --- Khai báo các Command ---
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand CallProcedureCommand { get; set; }

        public SachViewModel()
        {
            LoadData();

            // 1. Lệnh Thêm Sách
            AddCommand = new RelayCommand((p) =>
            {
                // Khối lệnh thực thi (Execute)
                using (var db = new QLTVEntities())
                {
                    var sachMoi = new Sach()
                    {
                        TenSach = TenSach,
                        TacGia = TacGia,
                        SoLuongTon = SoLuongTon,
                        NamXB = NamXB
                    };
                    db.Saches.Add(sachMoi);
                    db.SaveChanges();

                    ListSach.Add(sachMoi); // Cập nhật UI ngay lập tức
                    MessageBox.Show("Thêm sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }, (p) =>
            {
                // Điều kiện kích hoạt nút (CanExecute)
                return !string.IsNullOrEmpty(TenSach);
            });

            // 2. Lệnh Sửa Sách
            EditCommand = new RelayCommand((p) =>
            {
                using (var db = new QLTVEntities())
                {
                    var sachToEdit = db.Saches.Where(x => x.MaSach == SelectedSach.MaSach).SingleOrDefault();
                    if (sachToEdit != null)
                    {
                        sachToEdit.TenSach = TenSach;
                        sachToEdit.TacGia = TacGia;
                        sachToEdit.SoLuongTon = SoLuongTon;
                        sachToEdit.NamXB = NamXB;
                        db.SaveChanges();

                        LoadData(); // Load lại để làm mới UI
                        MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }, (p) =>
            {
                // Chỉ cho phép sửa khi đã chọn 1 cuốn sách trên DataGrid
                return SelectedSach != null;
            });

            // 3. Lệnh Xóa Sách
            DeleteCommand = new RelayCommand((p) =>
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn xóa cuốn sách này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    using (var db = new QLTVEntities())
                    {
                        var sachToDelete = db.Saches.Where(x => x.MaSach == SelectedSach.MaSach).SingleOrDefault();
                        if (sachToDelete != null)
                        {
                            db.Saches.Remove(sachToDelete);
                            db.SaveChanges();

                            ListSach.Remove(SelectedSach);
                            MessageBox.Show("Xóa sách thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }, (p) =>
            {
                return SelectedSach != null;
            });

            // 4. Lệnh gọi Procedure sp_TraSach
            CallProcedureCommand = new RelayCommand((p) =>
            {
                CallStoredProcedure();
            }, (p) =>
            {
                return true; // Nút này luôn có thể click
            });
        }

        // --- Các hàm hỗ trợ ---

        private void LoadData()
        {
            using (var db = new QLTVEntities())
            {
                // Sử dụng AsNoTracking để tối ưu hiệu suất khi đọc dữ liệu lên UI
                ListSach = new ObservableCollection<Sach>(db.Saches.AsNoTracking().ToList());
            }
        }

        private void CallStoredProcedure()
        {
            using (var db = new QLTVEntities())
            {
                try
                {
                    // Giả lập dữ liệu truyền vào sp_TraSach để test chức năng
                    // Trong thực tế bạn có thể lấy giá trị này từ giao diện Trả Sách
                    int maPhieuTest = 1;
                    int maSachTest = 1;
                    DateTime ngayTraTest = DateTime.Now;

                    // Gọi raw SQL query thẳng từ Database Context
                    db.Database.ExecuteSqlCommand("EXEC sp_TraSach @p0, @p1, @p2", maPhieuTest, maSachTest, ngayTraTest);

                    MessageBox.Show("Gọi Procedure sp_TraSach thành công!\nĐã cập nhật lại trạng thái trả và số lượng tồn kho.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                    LoadData(); // Cập nhật lại list sách để thấy SoLuongTon có thể thay đổi
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi gọi Procedure: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
