using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystemAPI.Models;
using StudentManagementSystemAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace StudentManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // Create STUDENT
        [HttpPost]
        public IActionResult Create([FromBody] Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            var existingStudent = _context.Students
                .FirstOrDefault(s => s.Email == student.Email);

            if (existingStudent != null)
            {
                return Conflict($"A student with email '{student.Email}' already exists.");
            }

            _context.Students.Add(student);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = student.ID }, student);
        }

        // GET THE STUDENT BY ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var student = _context.Students.Find(id);

            if (student == null)
            {
                return NotFound($"Student with ID {id} not found");
            }

            return Ok(student);
        }

        // GET ALL STUDENTS
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            var students = _context.Students.ToList();

            if (students == null || !students.Any())
            {
                return NotFound("No students found.");
            }

            return Ok(students);
        }

        // UPDATE STUDENT INFO BY ID
        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, [FromBody] Student updatedStudent)
        {
            var studentInDb = _context.Students.Find(id);

            if (studentInDb == null)
            {
                return NotFound($"Student with ID {id} not found");
            }

            if (_context.Students.Any(s => s.Email == updatedStudent.Email && s.ID != id))
            {
                return Conflict($"A student with email '{updatedStudent.Email}' already exists.");
            }

            studentInDb.FirstName = updatedStudent.FirstName;
            studentInDb.LastName = updatedStudent.LastName;
            studentInDb.Email = updatedStudent.Email;
            studentInDb.DateofBirth = updatedStudent.DateofBirth;

            _context.SaveChanges();

            return Ok(studentInDb);
        }

        // DELETE STUDENT
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var studentInDb = _context.Students.Find(id);

            if (studentInDb == null)
            {
                return NotFound($"Student with ID {id} not found");
            }

            _context.Students.Remove(studentInDb);
            _context.SaveChanges();

            return Ok(studentInDb);
        }
    }
}
