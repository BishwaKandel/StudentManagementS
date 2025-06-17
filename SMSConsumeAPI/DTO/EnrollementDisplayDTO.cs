using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SMSConsumeAPI.DTO
{
    public class EnrollmentDisplayDTO
    {
        [Required(ErrorMessage = "Course ID is required")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        public string CourseName { get; set; } = string.Empty;

        public string CourseDescription { get; set; } = string.Empty;

        public DateTime EnrollmentDate { get; set; }
    }
}
