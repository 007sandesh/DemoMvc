using System;

namespace DemoMvc.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }  // We NEVER store plain passwords
        public string Role { get; set; }  // "Admin", "Teacher", "Student"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}