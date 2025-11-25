using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DemoMvc.Core.Entities;
using DemoMvc.Core.Interfaces;

namespace DemoMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // All endpoints require authentication
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _repository;

        // Constructor Injection! We ask for the Repository here.
        public StudentsController(IStudentRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            var students = await _repository.GetAllAsync();
            return Ok(students);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]  // Only Admin can create students
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            await _repository.AddAsync(student);
            return CreatedAtAction(nameof(GetStudents), new { id = student.Id }, student);
        }

        [HttpPost("{studentId}/enroll/{courseId}")]
        [Authorize(Roles = "Admin,Teacher")]  // Admin or Teacher can enroll students
        public async Task<IActionResult> EnrollStudent(int studentId, int courseId)
        {
            try
            {
                await _repository.EnrollStudentAsync(studentId, courseId);
                return Ok("Student enrolled successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}