using MyWpfApp.Services;
using MyWpfApp.ViewModels;
using MyWpfApp.Views;
using System.Windows;

namespace MyWpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IDataService _dataService;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Initialize services
            _dataService = new MockDataService();

            // Create and show the main window
            var mainViewModel = new MainViewModel(_dataService);
            var mainWindow = new MainWindow { DataContext = mainViewModel };
            mainWindow.Show();
        }
    }

}
