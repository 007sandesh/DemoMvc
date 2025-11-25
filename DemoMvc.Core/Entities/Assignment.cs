using System.ComponentModel.DataAnnotations;

namespace DemoMvc.Core.Entities
{
    public class Assignment
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        // Foreign Key
        public int CourseId { get; set; }
        public Course Course { get; set; }

        // Navigation Property
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
