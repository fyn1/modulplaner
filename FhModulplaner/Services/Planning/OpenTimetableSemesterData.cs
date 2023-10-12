using FhModulplaner.Data;
using FhModulplaner.Models.Timetable;

namespace FhModulplaner.Services.Planning;

public class OpenTimetableSemesterData
{
    public Guid Id { get; set; }
    
    public Guid CourseOfStudyId { get; set; }
    
    public string CourseOfStudyShortName { get; set; } = string.Empty;
    
    public string CourseOfStudyName { get; set; } = string.Empty;
    
    public int Semester { get; set; }

    public List<LessonDto> Lessons { get; set; } = new List<LessonDto>();
}

