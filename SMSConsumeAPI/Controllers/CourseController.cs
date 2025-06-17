using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SMSConsumeAPI.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SMSConsumeAPI.Controllers
{
    public class CourseController : Controller
    {
        private readonly HttpClient client;

        public CourseController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("StudentApi");
        }

        // GET: Course/Index
        public async Task<IActionResult> Index()
        {
            // Calling the Web API and populating the data in view using Model Class
            IList<Course> courses = new List<Course>();

            HttpResponseMessage getData = await client.GetAsync("api/Courses");

            if (getData.IsSuccessStatusCode)
            {
                string results = await getData.Content.ReadAsStringAsync();
                courses = JsonConvert.DeserializeObject<List<Course>>(results);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error calling Web API to fetch courses.");
            }

            ViewData.Model = courses;
            return View();
        }

        // POST: Course/AddCourse
        [HttpPost]
        public async Task<IActionResult> AddCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage checkDuplicate = await client.GetAsync($"api/Courses?name={course.Name}");
                if (checkDuplicate.IsSuccessStatusCode)
                {
                    var result = await checkDuplicate.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(result) && result.Contains(course.Name))
                    {
                        ModelState.AddModelError("Name", "A course with this name already exists.");
                    }
                }

                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(course);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("api/Courses", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "An error occurred while adding the course.");
                    }
                }
            }

            return View(course);
        }

        // GET: Course/AddCourse
        [HttpGet]
        public IActionResult AddCourse()
        {
            return View();
        }

        // GET: Course/EditCourse/{id}
        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            Course course = new Course();
            HttpResponseMessage getData = await client.GetAsync($"api/Courses/{id}");

            if (getData.IsSuccessStatusCode)
            {
                string results = await getData.Content.ReadAsStringAsync();
                course = JsonConvert.DeserializeObject<Course>(results);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Error calling Web API to fetch course.");
                return NotFound();
            }

            return View(course);
        }

        // POST: Course/EditCourse
        [HttpPost]
        public async Task<IActionResult> EditCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                HttpResponseMessage checkExistence = await client.GetAsync($"api/Courses/{course.Id}");
                if (!checkExistence.IsSuccessStatusCode)
                {
                    ModelState.AddModelError(string.Empty, "Course not found.");
                    return View(course);
                }

                var json = JsonConvert.SerializeObject(course);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"api/Courses/{course.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the course.");
                }
            }

            return View(course);
        }

        // GET: Course/DeleteCourse/{id}
        [HttpGet]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            HttpResponseMessage getData = await client.GetAsync($"api/Courses/{id}");
            if (getData.IsSuccessStatusCode)
            {
                string results = await getData.Content.ReadAsStringAsync();
                Course course = JsonConvert.DeserializeObject<Course>(results);
                return View(course);
            }

            return NotFound();
        }

        // POST: Course/DeleteCourse/{id}
        [HttpPost, ActionName("DeleteCourse")]
        public async Task<IActionResult> DeleteCourseConfirmed(int id)
        {
            HttpResponseMessage getData = await client.GetAsync($"api/Courses/{id}");
            if (!getData.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, "Course not found.");
                return View();
            }

            HttpResponseMessage response = await client.DeleteAsync($"api/Courses/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to delete the course.");
            return View();
        }
    }
}
