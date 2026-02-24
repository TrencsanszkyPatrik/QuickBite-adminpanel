namespace Quickbite_AdminPanel.Models
{
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
}
