using System.Collections.Generic;

namespace DemoMvc.Core.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }

        // Navigation Property: One Teacher has Many Courses
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
