using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Core.Entities;
using DemoMvc.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DemoMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class ProgressController : ControllerBase
    {
        private readonly LmsDbContext _context;

        public ProgressController(LmsDbContext context)
        {
            _context = context;
        }

        // POST: api/progress/lessons/{lessonId}/toggle
        [HttpPost("lessons/{lessonId}/toggle")]
        public async Task<IActionResult> ToggleLessonCompletion(int lessonId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null) return Unauthorized();

            var progress = await _context.LessonProgresses
                .FirstOrDefaultAsync(p => p.StudentId == student.Id && p.LessonId == lessonId);

            if (progress == null)
            {
                // Create new progress record
                progress = new LessonProgress
                {
                    StudentId = student.Id,
                    LessonId = lessonId,
                    IsCompleted = true,
                    CompletedDate = DateTime.Now
                };
                _context.LessonProgresses.Add(progress);
            }
            else
            {
                // Toggle completion
                progress.IsCompleted = !progress.IsCompleted;
                progress.CompletedDate = progress.IsCompleted ? DateTime.Now : null;
            }

            await _context.SaveChangesAsync();
            return Ok(new { isCompleted = progress.IsCompleted });
        }

        // GET: api/progress/courses/{courseId}
        [HttpGet("courses/{courseId}")]
        public async Task<ActionResult> GetCourseProgress(int courseId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null) return Unauthorized();

            var totalLessons = await _context.Lessons.CountAsync(l => l.CourseId == courseId);
            if (totalLessons == 0) return Ok(new { percentage = 0, completed = 0, total = 0 });

            var completedLessons = await _context.LessonProgresses
                .Where(p => p.StudentId == student.Id && p.IsCompleted && p.Lesson.CourseId == courseId)
                .CountAsync();

            var percentage = (int)((double)completedLessons / totalLessons * 100);

            return Ok(new { percentage, completed = completedLessons, total = totalLessons });
        }

        // GET: api/progress/lessons/{lessonId}
        [HttpGet("lessons/{lessonId}")]
        public async Task<ActionResult> GetLessonProgress(int lessonId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null) return Unauthorized();

            var progress = await _context.LessonProgresses
                .FirstOrDefaultAsync(p => p.StudentId == student.Id && p.LessonId == lessonId);

            return Ok(new { isCompleted = progress?.IsCompleted ?? false });
        }
    }
}
