namespace FhModulplaner.Data
{
    public class Course
    {
        public Guid Id { get; set; }

        /// <summary>
        /// External course id from the FH Dortmund 
        /// </summary>
        public string FhCourseId { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;
        
        public int Semester { get; set; } 
        
        public Guid CourseOfStudyId { get; set; }
        
        public CourseOfStudy? CourseOfStufy { get; set; }
        
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
