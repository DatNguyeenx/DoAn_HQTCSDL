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
        private object _currentView;
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        private string _currentAccountName;
        public string CurrentAccountName
        {
            get { return _currentAccountName; }
            set
            {
                _currentAccountName = value;
                OnPropertyChanged(nameof(CurrentAccountName));
            }
        }

        private string _currentAccountRole;
        public string CurrentAccountRole
        {
            get { return _currentAccountRole; }
            set
            {
                _currentAccountRole = value;
                OnPropertyChanged(nameof(CurrentAccountRole));
                OnPropertyChanged(nameof(IsManager));
            }
        }

        public bool IsManager => CurrentAccountRole == "Quản lý";

        public ICommand ShowDashboardCommand { get; set; }
        public ICommand ShowSachCommand { get; set; }
        public ICommand ShowBanSaoCommand { get; set; }
        public ICommand ShowDanhMucCommand { get; set; }
        public ICommand ShowPhieuNhapCommand { get; set; }
        public ICommand ShowDocGiaCommand { get; set; }
        public ICommand ShowNhanVienCommand { get; set; }
        public ICommand ShowPhieuMuonCommand { get; set; }
        public ICommand ShowTraSachCommand { get; set; }
        public ICommand ShowPhieuPhatCommand { get; set; }
        public ICommand ShowThongKeCommand { get; set; }
        public ICommand LogoutCommand { get; set; }

        public MainViewModel()
        {
            if (GlobalStore.CurrentAccount != null)
            {
                CurrentAccountName = GlobalStore.CurrentAccount.HoTen;
                CurrentAccountRole = GlobalStore.CurrentAccount.ChucVu;
            }

            CurrentView = new UCDashboardView();

            ShowDashboardCommand = new RelayCommand(ExecuteShowDashboard, CanExecuteAlways);
            ShowSachCommand = new RelayCommand(ExecuteShowSach, CanExecuteAlways);
            ShowBanSaoCommand = new RelayCommand(ExecuteShowBanSao, CanExecuteAlways);
            ShowDanhMucCommand = new RelayCommand(ExecuteShowDanhMuc, CanExecuteAlways);
            ShowPhieuNhapCommand = new RelayCommand(ExecuteShowPhieuNhap, CanExecuteAlways);
            ShowDocGiaCommand = new RelayCommand(ExecuteShowDocGia, CanExecuteAlways);
            ShowNhanVienCommand = new RelayCommand(ExecuteShowNhanVien, CanExecuteAlways);
            ShowPhieuMuonCommand = new RelayCommand(ExecuteShowPhieuMuon, CanExecuteAlways);
            ShowTraSachCommand = new RelayCommand(ExecuteShowTraSach, CanExecuteAlways);
            ShowPhieuPhatCommand = new RelayCommand(ExecuteShowPhieuPhat, CanExecuteAlways);
            ShowThongKeCommand = new RelayCommand(ExecuteShowThongKe, CanExecuteAlways);
            LogoutCommand = new RelayCommand(ExecuteLogout, CanExecuteAlways);
        }

        private bool CanExecuteAlways(object parameter)
        {
            return true;
        }

        private void ExecuteShowDashboard(object parameter)
        {
            CurrentView = new UCDashboardView();
        }

        private void ExecuteShowSach(object parameter)
        {
            CurrentView = new UCSachView();
        }

        private void ExecuteShowBanSao(object parameter)
        {
            CurrentView = new UCBanSaoSachView();
        }

        private void ExecuteShowDanhMuc(object parameter)
        {
            CurrentView = new UCDanhMucView();
        }

        private void ExecuteShowPhieuNhap(object parameter)
        {
            CurrentView = new UCPhieuNhapView();
        }

        private void ExecuteShowDocGia(object parameter)
        {
            CurrentView = new UCDocGiaView();
        }

        private void ExecuteShowNhanVien(object parameter)
        {
            if (IsManager)
            {
                CurrentView = new UCNhanVienView();
            }
            else
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng này!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ExecuteShowPhieuMuon(object parameter)
        {
            CurrentView = new UCMuonSachView();
        }

        private void ExecuteShowTraSach(object parameter)
        {
            CurrentView = new UCTraSachView();
        }

        private void ExecuteShowPhieuPhat(object parameter)
        {
            CurrentView = new UCPhieuPhatView();
        }

        private void ExecuteShowThongKe(object parameter)
        {
            CurrentView = new UCThongKeView();
        }

        private void ExecuteLogout(object parameter)
        {
            GlobalStore.CurrentAccount = null;

            LoginView loginWindow = new LoginView();
            loginWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is MainView)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
