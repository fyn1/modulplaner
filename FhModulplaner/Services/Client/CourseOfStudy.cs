namespace FhModulplaner.Services.Client
{
    public class CourseOfStudy
    {
        public string Sname { get; set; }
        public string? Name { get; set; }
        public string? Po { get; set; }
        public List<Grade> Grades { get; set; }
    }
}
