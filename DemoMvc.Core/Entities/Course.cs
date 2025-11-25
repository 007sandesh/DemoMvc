using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMvc.Core.Entities
{
    public class Course
    {
        public int ID { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Credits { get; set; }
        public string Description { get; set; } = string.Empty;

        // Relationship: A course can have many students
        public ICollection<Student> Students { get; set; } = new List<Student>();

        // Relationship: One Course has One Teacher
        // We make it nullable (int?) so we don't break existing courses that have no teacher yet.
        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
    }
}
