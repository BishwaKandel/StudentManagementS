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


        [HttpPost]
        public async Task<IActionResult> AddEnrollment(Enrollment enrollment)
        {
            enrollment.EnrollmentDate = DateTime.Now;

            ModelState.Remove("Student");
            ModelState.Remove("Course");

            if (!ModelState.IsValid)
            {
                return View(enrollment);
            }

            HttpResponseMessage checkDuplicateResponse = await client.GetAsync($"api/Enrollment/student/{enrollment.StudentID}/course/{enrollment.CourseID}");
            if (checkDuplicateResponse.IsSuccessStatusCode)
            {
                var duplicateResult = await checkDuplicateResponse.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(duplicateResult))
                {
                    ModelState.AddModelError(string.Empty, "The student is already enrolled in this course.");
                    // Repopulate the courses dropdown
                    HttpResponseMessage coursesResponse = await client.GetAsync("api/Courses");
                    if (coursesResponse.IsSuccessStatusCode)
                    {
                        var coursesJson = await coursesResponse.Content.ReadAsStringAsync();
                        var courses = JsonConvert.DeserializeObject<List<Course>>(coursesJson);
                        ViewBag.Courses = new SelectList(courses, "Id", "Name");
                    }
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

            return View(enrollment);
        }

        [HttpGet]
        public async Task<IActionResult> AddEnrollment()
        {
            List<Course> courses = new List<Course>();
            HttpResponseMessage response = await client.GetAsync("api/Courses");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                courses = JsonConvert.DeserializeObject<List<Course>>(json);
                ViewBag.Courses = new SelectList(courses, "Id", "Name");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error fetching courses. Please try again.");
                ViewBag.Courses = new SelectList(Enumerable.Empty<Course>(), "Id", "Name");
            }

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

