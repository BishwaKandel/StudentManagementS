using System;
using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystemAPI.Models
{
    public class EnrollmentDTO
    {
        [Required(ErrorMessage = "Student ID is required")]
        public int StudentID { get; set; }

        [Required(ErrorMessage = "Course ID is required")]
        public int CourseID { get; set; }

        [Required(ErrorMessage = "Enrollment date is required")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; }
    }
}
