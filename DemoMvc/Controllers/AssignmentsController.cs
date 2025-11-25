using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Core.Entities;
using DemoMvc.Infrastructure.Data;
using System.Security.Claims;

namespace DemoMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AssignmentsController : ControllerBase
    {
        private readonly LmsDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AssignmentsController(LmsDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/courses/{courseId}/assignments
        [HttpGet("/api/courses/{courseId}/assignments")]
        public async Task<ActionResult<IEnumerable<Assignment>>> GetAssignmentsByCourse(int courseId)
        {
            var assignments = await _context.Assignments
                .Where(a => a.CourseId == courseId)
                .Include(a => a.Submissions) // Include submissions to check if student submitted
                .ToListAsync();

            return Ok(assignments);
        }

        // POST: api/assignments
        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<Assignment>> CreateAssignment(Assignment assignment)
        {
            _context.Assignments.Add(assignment);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAssignmentsByCourse), new { courseId = assignment.CourseId }, assignment);
        }

        // POST: api/assignments/{id}/submit
        [HttpPost("{id}/submit")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<Submission>> SubmitAssignment(int id, [FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Email == userEmail);

            if (student == null) return Unauthorized("Student not found.");

            // 1. Save file
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "submissions");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 2. Save Submission
            var submission = new Submission
            {
                AssignmentId = id,
                StudentId = student.Id,
                FileUrl = "/submissions/" + uniqueFileName,
                SubmissionDate = DateTime.Now
            };

            _context.Submissions.Add(submission);
            await _context.SaveChangesAsync();

            return Ok(submission);
        }

        // GET: api/assignments/{id}/submissions
        [HttpGet("{id}/submissions")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<IEnumerable<Submission>>> GetSubmissions(int id)
        {
            var submissions = await _context.Submissions
                .Where(s => s.AssignmentId == id)
                .Include(s => s.Student)
                .ToListAsync();

            return Ok(submissions);
        }
        
        // POST: api/assignments/submissions/{id}/grade
        [HttpPost("submissions/{id}/grade")]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> GradeSubmission(int id, [FromBody] GradeRequest request)
        {
            var submission = await _context.Submissions.FindAsync(id);
            if (submission == null) return NotFound();

            submission.Grade = request.Grade;
            submission.Feedback = request.Feedback;

            await _context.SaveChangesAsync();
            return Ok(submission);
        }
    }

    public class GradeRequest
    {
        public int Grade { get; set; }
        public string? Feedback { get; set; }
    }
}
