using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DemoMvc.Core.Entities;
using DemoMvc.Core.Interfaces;

namespace DemoMvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // All endpoints require authentication
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepo;

        public TeachersController(ITeacherRepository teacherRepo)
        {
            _teacherRepo = teacherRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> GetAll()
        {
            var teachers = await _teacherRepo.GetAllTeachersAsync();
            return Ok(teachers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> GetById(int id)
        {
            var teacher = await _teacherRepo.GetTeacherByIdAsync(id);
            if (teacher == null) return NotFound();
            return Ok(teacher);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]  // Only Admin can create teachers
        public async Task<ActionResult> Create(Teacher teacher)
        {
            await _teacherRepo.AddTeacherAsync(teacher);
            return CreatedAtAction(nameof(GetById), new { id = teacher.Id }, teacher);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]  // Only Admin can update teachers
        public async Task<ActionResult> Update(int id, Teacher teacher)
        {
            if (id != teacher.Id) return BadRequest();
            await _teacherRepo.UpdateTeacherAsync(teacher);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]  // Only Admin can delete teachers
        public async Task<ActionResult> Delete(int id)
        {
            await _teacherRepo.DeleteTeacherAsync(id);
            return NoContent();
        }
    }
}
