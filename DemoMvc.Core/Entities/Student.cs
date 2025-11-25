using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMvc.Core.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string phonenumber { get; set; } =string.Empty;
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        // Relationship: A student can have many courses
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}


