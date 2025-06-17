using System.ComponentModel.DataAnnotations;

namespace SMSConsumeAPI.Models
{
    public class Course
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Course name is required")]
        [StringLength(100, ErrorMessage = "Course name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        // Navigation to Enrollment (1 Course -> many Enrollments)
        public ICollection<Enrollment>? Enrollments { get; set; }
    }
}
