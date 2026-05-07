using QuanLyThuVien.Models;
using QuanLyThuVien.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace QuanLyThuVien.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        // View hiện tại được hiển thị trên ContentControl
        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(nameof(CurrentView)); }
        }

        // --- Thông tin tài khoản hiển thị lên Sidebar ---
        private string _currentAccountName;
        public string CurrentAccountName
        {
            get => _currentAccountName;
            set { _currentAccountName = value; OnPropertyChanged(nameof(CurrentAccountName)); }
        }

        private string _currentAccountRole;
        public string CurrentAccountRole
        {
            get => _currentAccountRole;
            set { _currentAccountRole = value; OnPropertyChanged(nameof(CurrentAccountRole)); }
        }

        // Biến lưu trữ Account hiện tại để dùng cho các nghiệp vụ sau này (nếu cần)
        public NhanVien CurrentAccount { get; set; }

        // --- Định nghĩa các Command để điều hướng ---
        public ICommand ShowDashboardCommand { get; set; }
        public ICommand ShowSachCommand { get; set; }
        public ICommand ShowDocGiaCommand { get; set; }
        public ICommand ShowNhanVienCommand { get; set; }
        public ICommand ShowPhieuMuonCommand { get; set; }
        public ICommand ShowTraSachCommand { get; set; }
        public ICommand ShowThongKeCommand { get; set; }
        public ICommand ShowProfileCommand { get; set; }
        public ICommand ShowSettingsCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        // Constructor nhận thông tin tài khoản từ LoginViewModel
        public MainViewModel(NhanVien account)
        {
            // 1. Gán thông tin nhân viên
            CurrentAccount = account;
            CurrentAccountName = account.HoTen;
            CurrentAccountRole = account.ChucVu;

            // 2. Khởi tạo View mặc định khi vừa đăng nhập xong là Dashboard
            CurrentView = new UCDashboardView(); // Hãy chắc chắn bạn đã tạo UCDashboardView

            // 3. Cài đặt các lệnh điều hướng menu
            ShowDashboardCommand = new RelayCommand((p) => { CurrentView = new UCDashboardView(); }, (p) => true);

            ShowSachCommand = new RelayCommand((p) => { CurrentView = new UCQuanLySach(); }, (p) => true);

            ShowDocGiaCommand = new RelayCommand((p) => { CurrentView = new UCQuanLyDocGia(); }, (p) => true);

            // Phân quyền: Nút nhân viên chỉ hoạt động khi là Admin
            ShowNhanVienCommand = new RelayCommand((p) =>
            {
                if (CurrentAccountRole == "Admin")
                    CurrentView = new UCQuanLyNhanVien();
                else
                    MessageBox.Show("Bạn không có quyền truy cập chức năng này!", "Cảnh báo quyền", MessageBoxButton.OK, MessageBoxImage.Warning);
            }, (p) => true);

            // Các UserControl dưới đây nếu bạn chưa làm thì có thể tạo rỗng để tránh lỗi build
            // ShowPhieuMuonCommand = new RelayCommand((p) => { CurrentView = new UCPhieuMuon(); }, (p) => true);
            // ShowTraSachCommand = new RelayCommand((p) => { CurrentView = new UCTraSach(); }, (p) => true);
            // ShowThongKeCommand = new RelayCommand((p) => { CurrentView = new UCThongKe(); }, (p) => true);

            ShowProfileCommand = new RelayCommand((p) => { /* CurrentView = new UCProfile(); */ }, (p) => true);

            ShowSettingsCommand = new RelayCommand((p) =>
            {
                if (CurrentAccountRole == "Admin")
                    CurrentView = new UCSetting();
                else
                    MessageBox.Show("Chỉ Admin mới được vào phần Cài đặt hệ thống!", "Cảnh báo quyền", MessageBoxButton.OK, MessageBoxImage.Warning);
            }, (p) => true);

            // 4. Lệnh Đăng xuất
            LogoutCommand = new RelayCommand((p) =>
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Lấy window hiện tại
                    Window currentWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);

                    // Khởi tạo và hiển thị màn hình đăng nhập
                    LoginView loginView = new LoginView();
                    loginView.DataContext = new LoginViewModel();
                    loginView.Show();

                    // Đóng màn hình chính
                    currentWindow?.Close();
                }
            }, (p) => true);
        }
    }
}
