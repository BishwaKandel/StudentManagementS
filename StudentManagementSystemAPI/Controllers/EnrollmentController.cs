using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystemAPI.Data;
using StudentManagementSystemAPI.Models;
using System.Linq;

namespace StudentManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnrollmentController(AppDbContext context)
        {
            _context = context;
        }

        // Create Enrollment
        [HttpPost]
        public IActionResult CreateEnrollment([FromBody] EnrollmentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var existingEnrollment = _context.Enrollments
                .FirstOrDefault(e => e.StudentID == dto.StudentID && e.CourseID == dto.CourseID);

            if (existingEnrollment != null)
            {
                return Conflict($"The student with ID {dto.StudentID} is already enrolled in the course with ID {dto.CourseID}.");
            }

            var enrollment = new Enrollment
            {
                StudentID = dto.StudentID,
                CourseID = dto.CourseID,
                EnrollmentDate = dto.EnrollmentDate
            };

            _context.Enrollments.Add(enrollment);
            _context.SaveChanges(); 

            return CreatedAtAction("GetCoursesByStudentId", new { studentId = dto.StudentID }, enrollment);
        }

        // Get all courses a student is enrolled in
        [HttpGet("student/{studentId}")]
        public IActionResult GetCoursesByStudentId(int studentId)
        {
            var details = _context.Enrollments
                .Where(e => e.StudentID == studentId)
                .Include(e => e.Course )
                .Include( e=> e.Student) 
                .Select(e => new
                {
                    StudentId = e.StudentID,
                    FirstName = e.Student.FirstName,
                    LastName = e.Student.LastName,
                    DateofBirth = e.Student.DateofBirth,
                    Email = e.Student.Email,
                    CourseId = e.CourseID,
                    CourseName = e.Course.Name,
                    CourseDescription = e.Course.Description,
                    EnrollmentDate = e.EnrollmentDate
                })
                .ToList();

            if (!details.Any())
            {
                return NotFound($"No courses found for student ID {studentId}.");
            }

            return Ok(details);
        }
    }
}
