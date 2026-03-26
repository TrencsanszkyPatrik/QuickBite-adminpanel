using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
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
                BaseAddress = new Uri("https://quickbite-backend-production-6372.up.railway.app/"),
                Timeout = TimeSpan.FromSeconds(30)
            };
        }

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

        public async Task<List<RestaurantListItem>?> GetRestaurantsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/super-admin/restaurants-with-admins");
                
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

        public async Task<List<RestaurantAdminRestaurantItem>?> GetMyRestaurantsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/restaurant-admin/restaurants");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<RestaurantAdminRestaurantItem>>(json);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"API hiba ({(int)response.StatusCode}): {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Saját éttermek lekérése sikertelen: {ex.Message}");
            }
        }

        public async Task<bool> UpdateMyRestaurantAsync(int restaurantId, RestaurantAdminUpdateRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/restaurant-admin/restaurants/{restaurantId}", content);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<RestaurantAdminMenuItem>?> GetRestaurantMenuItemsAsync(int restaurantId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/restaurant-admin/restaurants/{restaurantId}/menu-items");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<RestaurantAdminMenuItem>>(json);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<RestaurantAdminMenuItem?> CreateMenuItemAsync(RestaurantAdminCreateMenuItemRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/restaurant-admin/menu-items", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<RestaurantAdminMenuItem>(responseJson);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<RestaurantAdminMenuItem?> UpdateMenuItemAsync(int menuItemId, RestaurantAdminUpdateMenuItemRequest request)
        {
            try
            {
                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/restaurant-admin/menu-items/{menuItemId}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<RestaurantAdminMenuItem>(responseJson);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteMenuItemAsync(int menuItemId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/restaurant-admin/menu-items/{menuItemId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<RestaurantAdminMenuItem?> UploadMenuItemImageAsync(int menuItemId, string imagePath)
        {
            try
            {
                using var formData = new MultipartFormDataContent();
                using var fileStream = File.OpenRead(imagePath);
                using var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(GetMediaType(imagePath));
                formData.Add(fileContent, "image", Path.GetFileName(imagePath));

                var response = await _httpClient.PostAsync($"api/restaurant-admin/menu-items/{menuItemId}/upload-image", formData);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<RestaurantAdminMenuItem>(responseJson);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Képfeltöltési API hiba ({(int)response.StatusCode}): {errorContent}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Képfeltöltés sikertelen: {ex.Message}");
            }
        }

        private static string GetMediaType(string filePath)
        {
            return Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
        }

        public async Task<List<UserListItem>?> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/super-admin/users");
                
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UserListItem>>(json);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/super-admin/users/{userId}");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
