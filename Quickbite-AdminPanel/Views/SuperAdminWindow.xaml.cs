using System.Windows;
using System.Windows.Controls;
using Quickbite_AdminPanel.Models;
using Quickbite_AdminPanel.Services;

namespace Quickbite_AdminPanel.Views
{
    public partial class SuperAdminWindow : Window
    {
        private readonly AdminLoginResponse _currentUser;
        private readonly ApiService _apiService;

        public SuperAdminWindow(AdminLoginResponse user, ApiService apiService)
        {
            InitializeComponent();
            _currentUser = user;
            _apiService = apiService;
            
            UserNameText.Text = _currentUser.Name;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                // Dashboard stats betöltése
                var stats = await _apiService.GetDashboardStatsAsync();
                if (stats != null)
                {
                    TotalRestaurantsText.Text = stats.TotalRestaurants.ToString();
                    TotalAdminsText.Text = stats.TotalAdmins.ToString();
                    TotalOrdersText.Text = stats.TotalOrders.ToString();
                    TotalRevenueText.Text = $"{stats.TotalRevenue:N0} Ft";
                }

                // Adminok listázása
                await LoadAdminsAsync();

                // Éttermek listázása
                await LoadRestaurantsAsync();

                // Felhasználók listázása
                await LoadUsersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az adatok betöltésekor: {ex.Message}", 
                              "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadAdminsAsync()
        {
            try
            {
                var admins = await _apiService.GetAdminsAsync();
                if (admins != null)
                {
                    AdminsDataGrid.ItemsSource = admins;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az adminok betöltésekor: {ex.Message}", 
                              "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddAdminButton_Click(object sender, RoutedEventArgs e)
        {
            var addAdminDialog = new AddAdminDialog(_apiService);
            if (addAdminDialog.ShowDialog() == true)
            {
                // Frissítés
                await LoadDataAsync();
                MessageBox.Show("Admin sikeresen hozzáadva!", 
                              "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void DeleteAdminButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not int adminId)
                return;

            var result = MessageBox.Show("Biztosan törölni szeretnéd ezt az admint?", 
                                        "Megerősítés", 
                                        MessageBoxButton.YesNo, 
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _apiService.DeleteAdminAsync(adminId);
                    if (success)
                    {
                        MessageBox.Show("Admin sikeresen törölve!", 
                                      "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("Hiba történt a törlés során!", 
                                      "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba: {ex.Message}", 
                                  "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Biztosan ki szeretnél jelentkezni?", 
                                        "Megerősítés", 
                                        MessageBoxButton.YesNo, 
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
            }
        }

        // === ÉTTEREM KEZELÉS ===
        private async Task LoadRestaurantsAsync()
        {
            try
            {
                var restaurants = await _apiService.GetRestaurantsAsync();
                if (restaurants != null)
                {
                    RestaurantsDataGrid.ItemsSource = restaurants;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba az éttermek betöltésekor: {ex.Message}", 
                              "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void AddRestaurantButton_Click(object sender, RoutedEventArgs e)
        {
            var addRestaurantDialog = new AddRestaurantDialog(_apiService);
            if (addRestaurantDialog.ShowDialog() == true)
            {
                await LoadDataAsync();
                MessageBox.Show("Étterem sikeresen hozzáadva!", 
                              "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void DeleteRestaurantButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not int restaurantId)
                return;

            var result = MessageBox.Show("Biztosan törölni szeretnéd ezt az éttermet?", 
                                        "Megerősítés", 
                                        MessageBoxButton.YesNo, 
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _apiService.DeleteRestaurantAsync(restaurantId);
                    if (success)
                    {
                        MessageBox.Show("Étterem sikeresen törölve!", 
                                      "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("Hiba történt a törlés során!", 
                                      "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba: {ex.Message}", 
                                  "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void ToggleRestaurantStatusButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not int restaurantId)
                return;

            try
            {
                var success = await _apiService.ToggleRestaurantStatusAsync(restaurantId);
                if (success)
                {
                    MessageBox.Show("Étterem státusza sikeresen megváltoztatva!", 
                                  "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadDataAsync();
                }
                else
                {
                    MessageBox.Show("Hiba történt a státusz módosítása során!", 
                                  "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba: {ex.Message}", 
                              "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // === TAGOK KEZELÉS ===
        private async Task LoadUsersAsync()
        {
            try
            {
                var users = await _apiService.GetUsersAsync();
                if (users != null)
                {
                    UsersDataGrid.ItemsSource = users;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hiba a tagok betöltésekor: {ex.Message}", 
                              "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag is not int userId)
                return;

            var result = MessageBox.Show("Biztosan törölni szeretnéd ezt a tagot?", 
                                        "Megerősítés", 
                                        MessageBoxButton.YesNo, 
                                        MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var success = await _apiService.DeleteUserAsync(userId);
                    if (success)
                    {
                        MessageBox.Show("Tag sikeresen törölve!", 
                                      "Siker", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("Hiba történt a törlés során!", 
                                      "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hiba: {ex.Message}", 
                                  "Hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
