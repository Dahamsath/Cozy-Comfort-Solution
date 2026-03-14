namespace CozyComfort.API.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; } 
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "Seller";
    }

    public class LoginRequest
    {
        public string Email { get; set; } 
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        public string Email { get; set; } 
        public string Password { get; set; }
        public string Role { get; set; } = "Seller";
    }

    
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Email { get; set; } 
        public string? Role { get; set; }
        public int Id { get; set; }
    }
}