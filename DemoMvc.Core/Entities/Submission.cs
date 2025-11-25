using System.ComponentModel.DataAnnotations;

namespace DemoMvc.Core.Entities
{
    public class Submission
    {
        public int Id { get; set; }

        [Required]
        public string FileUrl { get; set; } // Path to uploaded file

        public DateTime SubmissionDate { get; set; } = DateTime.Now;

        // Grading (Nullable because it's not graded yet)
        public int? Grade { get; set; } // 0-100
        public string? Feedback { get; set; }

        // Foreign Keys
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}
