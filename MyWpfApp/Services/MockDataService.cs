using MyWpfApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWpfApp.Services
{
    public class MockDataService : IDataService
    {
        private List<User> _users;
        private List<Product> _products;
        private int _nextUserId = 1;
        private int _nextProductId = 1;

        public MockDataService()
        {
            // Initialize with some sample data
            _users = new List<User>
            {
                new User { Id = _nextUserId++, Name = "John Doe", Email = "john@example.com", Phone = "555-1234" },
                new User { Id = _nextUserId++, Name = "Jane Smith", Email = "jane@example.com", Phone = "555-5678" }
            };

            _products = new List<Product>
            {
                new Product { Id = _nextProductId++, Name = "Laptop", Price = 1299.99m, StockQuantity = 10 },
                new Product { Id = _nextProductId++, Name = "Smartphone", Price = 899.99m, StockQuantity = 25 }
            };
        }

        // User CRUD operations
        public Task<List<User>> GetAllUsersAsync()
        {
            return Task.FromResult(_users.ToList());
        }

        public Task<User> GetUserByIdAsync(int id)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == id));
        }

        public Task<User> AddUserAsync(User user)
        {
            user.Id = _nextUserId++;
            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task<bool> UpdateUserAsync(User user)
        {
            var existingUserIndex = _users.FindIndex(u => u.Id == user.Id);
            if (existingUserIndex != -1)
            {
                _users[existingUserIndex] = user;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> DeleteUserAsync(int id)
        {
            var existingUser = _users.FirstOrDefault(u => u.Id == id);
            if (existingUser != null)
            {
                _users.Remove(existingUser);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        // Product CRUD operations
        public Task<List<Product>> GetAllProductsAsync()
        {
            return Task.FromResult(_products.ToList());
        }

        public Task<Product> GetProductByIdAsync(int id)
        {
            return Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
        }

        public Task<Product> AddProductAsync(Product product)
        {
            product.Id = _nextProductId++;
            _products.Add(product);
            return Task.FromResult(product);
        }

        public Task<bool> UpdateProductAsync(Product product)
        {
            var existingProductIndex = _products.FindIndex(p => p.Id == product.Id);
            if (existingProductIndex != -1)
            {
                _products[existingProductIndex] = product;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == id);
            if (existingProduct != null)
            {
                _products.Remove(existingProduct);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
