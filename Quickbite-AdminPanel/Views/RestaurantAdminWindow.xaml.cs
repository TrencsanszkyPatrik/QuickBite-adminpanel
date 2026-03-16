using System.Windows;
using System.Windows.Controls;
using Quickbite_AdminPanel.Models;
using Quickbite_AdminPanel.Services;

namespace Quickbite_AdminPanel.Views
{
    public partial class RestaurantAdminWindow : Window
    {
        private readonly AdminLoginResponse _currentUser;
        private readonly ApiService _apiService;
        private List<RestaurantAdminRestaurantItem> _restaurants = new();
        private List<RestaurantAdminMenuItem> _menuItems = new();

        public RestaurantAdminWindow(AdminLoginResponse user, ApiService apiService)
        {
            InitializeComponent();
            _currentUser = user;
            _apiService = apiService;
            UserNameText.Text = _currentUser.Name;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadRestaurantsAsync();
        }

        private async Task LoadRestaurantsAsync()
        {
            try
            {
                var restaurants = await _apiService.GetMyRestaurantsAsync();
                if (restaurants == null || restaurants.Count == 0)
                {
                    var ids = _currentUser.RestaurantIds != null && _currentUser.RestaurantIds.Any()
                        ? string.Join(", ", _currentUser.RestaurantIds)
                        : "nincs";
                    ShowStatus($"Nincs betölthető hozzárendelt étterem. Login token RestaurantIds: {ids}");
                    SaveButton.IsEnabled = false;
                    return;
                }

                _restaurants = restaurants;
                RestaurantSelector.ItemsSource = _restaurants;
                RestaurantSelector.SelectedIndex = 0;
                SaveButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ShowStatus($"Hiba az éttermek betöltésekor: {ex.Message}");
                SaveButton.IsEnabled = false;
            }
        }

        private async void RestaurantSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RestaurantSelector.SelectedItem is not RestaurantAdminRestaurantItem selected)
                return;

            NameTextBox.Text = selected.name;
            AddressTextBox.Text = selected.address;
            CityTextBox.Text = selected.city;
            PhoneTextBox.Text = selected.phonenumber;
            OpeningTimeTextBox.Text = NormalizeTime(selected.openingTime);
            ClosingTimeTextBox.Text = NormalizeTime(selected.closingTime);
            DiscountTextBox.Text = selected.discount.ToString();
            FreeDeliveryCheckBox.IsChecked = selected.freeDelivery;
            AcceptCardsCheckBox.IsChecked = selected.acceptCards;
            DescriptionTextBox.Text = selected.description;
            DescriptionLongTextBox.Text = selected.descriptionLong;
            StatusText.Visibility = Visibility.Collapsed;

            await LoadMenuForSelectedRestaurantAsync(selected.id);
        }

        private async Task LoadMenuForSelectedRestaurantAsync(int restaurantId)
        {
            try
            {
                MenuStatusText.Visibility = Visibility.Collapsed;
                var menuItems = await _apiService.GetRestaurantMenuItemsAsync(restaurantId);

                if (menuItems == null)
                {
                    _menuItems = new List<RestaurantAdminMenuItem>();
                    MenuItemsGrid.ItemsSource = _menuItems;
                    ShowMenuStatus("Nem sikerült lekérni az étlapot.");
                    return;
                }

                _menuItems = menuItems;
                MenuItemsGrid.ItemsSource = _menuItems;

                if (_menuItems.Count == 0)
                {
                    ShowMenuStatus("Ehhez az étteremhez nincs elérhető étlapelem.", false);
                }
            }
            catch (Exception ex)
            {
                _menuItems = new List<RestaurantAdminMenuItem>();
                MenuItemsGrid.ItemsSource = _menuItems;
                ShowMenuStatus($"Hiba az étlap betöltésekor: {ex.Message}");
            }
        }

        private async void RefreshMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (RestaurantSelector.SelectedItem is not RestaurantAdminRestaurantItem selected)
            {
                ShowMenuStatus("Nincs kiválasztott étterem.");
                return;
            }

            await LoadMenuForSelectedRestaurantAsync(selected.id);
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (RestaurantSelector.SelectedItem is not RestaurantAdminRestaurantItem selected)
            {
                ShowStatus("Nincs kiválasztott étterem.");
                return;
            }

            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(AddressTextBox.Text) ||
                string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                ShowStatus("A név, cím és város mezők kitöltése kötelező.");
                return;
            }

            if (!int.TryParse(DiscountTextBox.Text, out var discountValue) || discountValue < 0 || discountValue > 100)
            {
                ShowStatus("A kedvezmény 0 és 100 közötti szám lehet.");
                return;
            }

            if (!IsValidTime(OpeningTimeTextBox.Text) || !IsValidTime(ClosingTimeTextBox.Text))
            {
                ShowStatus("A nyitási és zárási idő HH:mm formátumú legyen.");
                return;
            }

            SaveButton.IsEnabled = false;

            try
            {
                var request = new RestaurantAdminUpdateRequest
                {
                    Name = NameTextBox.Text.Trim(),
                    Address = AddressTextBox.Text.Trim(),
                    City = CityTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    DescriptionLong = DescriptionLongTextBox.Text.Trim(),
                    Discount = discountValue,
                    FreeDelivery = FreeDeliveryCheckBox.IsChecked ?? false,
                    AcceptCards = AcceptCardsCheckBox.IsChecked ?? false,
                    CuisineId = selected.cuisineId,
                    OpeningTime = OpeningTimeTextBox.Text.Trim(),
                    ClosingTime = ClosingTimeTextBox.Text.Trim(),
                    PhoneNumber = PhoneTextBox.Text.Trim()
                };

                var success = await _apiService.UpdateMyRestaurantAsync(selected.id, request);
                if (!success)
                {
                    ShowStatus("Nem sikerült menteni az étterem adatait.");
                    SaveButton.IsEnabled = true;
                    return;
                }

                ShowStatus("Étterem adatai sikeresen mentve.", false);
                await LoadRestaurantsAsync();
            }
            catch (Exception ex)
            {
                ShowStatus($"Mentési hiba: {ex.Message}");
            }
            finally
            {
                SaveButton.IsEnabled = true;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            Close();
        }

        private void ShowStatus(string message, bool isError = true)
        {
            StatusText.Text = message;
            StatusText.Foreground = isError
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.Green;
            StatusText.Visibility = Visibility.Visible;
        }

        private void ShowMenuStatus(string message, bool isError = true)
        {
            MenuStatusText.Text = message;
            MenuStatusText.Foreground = isError
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.Green;
            MenuStatusText.Visibility = Visibility.Visible;
        }

        private static bool IsValidTime(string value)
        {
            return TimeSpan.TryParse(value, out _);
        }

        private static string NormalizeTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            if (TimeSpan.TryParse(value, out var time))
                return $"{time.Hours:D2}:{time.Minutes:D2}";

            return value;
        }
    }
}
