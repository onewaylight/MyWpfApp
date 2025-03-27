using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyWpfApp.Services;
using System.Windows;
using MyWpfApp.Views;

namespace MyWpfApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;

            // Initialize commands
            OpenUserWindowCommand = new RelayCommand(OpenUserWindow);
            OpenProductWindowCommand = new RelayCommand(OpenProductWindow);
        }

        // Commands
        public RelayCommand OpenUserWindowCommand { get; }
        public RelayCommand OpenProductWindowCommand { get; }

        private void OpenUserWindow()
        {
            try
            {
                var userViewModel = new UserViewModel(_dataService);
                var userWindow = new UserWindow { DataContext = userViewModel };
                userWindow.Owner = Application.Current.MainWindow;
                userWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error opening User window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenProductWindow()
        {
            try
            {
                var productViewModel = new ProductViewModel(_dataService);
                var productWindow = new ProductWindow { DataContext = productViewModel };
                productWindow.Owner = Application.Current.MainWindow;
                productWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error opening Product window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}