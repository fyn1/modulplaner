namespace FhModulplaner.Data;

public class OpenTimetableSemester
{
    public Guid Id { get; set; }
    
    public Guid TimetableId { get; set; }
    
    public Guid CourseOfStudyId { get; set; }
    
    public int Semester { get; set; }
    
    public Timetable? Timetable { get; set; }
    
    public CourseOfStudy? CourseOfStudy { get; set; }
}