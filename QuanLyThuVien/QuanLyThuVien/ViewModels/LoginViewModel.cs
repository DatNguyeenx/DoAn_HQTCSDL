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
using QuanLyThuVien.Models;
using QuanLyThuVien.Views;

namespace QuanLyThuVien.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {

        private string _username;
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public LoginViewModel()
        {
            Username = "";
            ErrorMessage = "";

            LoginCommand = new RelayCommand(ExecuteLogin, CanExecuteLogin);
            ExitCommand = new RelayCommand(ExecuteExit, CanExecuteExit);
        }

        private bool CanExecuteLogin(object parameter)
        {
            return true;
        }

        private void ExecuteLogin(object parameter)
        {
            var passwordBox = parameter as PasswordBox;
            if (passwordBox == null) return;

            string password = passwordBox.Password;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Vui lòng nhập đầy đủ Tên đăng nhập và Mật khẩu!";
                return;
            }

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    // So khớp tài khoản dưới Database
                    var acc = context.NhanViens.FirstOrDefault(x => x.TenDN == Username && x.MatKhau == password);

                    if (acc != null)
                    {
                        ErrorMessage = "";
                        GlobalStore.CurrentAccount = acc;


                        MainView mainView = new MainView();
                        MainViewModel mainVM = new MainViewModel();

                        mainVM.CurrentAccountName = acc.HoTen;
                        mainVM.CurrentAccountRole = acc.ChucVu;

                        mainView.DataContext = mainVM;
                        mainView.Show();

                        // Đóng LoginView
                        Window loginWindow = Window.GetWindow(passwordBox);
                        if (loginWindow != null)
                        {
                            loginWindow.Close();
                        }
                    }
                    else
                    {
                        ErrorMessage = "Tên đăng nhập hoặc mật khẩu không chính xác!";
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Lỗi kết nối cơ sở dữ liệu: " + ex.Message;
            }
        }

        private bool CanExecuteExit(object parameter)
        {
            return true;
        }

        private void ExecuteExit(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
    public static class GlobalStore
    {
        public static NhanVien CurrentAccount { get; set; }
    }
}
