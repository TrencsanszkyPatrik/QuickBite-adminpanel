using System.Windows;
using Quickbite_AdminPanel.Models;
using Quickbite_AdminPanel.Services;

namespace Quickbite_AdminPanel.Views
{
    public partial class AddRestaurantDialog : Window
    {
        private readonly ApiService _apiService;

        public AddRestaurantDialog(ApiService apiService)
        {
            InitializeComponent();
            _apiService = apiService;
            LoadAdmins();
        }

        private async void LoadAdmins()
        {
            try
            {
                var admins = await _apiService.GetAdminsAsync();
                if (admins != null)
                {
                    AdminComboBox.ItemsSource = admins;
                    if (admins.Count > 0)
                    {
                        AdminComboBox.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Hiba az adminok betöltésekor: {ex.Message}";
                ErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Validáció
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text) ||
                AdminComboBox.SelectedValue == null)
            {
                ErrorMessage.Text = "Kérlek töltsd ki az összes mezőt!";
                ErrorMessage.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                var request = new CreateRestaurantRequest
                {
                    Name = NameTextBox.Text,
                    Email = EmailTextBox.Text,
                    Phone = PhoneTextBox.Text,
                    AdminId = (int)AdminComboBox.SelectedValue
                };

                var result = await _apiService.CreateRestaurantAsync(request);
                
                if (result != null)
                {
                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    ErrorMessage.Text = "Hiba történt az étterem hozzáadása során!";
                    ErrorMessage.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage.Text = $"Hiba: {ex.Message}";
                ErrorMessage.Visibility = Visibility.Visible;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }
}
