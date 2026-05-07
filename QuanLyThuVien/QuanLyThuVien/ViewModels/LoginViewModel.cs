using QuanLyThuVien.Models;
using QuanLyThuVien.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyThuVien.ViewModels
{
    class LoginViewModel : BaseViewModel
    {
        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(nameof(Username)); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public ICommand LoginCommand { get; set; }

        public LoginViewModel()
        {
            // Reset thông báo lỗi
            ErrorMessage = "";

            LoginCommand = new RelayCommand((parameter) =>
            {
                // parameter chính là PasswordBox được truyền từ XAML
                var passwordBox = parameter as PasswordBox;
                if (passwordBox == null) return;

                string password = passwordBox.Password;

                if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
                {
                    ErrorMessage = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!";
                    return;
                }

                using (var db = new QuanLyThuVienEntities())
                {
                    // Truy vấn bảng NhanVien kiểm tra tài khoản
                    var account = db.NhanViens.FirstOrDefault(x => x.TenDN == Username && x.MatKhau == password);

                    if (account != null)
                    {
                        ErrorMessage = "";

                        // Khởi tạo cửa sổ chính
                        MainView mainWindow = new MainView();

                        // Khởi tạo MainViewModel và truyền thông tin tài khoản vào
                        MainViewModel mainViewModel = new MainViewModel(account);
                        mainWindow.DataContext = mainViewModel;

                        // Lấy cửa sổ Login hiện tại để đóng lại
                        Window loginWindow = Window.GetWindow(passwordBox);

                        // Mở cửa sổ chính và đóng cửa sổ login
                        mainWindow.Show();
                        loginWindow?.Close();
                    }
                    else
                    {
                        ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                    }
                }
            }, (parameter) => true);
        }
    }
}
