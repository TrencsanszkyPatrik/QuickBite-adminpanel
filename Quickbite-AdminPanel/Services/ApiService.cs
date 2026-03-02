using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Quickbite_AdminPanel.Models;

namespace Quickbite_AdminPanel.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private string? _token;

        public string? Token
        {
            get => _token;
            set
            {
                _token = value;
                if (!string.IsNullOrEmpty(_token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", _token);
                }
                else
                {
                    _httpClient.DefaultRequestHeaders.Authorization = null;
                }
            }
        }

        public ApiService()
        {
            _httpClient = new HttpClient()
            {
                // Uri needs a scheme (http/https) – without it you'll see
                // "localhost scheme is not supported" exceptions when creating
                // the Uri.  Was missing in original code.
                BaseAddress = new Uri("http://localhost:5158/"),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

        // === AUTH ===
        public async Task<AdminLoginResponse?> LoginAsync(string email, string password)
        {
            try
            {
                var request = new AdminLoginRequest { Email = email, Password = password };
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/admin/auth/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<AdminLoginResponse>(responseJson);
                    
                    if (result != null)
                    {
                        Token = result.Token;
                    }
                    
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Login hiba: {ex.Message}");
            }
        }

        // === DASHBOARD ===
        public async Task<DashboardStats?> GetDashboardStatsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/super-admin/dashboard");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DashboardStats>(json);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        // === ADMIN MANAGEMENT ===
        public async Task<List<AdminListItem>?> GetAdminsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/super-admin/admins");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<AdminListItem>>(json);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<AdminListItem?> CreateAdminAsync(CreateAdminRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/super-admin/admins", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<AdminListItem>(responseJson);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteAdminAsync(int adminId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/super-admin/admins/{adminId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // === RESTAURANTS ===
        public async Task<List<RestaurantListItem>?> GetRestaurantsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/super-admin/restaurants");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<RestaurantListItem>>(json);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<RestaurantListItem?> CreateRestaurantAsync(CreateRestaurantRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/super-admin/restaurants", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<RestaurantListItem>(responseJson);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateRestaurantAsync(UpdateRestaurantRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/super-admin/restaurants/{request.Id}", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteRestaurantAsync(int restaurantId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/super-admin/restaurants/{restaurantId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ToggleRestaurantStatusAsync(int restaurantId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"api/super-admin/restaurants/{restaurantId}/toggle-status", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
