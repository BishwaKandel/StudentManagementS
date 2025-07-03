using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMSConsumeAPI.ViewModel
{
    public class EnrollmentDisplay
    {

        public int StudentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateofBirth { get; set; }

        public string Email { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        public string CourseName { get; set; }

        public string CourseDescription { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; }

    }
}

