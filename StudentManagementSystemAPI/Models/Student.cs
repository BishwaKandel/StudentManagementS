using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystemAPI.Models
{
    public class Student
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First name can only contain letters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last name can only contain letters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        public DateTime DateofBirth { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        public string Email { get; set; }

        // Navigation to Enrollment
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
