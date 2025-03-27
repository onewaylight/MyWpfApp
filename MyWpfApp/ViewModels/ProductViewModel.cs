using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyWpfApp.Models;
using MyWpfApp.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace MyWpfApp.ViewModels
{
    public partial class ProductViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        // Properties
        [ObservableProperty]
        private ObservableCollection<Product> _products;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditProductCommand), nameof(DeleteProductCommand))]
        private Product _selectedProduct;

        [ObservableProperty]
        private Product _editingProduct;

        [ObservableProperty]
        private bool _isEditing;

        public ProductViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Products = new ObservableCollection<Product>();
            EditingProduct = new Product();

            // Load products when the ViewModel is created
            _ = LoadProducts();
        }

        [RelayCommand]
        private async Task LoadProducts()
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

        [RelayCommand]
        private void AddProduct()
        {
            EditingProduct = new Product();
            IsEditing = true;
            SelectedProduct = null;
        }

        [RelayCommand(CanExecute = nameof(CancelEditProduct))]
        private void EditProduct()
        {
            if (SelectedProduct != null)
            {
                EditingProduct = SelectedProduct.Clone();
                IsEditing = true;
            }
        }

        private bool CancelEditProduct() => SelectedProduct != null;

        [RelayCommand]
        private void CancelEdit()
        {
            IsEditing = false;
            EditingProduct = new Product();
        }

        [RelayCommand]
        private async Task SaveProduct()
        {
            if (EditingProduct == null) return;

            try
            {
                if (EditingProduct.Id == 0)
                {
                    // Add new product
                    var addedProduct = await _dataService.AddProductAsync(EditingProduct);
                    Products.Add(addedProduct);
                }
                else
                {
                    // Update existing product
                    await _dataService.UpdateProductAsync(EditingProduct);
                    var existingProductIndex = Products.IndexOf(Products.FirstOrDefault(p => p.Id == EditingProduct.Id));
                    if (existingProductIndex >= 0)
                    {
                        Products[existingProductIndex] = EditingProduct;
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

        [RelayCommand(CanExecute = nameof(CanDeleteProduct))]
        private async Task DeleteProduct()
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

        private bool CanDeleteProduct() => SelectedProduct != null;

        partial void OnSelectedProductChanged(Product value)
        {
            EditingProduct = value?.Clone();
        }
    }
}
