using Microsoft.EntityFrameworkCore;
using DemoMvc.Core.Entities;
using DemoMvc.Core.Interfaces;
using DemoMvc.Infrastructure.Data;

namespace DemoMvc.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly LmsDbContext _context;

        public CourseRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses.Include(c => c.Students).ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses.Include(c => c.Students)
                                         .FirstOrDefaultAsync(c => c.ID == id);
        }

        public async Task AddAsync(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
        }
    }
}
