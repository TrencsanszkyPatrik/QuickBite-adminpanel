using Microsoft.Win32;
using System.IO;
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
        private RestaurantAdminMenuItem? _selectedMenuItem;
        private string? _selectedMenuImagePath;

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
                ClearMenuItemForm();

                if (_menuItems.Count == 0)
                {
                    ShowMenuStatus("Ehhez az étteremhez nincs étlapelem.", false);
                }
            }
            catch (Exception ex)
            {
                _menuItems = new List<RestaurantAdminMenuItem>();
                MenuItemsGrid.ItemsSource = _menuItems;
                ClearMenuItemForm();
                ShowMenuStatus($"Hiba az étlap betöltésekor: {ex.Message}");
            }
        }

        private void MenuItemsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedMenuItem = MenuItemsGrid.SelectedItem as RestaurantAdminMenuItem;
            BindSelectedMenuItemToForm();

            if (_selectedMenuItem != null)
            {
                MenuEditorExpander.IsExpanded = true;
            }
        }

        private async void AddMenuItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (RestaurantSelector.SelectedItem is not RestaurantAdminRestaurantItem selectedRestaurant)
            {
                ShowMenuStatus("Nincs kiválasztott étterem.");
                return;
            }

            if (!TryBuildMenuCreateRequest(selectedRestaurant.id, out var request, out var validationError))
            {
                ShowMenuStatus(validationError);
                return;
            }

            AddMenuItemButton.IsEnabled = false;

            try
            {
                var created = await _apiService.CreateMenuItemAsync(request!);
                if (created == null)
                {
                    ShowMenuStatus("Nem sikerült hozzáadni az étlap tételt.");
                    return;
                }

                ShowMenuStatus("Étlap tétel sikeresen hozzáadva.", false);
                await LoadMenuForSelectedRestaurantAsync(selectedRestaurant.id);
            }
            finally
            {
                AddMenuItemButton.IsEnabled = true;
            }
        }

        private async void UpdateMenuItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMenuItem == null)
            {
                ShowMenuStatus("Válassz ki egy módosítandó tételt.");
                return;
            }

            if (RestaurantSelector.SelectedItem is not RestaurantAdminRestaurantItem selectedRestaurant)
            {
                ShowMenuStatus("Nincs kiválasztott étterem.");
                return;
            }

            if (!TryBuildMenuUpdateRequest(out var request, out var validationError))
            {
                ShowMenuStatus(validationError);
                return;
            }

            UpdateMenuItemButton.IsEnabled = false;

            try
            {
                var updated = await _apiService.UpdateMenuItemAsync(_selectedMenuItem.id, request!);
                if (updated == null)
                {
                    ShowMenuStatus("Nem sikerült módosítani az étlap tételt.");
                    return;
                }

                ShowMenuStatus("Étlap tétel sikeresen módosítva.", false);
                await LoadMenuForSelectedRestaurantAsync(selectedRestaurant.id);
            }
            finally
            {
                UpdateMenuItemButton.IsEnabled = true;
            }
        }

        private async void DeleteMenuItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMenuItem == null)
            {
                ShowMenuStatus("Válassz ki egy törlendő tételt.");
                return;
            }

            if (RestaurantSelector.SelectedItem is not RestaurantAdminRestaurantItem selectedRestaurant)
            {
                ShowMenuStatus("Nincs kiválasztott étterem.");
                return;
            }

            var result = MessageBox.Show(
                $"Biztosan törlöd ezt a tételt: {_selectedMenuItem.name}?",
                "Tétel törlése",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            DeleteMenuItemButton.IsEnabled = false;

            try
            {
                var success = await _apiService.DeleteMenuItemAsync(_selectedMenuItem.id);
                if (!success)
                {
                    ShowMenuStatus("Nem sikerült törölni az étlap tételt.");
                    return;
                }

                ShowMenuStatus("Étlap tétel sikeresen törölve.", false);
                await LoadMenuForSelectedRestaurantAsync(selectedRestaurant.id);
            }
            finally
            {
                DeleteMenuItemButton.IsEnabled = true;
            }
        }

        private void NewMenuItemButton_Click(object sender, RoutedEventArgs e)
        {
            ClearMenuItemForm();
            MenuEditorExpander.IsExpanded = true;
            ShowMenuStatus("Új étlap tétel rögzítéséhez töltsd ki a mezőket.", false);
        }

        private void BrowseMenuImageButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Title = "Menü item kép kiválasztása",
                Filter = "Képfájlok|*.jpg;*.jpeg;*.png;*.gif;*.webp|Minden fájl|*.*"
            };

            if (dialog.ShowDialog() != true)
                return;

            _selectedMenuImagePath = dialog.FileName;
            MenuSelectedImagePathTextBox.Text = _selectedMenuImagePath;
            UpdateUploadButtonState();
            ShowMenuStatus("Kép kiválasztva. Feltöltéshez kattints a 'Kép feltöltése' gombra.", false);
        }

        private async void UploadMenuImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedMenuItem == null)
            {
                ShowMenuStatus("Kép feltöltéséhez előbb válassz ki egy mentett étlap tételt.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_selectedMenuImagePath) || !File.Exists(_selectedMenuImagePath))
            {
                ShowMenuStatus("Válassz ki egy létező képfájlt.");
                return;
            }

            if (RestaurantSelector.SelectedItem is not RestaurantAdminRestaurantItem selectedRestaurant)
            {
                ShowMenuStatus("Nincs kiválasztott étterem.");
                return;
            }

            UploadMenuImageButton.IsEnabled = false;

            try
            {
                var updatedMenuItem = await _apiService.UploadMenuItemImageAsync(_selectedMenuItem.id, _selectedMenuImagePath!);

                MenuImageUrlTextBox.Text = updatedMenuItem.imageUrl ?? string.Empty;
                ShowMenuStatus("Kép sikeresen feltöltve.", false);
                _selectedMenuImagePath = null;
                MenuSelectedImagePathTextBox.Text = string.Empty;
                await LoadMenuForSelectedRestaurantAsync(selectedRestaurant.id);
            }
            catch (Exception ex)
            {
                ShowMenuStatus(ex.Message);
            }
            finally
            {
                UpdateUploadButtonState();
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

        private void BindSelectedMenuItemToForm()
        {
            if (_selectedMenuItem == null)
            {
                UpdateMenuItemButton.IsEnabled = false;
                DeleteMenuItemButton.IsEnabled = false;
                MenuImageUrlTextBox.Text = string.Empty;
                UpdateUploadButtonState();
                return;
            }

            MenuNameTextBox.Text = _selectedMenuItem.name;
            MenuCategoryTextBox.Text = _selectedMenuItem.category ?? string.Empty;
            MenuDescriptionTextBox.Text = _selectedMenuItem.description ?? string.Empty;
            MenuPriceTextBox.Text = _selectedMenuItem.price.ToString();
            MenuIsAvailableCheckBox.IsChecked = _selectedMenuItem.isAvailable;
            MenuImageUrlTextBox.Text = _selectedMenuItem.imageUrl ?? string.Empty;

            UpdateMenuItemButton.IsEnabled = true;
            DeleteMenuItemButton.IsEnabled = true;
            UpdateUploadButtonState();
            MenuStatusText.Visibility = Visibility.Collapsed;
        }

        private void ClearMenuItemForm()
        {
            _selectedMenuItem = null;
            _selectedMenuImagePath = null;
            MenuItemsGrid.SelectedItem = null;
            MenuNameTextBox.Text = string.Empty;
            MenuCategoryTextBox.Text = string.Empty;
            MenuDescriptionTextBox.Text = string.Empty;
            MenuPriceTextBox.Text = string.Empty;
            MenuIsAvailableCheckBox.IsChecked = true;
            MenuSelectedImagePathTextBox.Text = string.Empty;
            MenuImageUrlTextBox.Text = string.Empty;
            UpdateMenuItemButton.IsEnabled = false;
            DeleteMenuItemButton.IsEnabled = false;
            UpdateUploadButtonState();
        }

        private void UpdateUploadButtonState()
        {
            UploadMenuImageButton.IsEnabled = _selectedMenuItem != null && !string.IsNullOrWhiteSpace(_selectedMenuImagePath);
        }

        private bool TryBuildMenuCreateRequest(int restaurantId, out RestaurantAdminCreateMenuItemRequest? request, out string error)
        {
            request = null;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(MenuNameTextBox.Text))
            {
                error = "A név mező kitöltése kötelező.";
                return false;
            }

            if (!int.TryParse(MenuPriceTextBox.Text, out var price) || price < 0)
            {
                error = "Az ár mezőben érvényes, pozitív szám szükséges.";
                return false;
            }

            request = new RestaurantAdminCreateMenuItemRequest
            {
                RestaurantId = restaurantId,
                Name = MenuNameTextBox.Text.Trim(),
                Category = string.IsNullOrWhiteSpace(MenuCategoryTextBox.Text) ? null : MenuCategoryTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(MenuDescriptionTextBox.Text) ? null : MenuDescriptionTextBox.Text.Trim(),
                Price = price,
                IsAvailable = MenuIsAvailableCheckBox.IsChecked ?? true
            };

            return true;
        }

        private bool TryBuildMenuUpdateRequest(out RestaurantAdminUpdateMenuItemRequest? request, out string error)
        {
            request = null;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(MenuNameTextBox.Text))
            {
                error = "A név mező kitöltése kötelező.";
                return false;
            }

            if (!int.TryParse(MenuPriceTextBox.Text, out var price) || price < 0)
            {
                error = "Az ár mezőben érvényes, pozitív szám szükséges.";
                return false;
            }

            request = new RestaurantAdminUpdateMenuItemRequest
            {
                Name = MenuNameTextBox.Text.Trim(),
                Category = string.IsNullOrWhiteSpace(MenuCategoryTextBox.Text) ? null : MenuCategoryTextBox.Text.Trim(),
                Description = string.IsNullOrWhiteSpace(MenuDescriptionTextBox.Text) ? null : MenuDescriptionTextBox.Text.Trim(),
                Price = price,
                IsAvailable = MenuIsAvailableCheckBox.IsChecked ?? true
            };

            return true;
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
