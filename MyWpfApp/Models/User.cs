using CommunityToolkit.Mvvm.ComponentModel;

namespace MyWpfApp.Models
{
    public class User : ObservableObject
    {
        private int _id;
        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        // Clone method for editing
        public User Clone()
        {
            return new User
            {
                Id = this.Id,
                Name = this.Name,
                Email = this.Email,
                Phone = this.Phone
            };
        }
    }
}