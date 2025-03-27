using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyWpfApp.Models;
using MyWpfApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyWpfApp.ViewModels
{
    public partial class UserViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        // Properties
        [ObservableProperty]
        private ObservableCollection<User> _users;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditUserCommand), nameof(DeleteUserCommand))]
        private User _selectedUser;

        [ObservableProperty]
        private User _editingUser;

        [ObservableProperty]
        private bool _isEditing;

        public UserViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Users = new ObservableCollection<User>();
            EditingUser = new User();

            // Load users when the ViewModel is created
            _ = LoadUsers();
        }

        [RelayCommand]
        private async Task LoadUsers()
        {
            try
            {
                var users = await _dataService.GetAllUsersAsync();
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private void AddUser()
        {
            EditingUser = new User();
            IsEditing = true;
            SelectedUser = null;
        }

        [RelayCommand(CanExecute = nameof(CanEditUser))]
        private void EditUser()
        {
            if (SelectedUser != null)
            {
                EditingUser = SelectedUser.Clone();
                IsEditing = true;
            }
        }

        private bool CanEditUser() => SelectedUser != null;

        [RelayCommand]
        private void CancelEdit()
        {
            IsEditing = false;
            EditingUser = new User();
        }

        [RelayCommand]
        private async Task SaveUser()
        {
            if (EditingUser == null) return;

            try
            {
                if (EditingUser.Id == 0)
                {
                    // Add new user
                    var addedUser = await _dataService.AddUserAsync(EditingUser);
                    Users.Add(addedUser);
                }
                else
                {
                    // Update existing user
                    await _dataService.UpdateUserAsync(EditingUser);
                    var existingUserIndex = Users.IndexOf(Users.FirstOrDefault(u => u.Id == EditingUser.Id));
                    if (existingUserIndex >= 0)
                    {
                        Users[existingUserIndex] = EditingUser;
                    }
                }

                IsEditing = false;
                EditingUser = new User();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand(CanExecute = nameof(CanDeleteUser))]
        private async Task DeleteUser()
        {
            if (SelectedUser == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete {SelectedUser.Name}?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _dataService.DeleteUserAsync(SelectedUser.Id);
                    Users.Remove(SelectedUser);
                    SelectedUser = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private bool CanDeleteUser() => SelectedUser != null;

        partial void OnSelectedUserChanged(User value)
        {
            // When selection changes, update the editing user
            EditingUser = value?.Clone();
        }
    }
}