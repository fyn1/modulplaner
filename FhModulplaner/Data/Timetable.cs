namespace FhModulplaner.Data
{
    public class Timetable
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public bool isInUse { get; set; }
        
        public DayOfWeek OpendDayOfWeek { get; set; }

        public Guid UserId { get; set; }
        
        public User? User { get; set; }
        
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        
        public ICollection<OpenTimetableSemester> OpenTimetableSemesters { get; set; } = new List<OpenTimetableSemester>();
    }
}
