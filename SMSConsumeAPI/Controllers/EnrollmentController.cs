using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using SMSConsumeAPI.DTO;
using SMSConsumeAPI.Models;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;



namespace SMSConsumeAPI.Controllers
{
    public class EnrollmentController : Controller
    {

        private readonly HttpClient client;
        public EnrollmentController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("StudentApi");
        }
        public async Task<IActionResult> Index(int? studentId)
        {
            if (!studentId.HasValue)
            {
                return BadRequest("StudentId is required");
            }

            List<EnrollmentDisplayDTO> enrollments = new List<EnrollmentDisplayDTO>();
            HttpResponseMessage response = await client.GetAsync($"api/Enrollment/student/{studentId.Value}");

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                enrollments = JsonConvert.DeserializeObject<List<EnrollmentDisplayDTO>>(json);
            }
            else
            {

                ModelState.AddModelError(string.Empty, "Error fetching enrollments.");
            }

            return View(enrollments);
        }
        private async Task PopulateCoursesDropdown()
        {
            HttpResponseMessage response = await client.GetAsync("api/Courses");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var courses = JsonConvert.DeserializeObject<List<Course>>(json);
                ViewBag.Courses = new SelectList(courses, "Id", "Name");
            }
            else
            {
                ViewBag.Courses = new SelectList(Enumerable.Empty<Course>(), "Id", "Name");
            }
        }

        private async Task PopulateStudentDropdown()
        {
            HttpResponseMessage response = await client.GetAsync("api/Students");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var students = JsonConvert.DeserializeObject<List<Student>>(json);
                ViewBag.Students = new SelectList(students, "ID", "FirstName");
            }
            else
            {
                ViewBag.Students = new SelectList(Enumerable.Empty<Student>(), "ID", "FirstName");
            }
        }




        [HttpPost]
        public async Task<IActionResult> AddEnrollment(Enrollment enrollment)
        {
            enrollment.EnrollmentDate = DateTime.Now;

            ModelState.Remove("Student");
            ModelState.Remove("Course");

            if (!ModelState.IsValid)
            {
                await PopulateCoursesDropdown();
                await PopulateStudentDropdown();
                return View(enrollment);
            }

            HttpResponseMessage studentResponse = await client.GetAsync($"api/Students/{enrollment.StudentID}");
            if (!studentResponse.IsSuccessStatusCode)
            {
                ModelState.AddModelError("StudentID", $"Student with ID {enrollment.StudentID} does not exist.");
                await PopulateCoursesDropdown();
                await PopulateStudentDropdown();
                return View(enrollment);
            }

            HttpResponseMessage checkDuplicateResponse = await client.GetAsync($"api/Enrollment/student/{enrollment.StudentID}/course/{enrollment.CourseID}");
            if (checkDuplicateResponse.IsSuccessStatusCode)
            {
                var duplicateResult = await checkDuplicateResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(duplicateResult))
                {
                    ModelState.AddModelError(string.Empty, "The student is already enrolled in this course.");
                    await PopulateCoursesDropdown();
                    await PopulateStudentDropdown();
                    return View(enrollment);
                }
            }

            var json = JsonConvert.SerializeObject(enrollment);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Enrollment", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", new { studentId = enrollment.StudentID });
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ModelState.AddModelError(string.Empty, "An enrollment with these details already exists.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Error adding enrollment. Status: {response.StatusCode}, Error: {errorContent}");
            }

            await PopulateCoursesDropdown();
            await PopulateStudentDropdown();
            return View(enrollment);
        }

        [HttpGet]
        public async Task<IActionResult> AddEnrollment()
        {
            await PopulateCoursesDropdown();
            await PopulateStudentDropdown();
            return View(new Enrollment());
        }


        [HttpGet]
        public async Task<IActionResult> SelectStudent()
        {
            List<Student> students = new List<Student>();
            HttpResponseMessage response = await client.GetAsync("api/Students");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                students = JsonConvert.DeserializeObject<List<Student>>(json);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error fetching student list.");
            }

            ViewBag.Students = new SelectList(students, "ID", "FirstName");

            return View();
        }

    }





}