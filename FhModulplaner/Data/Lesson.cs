namespace FhModulplaner.Data
{
    public class Lesson
    {
        public Guid Id { get; set; }
        
        public LessonType LessonType { get; set; }
        
        public string StudentSet { get; set; } = string.Empty;
        
        public string Room { get; set; } = string.Empty;
        
        public DateTime TimeBegin { get; set; } 
        
        public DateTime TimeEnd { get; set; }
        
        public int TimeSlotBegin { get; set; }
        
        public int TimeSlotDuration { get; set;}
        
        public DayOfWeek Weekday { get; set; } 
        
        public Guid CourseId { get; set; }
        
        public Course? Course { get; set; }
        
        public ICollection<Timetable> Timetables { get; set; }

    }

    public enum LessonType
    {
        Lecture,
        Exercise,
        Practise,
        SeminarLecture,
        Seminar,
        ProjectWeek,
        QualityOfLearning,
        Tutorium,
        Organisation,
    }
}

