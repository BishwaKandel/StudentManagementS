
namespace SMSConsumeAPI.Models.ViewModels
{
    public class StudentDeleteViewModel
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateofBirth { get; set; }

        public List<EnrollmentDisplayViewModel> Enrollments { get; set; } = new();
    }

    public class EnrollmentDisplayViewModel
    {
        public string CourseName { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
