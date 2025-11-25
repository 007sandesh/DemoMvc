using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Core.Entities;
using DemoMvc.Infrastructure.Data;

namespace DemoMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LessonsController : ControllerBase
    {
        private readonly LmsDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public LessonsController(LmsDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/courses/{courseId}/lessons
        [HttpGet("/api/courses/{courseId}/lessons")]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessonsByCourse(int courseId)
        {
            // --- ENROLLMENT CHECK ---
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            if (userRole == "Student")
            {
                var student = await _context.Students
                    .Include(s => s.Courses)
                    .FirstOrDefaultAsync(s => s.Email == userEmail);

                if (student == null || !student.Courses.Any(c => c.ID == courseId))
                {
                    return StatusCode(403, "You must be enrolled in this course to view its content.");
                }
            }
            // ------------------------

            var lessons = await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .ToListAsync();

            return Ok(lessons);
        }

        // POST: api/lessons
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<Lesson>> CreateLesson([FromForm] LessonUploadRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No file uploaded.");

            // 1. Create uploads folder if not exists
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // 2. Save file
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            // 3. Save to DB
            var lesson = new Lesson
            {
                Title = request.Title,
                ContentType = request.ContentType, // "Video" or "Document"
                FileUrl = "/uploads/" + uniqueFileName,
                CourseId = request.CourseId
            };

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLessonsByCourse), new { courseId = lesson.CourseId }, lesson);
        }
    }

    public class LessonUploadRequest
    {
        public required string Title { get; set; }
        public required string ContentType { get; set; }
        public int CourseId { get; set; }
        public required IFormFile File { get; set; }
    }
}
