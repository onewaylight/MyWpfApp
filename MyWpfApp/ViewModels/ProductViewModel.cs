using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyWpfApp.Models;
using MyWpfApp.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace MyWpfApp.ViewModels
{
    public class ProductViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        // Properties
        private ObservableCollection<Product> _products;
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }

        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    // When selection changes, update the editing product
                    EditingProduct = value?.Clone();
                    DeleteProductCommand.NotifyCanExecuteChanged();
                    EditProductCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private Product _editingProduct;
        public Product EditingProduct
        {
            get => _editingProduct;
            set => SetProperty(ref _editingProduct, value);
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        // Commands
        public RelayCommand LoadProductsCommand { get; }
        public RelayCommand AddProductCommand { get; }
        public RelayCommand<Product> SaveProductCommand { get; }
        public RelayCommand CancelEditCommand { get; }
        public RelayCommand EditProductCommand { get; }
        public RelayCommand DeleteProductCommand { get; }

        public ProductViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Products = new ObservableCollection<Product>();
            EditingProduct = new Product();

            // Initialize commands
            LoadProductsCommand = new RelayCommand(async () => await LoadProductsAsync());
            AddProductCommand = new RelayCommand(AddProduct);
            SaveProductCommand = new RelayCommand<Product>(async (product) => await SaveProductAsync(product), (product) => product != null);
            CancelEditCommand = new RelayCommand(CancelEdit);
            EditProductCommand = new RelayCommand(EditProduct, () => SelectedProduct != null);
            DeleteProductCommand = new RelayCommand(async () => await DeleteProductAsync(), () => SelectedProduct != null);

            // Load products when the ViewModel is created
            LoadProductsCommand.Execute(null);
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var products = await _dataService.GetAllProductsAsync();
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProduct()
        {
            EditingProduct = new Product();
            IsEditing = true;
            SelectedProduct = null;
        }

        private void EditProduct()
        {
            if (SelectedProduct != null)
            {
                EditingProduct = SelectedProduct.Clone();
                IsEditing = true;
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
            EditingProduct = new Product();
        }

        private async Task SaveProductAsync(Product product)
        {
            try
            {
                if (product.Id == 0)
                {
                    // Add new product
                    var addedProduct = await _dataService.AddProductAsync(product);
                    Products.Add(addedProduct);
                }
                else
                {
                    // Update existing product
                    await _dataService.UpdateProductAsync(product);
                    var existingProductIndex = Products.IndexOf(Products.FirstOrDefault(p => p.Id == product.Id));
                    if (existingProductIndex >= 0)
                    {
                        Products[existingProductIndex] = product;
                    }
                }

                IsEditing = false;
                EditingProduct = new Product();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task DeleteProductAsync()
        {
            if (SelectedProduct == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete {SelectedProduct.Name}?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _dataService.DeleteProductAsync(SelectedProduct.Id);
                    Products.Remove(SelectedProduct);
                    SelectedProduct = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
