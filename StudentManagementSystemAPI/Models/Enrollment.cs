﻿using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace StudentManagementSystemAPI.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        // StudentID - Foreign Key from Student ; 1-M relationship
        [Required(ErrorMessage = "Student ID is required")]
        public int StudentID { get; set; }

        // CourseID - Foreign Key from Course ; 1-M Relationship
        [Required(ErrorMessage = "Course ID is required")]
        public int CourseID { get; set; }

        // Navigation Back to Student
        [JsonIgnore] 
        public Student Student { get; set; } = null!;

        // Navigation Back to Course
        [JsonIgnore] 
        public Course Course { get; set; } = null!;

        [JsonProperty("enrollmentDate")]
        [Required(ErrorMessage = "Enrollment date is required")]
        [DataType(DataType.Date, ErrorMessage = "Invalid date format")]
        public DateTime EnrollmentDate { get; set; }


    }
}
