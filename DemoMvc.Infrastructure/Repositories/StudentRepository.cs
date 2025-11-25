using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DemoMvc.Core.Entities;
using DemoMvc.Core.Interfaces;
using DemoMvc.Infrastructure.Data;

namespace DemoMvc.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly LmsDbContext _context;

        public StudentRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students.Include(s => s.Courses).ToListAsync();
        }

        public async Task<Student?>GetByIdAsync(int id)
        {
            return await _context.Students.Include(s => s.Courses)
                                          .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddAsync(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
        public async Task EnrollStudentAsync(int studentId, int courseId)
        {
            // 1. Get the student (including their existing courses)
            var student = await _context.Students
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            // 2. Get the course
            var course = await _context.Courses.FindAsync(courseId);

            // 3. Validation
            if (student == null || course == null)
            {
                throw new Exception("Student or Course not found");
            }

            // 4. Add the course to the student's list
            if (!student.Courses.Contains(course))
            {
                student.Courses.Add(course);
                await _context.SaveChangesAsync();
            }
        }
    }
}