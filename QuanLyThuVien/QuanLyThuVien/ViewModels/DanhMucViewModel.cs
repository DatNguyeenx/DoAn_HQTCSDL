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
    class DanhMucViewModel : BaseViewModel
    {
        private ObservableCollection<LoaiSach> _listLoaiSach;
        public ObservableCollection<LoaiSach> ListLoaiSach
        {
            get { return _listLoaiSach; }
            set { _listLoaiSach = value; OnPropertyChanged(nameof(ListLoaiSach)); }
        }

        private LoaiSach _selectedLoaiSach;
        public LoaiSach SelectedLoaiSach
        {
            get { return _selectedLoaiSach; }
            set
            {
                if (_selectedLoaiSach == value) return;
                _selectedLoaiSach = value;
                OnPropertyChanged(nameof(SelectedLoaiSach));
                if (_selectedLoaiSach != null)
                {
                    MaLoai = _selectedLoaiSach.MaLoai;
                    TenLoai = _selectedLoaiSach.TenLoai;
                    IsEditingLoaiMode = false;
                }
                else
                {
                    MaLoai = string.Empty;
                    TenLoai = string.Empty;
                    IsEditingLoaiMode = true;
                }
            }
        }

        private string _maLoai;
        public string MaLoai
        {
            get { return _maLoai; }
            set { _maLoai = value; OnPropertyChanged(nameof(MaLoai)); }
        }

        private string _tenLoai;
        public string TenLoai
        {
            get { return _tenLoai; }
            set { _tenLoai = value; OnPropertyChanged(nameof(TenLoai)); }
        }

        private bool _isEditingLoaiMode;
        public bool IsEditingLoaiMode
        {
            get { return _isEditingLoaiMode; }
            set { _isEditingLoaiMode = value; OnPropertyChanged(nameof(IsEditingLoaiMode)); }
        }

        private ObservableCollection<NhaXuatBan> _listNXB;
        public ObservableCollection<NhaXuatBan> ListNXB
        {
            get { return _listNXB; }
            set { _listNXB = value; OnPropertyChanged(nameof(ListNXB)); }
        }

        private NhaXuatBan _selectedNXB;
        public NhaXuatBan SelectedNXB
        {
            get { return _selectedNXB; }
            set
            {
                if (_selectedNXB == value) return;
                _selectedNXB = value;
                OnPropertyChanged(nameof(SelectedNXB));
                if (_selectedNXB != null)
                {
                    MaNXB = _selectedNXB.MaNXB;
                    TenNXB = _selectedNXB.TenNXB;
                    DiaChiNXB = _selectedNXB.DiaChi;
                    SoDTNXB = _selectedNXB.SoDT;
                    IsEditingNXBMode = false;
                }
                else
                {
                    MaNXB = string.Empty;
                    TenNXB = string.Empty;
                    DiaChiNXB = string.Empty;
                    SoDTNXB = string.Empty;
                    IsEditingNXBMode = true;
                }
            }
        }

        private string _maNXB;
        public string MaNXB
        {
            get { return _maNXB; }
            set { _maNXB = value; OnPropertyChanged(nameof(MaNXB)); }
        }

        private string _tenNXB;
        public string TenNXB
        {
            get { return _tenNXB; }
            set { _tenNXB = value; OnPropertyChanged(nameof(TenNXB)); }
        }

        private string _diaChiNXB;
        public string DiaChiNXB
        {
            get { return _diaChiNXB; }
            set { _diaChiNXB = value; OnPropertyChanged(nameof(DiaChiNXB)); }
        }

        private string _soDTNXB;
        public string SoDTNXB
        {
            get { return _soDTNXB; }
            set { _soDTNXB = value; OnPropertyChanged(nameof(SoDTNXB)); }
        }

        private bool _isEditingNXBMode;
        public bool IsEditingNXBMode
        {
            get { return _isEditingNXBMode; }
            set { _isEditingNXBMode = value; OnPropertyChanged(nameof(IsEditingNXBMode)); }
        }

        private ObservableCollection<TacGia> _listTacGia;
        public ObservableCollection<TacGia> ListTacGia
        {
            get { return _listTacGia; }
            set { _listTacGia = value; OnPropertyChanged(nameof(ListTacGia)); }
        }

        private TacGia _selectedTacGia;
        public TacGia SelectedTacGia
        {
            get { return _selectedTacGia; }
            set
            {
                if (_selectedTacGia == value) return;
                _selectedTacGia = value;
                OnPropertyChanged(nameof(SelectedTacGia));
                if (_selectedTacGia != null)
                {
                    MaTG = _selectedTacGia.MaTG;
                    TenTG = _selectedTacGia.TenTG;
                    IsEditingTacGiaMode = false;
                }
                else
                {
                    MaTG = string.Empty;
                    TenTG = string.Empty;
                    IsEditingTacGiaMode = true;
                }
            }
        }

        private string _maTG;
        public string MaTG
        {
            get { return _maTG; }
            set { _maTG = value; OnPropertyChanged(nameof(MaTG)); }
        }

        private string _tenTG;
        public string TenTG
        {
            get { return _tenTG; }
            set { _tenTG = value; OnPropertyChanged(nameof(TenTG)); }
        }

        private bool _isEditingTacGiaMode;
        public bool IsEditingTacGiaMode
        {
            get { return _isEditingTacGiaMode; }
            set { _isEditingTacGiaMode = value; OnPropertyChanged(nameof(IsEditingTacGiaMode)); }
        }

        public ICommand AddLoaiCommand { get; set; }
        public ICommand UpdateLoaiCommand { get; set; }
        public ICommand DeleteLoaiCommand { get; set; }

        public ICommand AddNXBCommand { get; set; }
        public ICommand UpdateNXBCommand { get; set; }
        public ICommand DeleteNXBCommand { get; set; }

        public ICommand AddTacGiaCommand { get; set; }
        public ICommand UpdateTacGiaCommand { get; set; }
        public ICommand DeleteTacGiaCommand { get; set; }

        public DanhMucViewModel()
        {
            IsEditingLoaiMode = true;
            IsEditingNXBMode = true;
            IsEditingTacGiaMode = true;

            LoadData();

            AddLoaiCommand = new RelayCommand(ExecuteAddLoai, CanExecuteAlways);
            UpdateLoaiCommand = new RelayCommand(ExecuteUpdateLoai, CanExecuteAlways);
            DeleteLoaiCommand = new RelayCommand(ExecuteDeleteLoai, CanExecuteAlways);

            AddNXBCommand = new RelayCommand(ExecuteAddNXB, CanExecuteAlways);
            UpdateNXBCommand = new RelayCommand(ExecuteUpdateNXB, CanExecuteAlways);
            DeleteNXBCommand = new RelayCommand(ExecuteDeleteNXB, CanExecuteAlways);

            AddTacGiaCommand = new RelayCommand(ExecuteAddTacGia, CanExecuteAlways);
            UpdateTacGiaCommand = new RelayCommand(ExecuteUpdateTacGia, CanExecuteAlways);
            DeleteTacGiaCommand = new RelayCommand(ExecuteDeleteTacGia, CanExecuteAlways);
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
                    ListLoaiSach = new ObservableCollection<LoaiSach>(context.LoaiSaches.ToList());
                    ListNXB = new ObservableCollection<NhaXuatBan>(context.NhaXuatBans.ToList());
                    ListTacGia = new ObservableCollection<TacGia>(context.TacGias.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteAddLoai(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaLoai) || string.IsNullOrWhiteSpace(TenLoai)) return;

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    if (context.LoaiSaches.Any(x => x.MaLoai == MaLoai)) return;

                    context.LoaiSaches.Add(new LoaiSach { MaLoai = MaLoai, TenLoai = TenLoai });
                    context.SaveChanges();

                    SelectedLoaiSach = null;
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteUpdateLoai(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaLoai) || string.IsNullOrWhiteSpace(TenLoai)) return;

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var entity = context.LoaiSaches.FirstOrDefault(x => x.MaLoai == MaLoai);
                    if (entity != null)
                    {
                        entity.TenLoai = TenLoai;
                        context.SaveChanges();

                        SelectedLoaiSach = null;
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteLoai(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaLoai)) return;

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        var entity = context.LoaiSaches.FirstOrDefault(x => x.MaLoai == MaLoai);
                        if (entity != null)
                        {
                            context.LoaiSaches.Remove(entity);
                            context.SaveChanges();

                            SelectedLoaiSach = null;
                            LoadData();
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Không thể xóa dữ liệu đang được sử dụng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteAddNXB(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaNXB) || string.IsNullOrWhiteSpace(TenNXB)) return;

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    if (context.NhaXuatBans.Any(x => x.MaNXB == MaNXB)) return;

                    context.NhaXuatBans.Add(new NhaXuatBan { MaNXB = MaNXB, TenNXB = TenNXB, DiaChi = DiaChiNXB, SoDT = SoDTNXB });
                    context.SaveChanges();

                    SelectedNXB = null;
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteUpdateNXB(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaNXB) || string.IsNullOrWhiteSpace(TenNXB)) return;

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var entity = context.NhaXuatBans.FirstOrDefault(x => x.MaNXB == MaNXB);
                    if (entity != null)
                    {
                        entity.TenNXB = TenNXB;
                        entity.DiaChi = DiaChiNXB;
                        entity.SoDT = SoDTNXB;
                        context.SaveChanges();

                        SelectedNXB = null;
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteNXB(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaNXB)) return;

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        var entity = context.NhaXuatBans.FirstOrDefault(x => x.MaNXB == MaNXB);
                        if (entity != null)
                        {
                            context.NhaXuatBans.Remove(entity);
                            context.SaveChanges();

                            SelectedNXB = null;
                            LoadData();
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Không thể xóa dữ liệu đang được sử dụng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExecuteAddTacGia(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaTG) || string.IsNullOrWhiteSpace(TenTG)) return;

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    if (context.TacGias.Any(x => x.MaTG == MaTG)) return;

                    context.TacGias.Add(new TacGia { MaTG = MaTG, TenTG = TenTG });
                    context.SaveChanges();

                    SelectedTacGia = null;
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteUpdateTacGia(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaTG) || string.IsNullOrWhiteSpace(TenTG)) return;

            try
            {
                using (var context = new QuanLyThuVienEntities())
                {
                    var entity = context.TacGias.FirstOrDefault(x => x.MaTG == MaTG);
                    if (entity != null)
                    {
                        entity.TenTG = TenTG;
                        context.SaveChanges();

                        SelectedTacGia = null;
                        LoadData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteDeleteTacGia(object parameter)
        {
            if (string.IsNullOrWhiteSpace(MaTG)) return;

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    using (var context = new QuanLyThuVienEntities())
                    {
                        var entity = context.TacGias.FirstOrDefault(x => x.MaTG == MaTG);
                        if (entity != null)
                        {
                            context.TacGias.Remove(entity);
                            context.SaveChanges();

                            SelectedTacGia = null;
                            LoadData();
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Không thể xóa dữ liệu đang được sử dụng!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
