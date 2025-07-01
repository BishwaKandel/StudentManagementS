using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystemAPI.Models;
using StudentManagementSystemAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace StudentManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CoursesController(AppDbContext context)
        {
            _context = context;
        }

        // Create Course
        [HttpPost]
        public IActionResult CreateCourse([FromBody] Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existingCourse = _context.Courses
    .FirstOrDefault(c => c.Name == course.Name);


            if (existingCourse != null)
            {
                return Conflict($"A course with the name '{course.Name}' already exists.");
            }

            _context.Courses.Add(course);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetCourseByID), new { id = course.Id }, course);
        }

        // Get Course by ID
        [HttpGet("{id}")]
        public IActionResult GetCourseByID(int id)
        {
            var course = _context.Courses.Find(id);

            if (course == null)
            {
                return NotFound($"The course with ID {id} was not found.");
            }

            return Ok(course);
        }

        // Get All Courses
        [HttpGet]
        public IActionResult GetAllCourses()
        {
            var courses = _context.Courses.ToList();

            if (courses == null || !courses.Any())
            {
                return NotFound("No courses found.");
            }

            return Ok(courses);
        }

        // Update Course Details
        [HttpPut("{id}")]
        public IActionResult UpdateCourses(int id, [FromBody] Course updatedCourse)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the course exists
            var courseInDb = _context.Courses.Find(id);

            if (courseInDb == null)
            {
                return NotFound($"The course with ID {id} was not found.");
            }

            if (_context.Courses.Any(c => c.Name.ToLower() == updatedCourse.Name.ToLower() && c.Id != id))
            {
                return Conflict($"A course with the name '{updatedCourse.Name}' already exists.");
            }

            {

                courseInDb.Name = updatedCourse.Name;
                courseInDb.Description = updatedCourse.Description;

                _context.SaveChanges();

                return Ok(courseInDb);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCourses(int id)
        {
            var courseInDb = _context.Courses.Find(id);

            if (courseInDb == null)
            {
                return NotFound($"The course with ID {id} was not found.");
            }

            _context.Courses.Remove(courseInDb);
            _context.SaveChanges();

            return Ok(courseInDb);
        }
    }
}
