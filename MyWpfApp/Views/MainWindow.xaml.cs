using MyWpfApp.Services;
using MyWpfApp.ViewModels;
using System.Windows;

namespace MyWpfApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // If DataContext is not set by App.xaml.cs, set it here
            if (DataContext == null)
            {
                DataContext = new MainViewModel(new MockDataService());
            }
        }
    }
}