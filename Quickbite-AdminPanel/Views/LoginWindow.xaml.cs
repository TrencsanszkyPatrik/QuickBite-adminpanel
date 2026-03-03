using System.Windows;
using Quickbite_AdminPanel.Services;
using Quickbite_AdminPanel.Models;

namespace Quickbite_AdminPanel.Views
{
    public partial class LoginWindow : Window
    {
        private readonly ApiService _apiService;

        public LoginWindow()
        {
            InitializeComponent();
            _apiService = new ApiService();
            
            // Enter key támogatás
            EmailTextBox.KeyDown += (s, e) => { if (e.Key == System.Windows.Input.Key.Enter) PasswordBox.Focus(); };
            PasswordBox.KeyDown += async (s, e) => { if (e.Key == System.Windows.Input.Key.Enter) await LoginAsync(); };
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await LoginAsync();
        }

        private async Task LoginAsync()
        {
            // Validáció
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowError("Kérlek add meg az email címedet!");
                return;
            }

            if (string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ShowError("Kérlek add meg a jelszavadat!");
                return;
            }

            // UI frissítés
            LoginButton.IsEnabled = false;
            LoadingMessage.Visibility = Visibility.Visible;
            ErrorMessage.Visibility = Visibility.Collapsed;

            try
            {
                var result = await _apiService.LoginAsync(EmailTextBox.Text, PasswordBox.Password);

                if (result != null)
                {
                    if (result.Role == "super_admin")
                    {
                        var superAdminWindow = new SuperAdminWindow(result, _apiService);
                        superAdminWindow.Show();
                        this.Close();
                        return;
                    }

                    if (result.Role == "restaurant_admin")
                    {
                        var restaurantAdminWindow = new RestaurantAdminWindow(result, _apiService);
                        restaurantAdminWindow.Show();
                        this.Close();
                        return;
                    }

                    ShowError("Ehhez a felülethez nincs jogosultságod.");
                    LoginButton.IsEnabled = true;
                    LoadingMessage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ShowError("Hibás email vagy jelszó!");
                    LoginButton.IsEnabled = true;
                    LoadingMessage.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                ShowError($"Kapcsolódási hiba: {ex.Message}");
                LoginButton.IsEnabled = true;
                LoadingMessage.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
