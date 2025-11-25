using System.ComponentModel.DataAnnotations;

namespace DemoMvc.Core.Entities
{
    public class Lesson
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }       // e.g., "Week 1: Introduction"

        [Required]
        public string ContentType { get; set; } // "Video" or "Document"

        [Required]
        public string FileUrl { get; set; }     // URL to video or path to uploaded file

        // Foreign Key
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
