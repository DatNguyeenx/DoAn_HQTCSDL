//using QuanLyThuVien.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//namespace QuanLyThuVien.Views
//{
//    /// <summary>
//    /// Interaction logic for UCProfile.xaml
//    /// </summary>
//    public partial class UCProfile : UserControl
//    {
//        public UCProfile() : this(null)
//        {
//        }

//        public UCProfile(NhanVien authenticatedUser)
//        {
//            InitializeComponent();

//            ProfileViewModel profileViewModel = new ProfileViewModel(authenticatedUser);
//            profileViewModel.PasswordInputsClearRequested += HandlePasswordInputsClearRequested;
//            DataContext = profileViewModel;

//            Unloaded += UCProfile_Unloaded;
//        }

//        private void CurrentPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
//        {
//            if (DataContext is ProfileViewModel viewModel)
//            {
//                viewModel.CurrentPassword = CurrentPasswordBox.Password;
//            }
//        }

//        private void NewPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
//        {
//            if (DataContext is ProfileViewModel viewModel)
//            {
//                viewModel.NewPassword = NewPasswordBox.Password;
//            }
//        }

//        private void ConfirmPasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
//        {
//            if (DataContext is ProfileViewModel viewModel)
//            {
//                viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
//            }
//        }

//        private void HandlePasswordInputsClearRequested()
//        {
//            CurrentPasswordBox.Password = string.Empty;
//            NewPasswordBox.Password = string.Empty;
//            ConfirmPasswordBox.Password = string.Empty;
//        }

//        private void UCProfile_Unloaded(object sender, RoutedEventArgs e)
//        {
//            if (DataContext is ProfileViewModel viewModel)
//            {
//                viewModel.PasswordInputsClearRequested -= HandlePasswordInputsClearRequested;
//            }

//            Unloaded -= UCProfile_Unloaded;
//        }
//    }   
//}
//}
