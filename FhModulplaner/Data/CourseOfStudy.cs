namespace FhModulplaner.Data
{
    public class CourseOfStudy
    {
        public Guid Id { get; set; }
        
        public string ShortName { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public string? Po { get; set; }
        
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        
        public ICollection<OpenTimetableSemester> OpenTimetableSemesters { get; set; } = new List<OpenTimetableSemester>();

    }
}
