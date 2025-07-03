
using System.ComponentModel.DataAnnotations;

namespace SMSConsumeAPI.Models.ViewModels
{
    public class CourseDeleteViewModel
    { 
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public List<EnrollmentStudentViewModel> Enrollments { get; set; } = new();
    }

    public class EnrollmentStudentViewModel
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateofBirth { get; set; }

        public DateTime EnrollmentDate { get; set; }
    }
}
