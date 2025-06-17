namespace StudentManagementSystemAPI.Models
{
    public class Auth
    {
        public int id { get; set; }
        public string Username { get; set; }

        public string PasswordHash { get; set; }
    }
}
