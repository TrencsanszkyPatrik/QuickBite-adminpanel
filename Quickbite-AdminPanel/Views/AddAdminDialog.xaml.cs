using System.Windows;
using System.Windows.Controls;
using Quickbite_AdminPanel.Models;
using Quickbite_AdminPanel.Services;

namespace Quickbite_AdminPanel.Views
{
    public partial class AddAdminDialog : Window
    {
        private readonly ApiService _apiService;
        private List<RestaurantBasicInfo> _restaurants = new();

        public AddAdminDialog(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            Loaded += Window_Loaded;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Éttermek betöltése
            var restaurants = await _apiService.GetRestaurantsAsync();
            if (restaurants != null)
            {
                _restaurants = restaurants;
                RestaurantComboBox.ItemsSource = _restaurants;
                if (_restaurants.Any())
                {
                    RestaurantComboBox.SelectedIndex = 0;
                }
            }
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RestaurantSection == null) return;

            var selectedRole = ((ComboBoxItem)RoleComboBox.SelectedItem)?.Tag?.ToString();
            
            // Restaurant section csak restaurant_admin esetén látszik
            RestaurantSection.Visibility = selectedRole == "restaurant_admin" 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Validáció
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ShowError("Kérlek add meg a nevet!");
                return;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowError("Kérlek add meg az email címet!");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowError("Kérlek add meg a jelszót!");
                return;
            }

            if (PasswordBox.Password.Length < 6)
            {
                ShowError("A jelszónak legalább 6 karakter hosszúnak kell lennie!");
                return;
            }

            // Szerepkör kiválasztása
            var selectedRole = ((ComboBoxItem)RoleComboBox.SelectedItem).Tag.ToString();

            // Restaurant admin esetén étterem validálása
            if (selectedRole == "restaurant_admin" && RestaurantComboBox.SelectedValue == null)
            {
                ShowError("Kérlek válassz éttermet!");
                return;
            }

            try
            {
                var request = new CreateAdminRequest
                {
                    Name = NameTextBox.Text.Trim(),
                    Email = EmailTextBox.Text.Trim(),
                    Password = PasswordBox.Password,
                    Role = selectedRole!,
                    RestaurantIds = selectedRole == "restaurant_admin" && RestaurantComboBox.SelectedValue != null
                        ? new List<int> { (int)RestaurantComboBox.SelectedValue }
                        : null
                };

                var result = await _apiService.CreateAdminAsync(request);

                if (result != null)
                {
                    DialogResult = true;
                    Close();
                }
                else
                {
                    ShowError("Hiba az admin létrehozásakor. Lehet, hogy az email már használatban van.");
                }
            }
            catch (Exception ex)
            {
                ShowError($"Hiba: {ex.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}
