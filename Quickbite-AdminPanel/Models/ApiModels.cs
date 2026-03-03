namespace Quickbite_AdminPanel.Models
{
    using Newtonsoft.Json;

    public class AdminLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AdminLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<int>? RestaurantIds { get; set; }
    }

    public class AdminListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<RestaurantBasicInfo>? Restaurants { get; set; }
    }

    public class RestaurantBasicInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateAdminRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "restaurant_admin";
        public List<int>? RestaurantIds { get; set; }
    }

    public class DashboardStats
    {
        public int TotalRestaurants { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TodayOrders { get; set; }
        public decimal TodayRevenue { get; set; }
    }

    public class RestaurantListItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int AdminId { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRestaurantRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int AdminId { get; set; }
    }

    public class UpdateRestaurantRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public int AdminId { get; set; }
    }

    public class RestaurantAdminRestaurantItem
    {
        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;

        [JsonProperty("description_long")]
        public string descriptionLong { get; set; } = string.Empty;

        public int discount { get; set; }

        [JsonProperty("free_delivery")]
        public bool freeDelivery { get; set; }

        [JsonProperty("accept_cards")]
        public bool acceptCards { get; set; }

        [JsonProperty("cuisine_id")]
        public int cuisineId { get; set; }

        [JsonProperty("opening_time")]
        public string openingTime { get; set; } = string.Empty;

        [JsonProperty("closing_time")]
        public string closingTime { get; set; } = string.Empty;

        public string phonenumber { get; set; } = string.Empty;
    }

    public class RestaurantAdminUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DescriptionLong { get; set; } = string.Empty;
        public int Discount { get; set; }
        public bool FreeDelivery { get; set; }
        public bool AcceptCards { get; set; }
        public int CuisineId { get; set; }
        public string OpeningTime { get; set; } = string.Empty;
        public string ClosingTime { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
