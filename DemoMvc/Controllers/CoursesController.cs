using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DemoMvc.Core.Entities;
using DemoMvc.Core.Interfaces;

namespace DemoMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // All endpoints require authentication
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _repository;

        public CoursesController(ICourseRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var courses = await _repository.GetAllAsync();
            return Ok(courses);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]  // Only Admin or Teacher can create courses
        public async Task<ActionResult<Course>> CreateCourse(Course course)
        {
            await _repository.AddAsync(course);
            return CreatedAtAction(nameof(GetCourses), new { id = course.ID }, course);
        }
    }
}