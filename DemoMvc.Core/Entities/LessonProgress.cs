using System.ComponentModel.DataAnnotations;

namespace DemoMvc.Core.Entities
{
    public class LessonProgress
    {
        public int Id { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedDate { get; set; }

        // Foreign Keys
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}
