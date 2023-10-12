using FhModulplaner.Data;

namespace FhModulplaner.Models.Timetable;

public class PlanningViewModel
{
    public Guid TimetableId { get; set; }
    
    public DayOfWeek OpenDayOfWeek { get; set; }
    public List<CourseOfStudyDto> CoursesOfStudy { get; set; } = new List<CourseOfStudyDto>();
    
    public List<TimetableDto> TimetablesFromUsers { get; set; } = new List<TimetableDto>();

    public List<LessonDto> PlannedLessons { get; set; } = new List<LessonDto>();

    public List<OpenTimetableSemesterDto> OpenSemesters { get; set; } = new List<OpenTimetableSemesterDto>();
    
    public List<TimetableDto> OpenTimetables { get; set; } = new List<TimetableDto>();
}

public class CourseOfStudyDto
{
    public Guid Id { get; set; }
    
    public string ShortName { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
}

public class OpenTimetableSemesterDto
{
    public Guid Id { get; set; }
    
    public Guid CourseOfStudyId { get; set; }
    
    public string CourseOfStudyShortName { get; set; } = string.Empty;
    
    public string CourseOfStudyName { get; set; } = string.Empty;
    
    public int Semester { get; set; }
    
    public List<List<LessonDto>> Lessons { get; set; } = new List<List<LessonDto>>();
}

public class TimetableDto
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
}

public class LessonDto
{
    public static readonly LessonDto Empty = new LessonDto();
    public Guid Id { get; set; }
    
    public LessonType LessonType { get; set; }
    
    public int TimeSlotBegin { get; set; }
    
    public int TimeSlotDuration { get; set; }
    
    public string CourseName { get; set; } = string.Empty;

    public string TrimmedCourseName { get; set; } = string.Empty;
    
    public DayOfWeek Weekday { get; set; } 

    public string TypeSymbol => LessonType switch
    {
        LessonType.Lecture => "V",
        LessonType.Exercise => "Ü",
        LessonType.Practise => "P",
        LessonType.SeminarLecture => "SL",
        LessonType.Seminar => "S",
        LessonType.ProjectWeek => "PW",
        LessonType.QualityOfLearning => "QoL",
        LessonType.Tutorium => "T",
        _ => string.Empty,
    };
}