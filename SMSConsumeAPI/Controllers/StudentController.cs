using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SMSConsumeAPI.Models;
using System.Data;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SMSConsumeAPI.Controllers
{
    public class StudentController : Controller
    {
        private readonly HttpClient client;
        public StudentController(IHttpClientFactory httpClientFactory)
        {
            client = httpClientFactory.CreateClient("StudentApi");
        }

        public async Task<IActionResult> Index()
        {
            //Calling the Web API and populating the data in view using Model Class
            IList<Student> student = new List<Student>();

                HttpResponseMessage getData = await client.GetAsync("api/Students");

            if (getData.IsSuccessStatusCode)
            {
                string results = getData.Content.ReadAsStringAsync().Result;
                student = JsonConvert.DeserializeObject<List<Student>>(results);
            }
            else
            {
                Console.WriteLine("Error calling Web API");
            }
                ViewData.Model = student;

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddStudent(Student student)
        {
            if (ModelState.IsValid)
            {
                 var json = JsonConvert.SerializeObject(student);
                 var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");


                 HttpResponseMessage response = await client.PostAsync("api/Students", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError(string.Empty, "A student with this email already exists.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please try again later.");
                }
            }
            return View(student);
        }

        // GET: /Student/AddStudent
        [HttpGet]
        public IActionResult AddStudent()
        {
            return View();
        }

        //Get to show Edit form 
        [HttpGet]

        public async Task<IActionResult> Edit(int id)
        {
            Student student = new Student();
           
                HttpResponseMessage response = await client.GetAsync($"api/Students/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    student = JsonConvert.DeserializeObject<Student>(result);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error. Please try again later.");
                }
            return View(student);
        }

        [HttpPost]

        public async Task<IActionResult> Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                    var json = JsonConvert.SerializeObject(student);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync($"api/Students/{student.ID}", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server error. Please try again later.");
                    }
            }
            return View(student);
        }


        // GET: Student/DeleteStudent/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage getData = await client.GetAsync($"api/Students/{id}");
            if (getData.IsSuccessStatusCode)
            {
                string results = await getData.Content.ReadAsStringAsync();
                Student student = JsonConvert.DeserializeObject<Student>(results);
                return View(student);
            }

            return NotFound();
        }

        //For Delete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteStudentConfirmed(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"api/Students/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Failed to delete the student.");
            return View();
        }


    }

}

