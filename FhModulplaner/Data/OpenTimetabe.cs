namespace FhModulplaner.Data;

public class OpenTimetable
{
    public Guid Id { get; set; }
    
    public Guid TimetableId { get; set; }
    
    public Guid OpendTimetableId { get; set; }
    
    public Timetable? Timetable { get; set; }
    
    public Timetable? OpendTimetable { get; set; }
    
}