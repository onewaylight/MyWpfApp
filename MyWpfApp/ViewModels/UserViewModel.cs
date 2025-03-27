using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyWpfApp.Models;
using MyWpfApp.Services;
using System.Collections.ObjectModel;
using System.Windows;

namespace MyWpfApp.ViewModels
{
    public class UserViewModel : ObservableObject
    {
        private readonly IDataService _dataService;

        // Properties
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                if (SetProperty(ref _selectedUser, value))
                {
                    // When selection changes, update the editing user
                    EditingUser = value?.Clone();
                    DeleteUserCommand.NotifyCanExecuteChanged();
                    EditUserCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private User _editingUser;
        public User EditingUser
        {
            get => _editingUser;
            set => SetProperty(ref _editingUser, value);
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        // Commands
        public RelayCommand LoadUsersCommand { get; }
        public RelayCommand AddUserCommand { get; }
        public RelayCommand<User> SaveUserCommand { get; }
        public RelayCommand CancelEditCommand { get; }
        public RelayCommand EditUserCommand { get; }
        public RelayCommand DeleteUserCommand { get; }

        public UserViewModel(IDataService dataService)
        {
            _dataService = dataService;
            Users = new ObservableCollection<User>();
            EditingUser = new User();

            // Initialize commands
            LoadUsersCommand = new RelayCommand(async () => await LoadUsersAsync());
            AddUserCommand = new RelayCommand(AddUser);
            SaveUserCommand = new RelayCommand<User>(async (user) => await SaveUserAsync(user), (user) => user != null);
            CancelEditCommand = new RelayCommand(CancelEdit);
            EditUserCommand = new RelayCommand(EditUser, () => SelectedUser != null);
            DeleteUserCommand = new RelayCommand(async () => await DeleteUserAsync(), () => SelectedUser != null);

            // Load users when the ViewModel is created
            LoadUsersCommand.Execute(null);
        }

        private async Task LoadUsersAsync()
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

        private void AddUser()
        {
            EditingUser = new User();
            IsEditing = true;
            SelectedUser = null;
        }

        private void EditUser()
        {
            if (SelectedUser != null)
            {
                EditingUser = SelectedUser.Clone();
                IsEditing = true;
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
            EditingUser = new User();
        }

        private async Task SaveUserAsync(User user)
        {
            try
            {
                if (user.Id == 0)
                {
                    // Add new user
                    var addedUser = await _dataService.AddUserAsync(user);
                    Users.Add(addedUser);
                }
                else
                {
                    // Update existing user
                    await _dataService.UpdateUserAsync(user);
                    var existingUserIndex = Users.IndexOf(Users.FirstOrDefault(u => u.Id == user.Id));
                    if (existingUserIndex >= 0)
                    {
                        Users[existingUserIndex] = user;
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

        private async Task DeleteUserAsync()
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
    }
}
