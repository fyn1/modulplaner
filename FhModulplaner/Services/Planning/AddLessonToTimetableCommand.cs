using FhModulplaner.Data;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Planning;

public interface IAddLessonToTimetableCommand
{
    Task AddLessonToTimetable(Guid timetableId, Guid lessonId);
}

public class AddLessonToTimetableCommand : IAddLessonToTimetableCommand
{
    private readonly AppDbContext _db;
    private readonly ILogger<AddSemesterToPlanningCommand> _logger;
    private readonly ITimeslotConverter _timeslotConverter;
    
    public AddLessonToTimetableCommand(AppDbContext db, ILogger<AddSemesterToPlanningCommand> logger, ITimeslotConverter timeslotConverter)
    {
        _db = db;
        _logger = logger;
        _timeslotConverter = timeslotConverter;
    }
    
   
        
    public async Task AddLessonToTimetable(Guid timetableId, Guid lessonId)
    {
        var timetable = await _db.Timetables.Include(t => t.Lessons).SingleOrDefaultAsync(t => t.Id == timetableId);
        var lesson = await _db.Lessons.SingleOrDefaultAsync(l => l.Id == lessonId);
        
        if (timetable is null)
        {
            throw new ArgumentException($"Timetable with id {timetableId} not found");
        }
        
        if (lesson is null)
        {
            throw new ArgumentException($"Lesson with id {lessonId} not found");
        }
        
        var lessonsIntimetable = timetable.Lessons;
        
        if (lessonsIntimetable.Any(l => l.Id == lessonId))
        {
            return;
            throw new ArgumentException($"Lesson with id {lessonId} already in timetable with id {timetableId}");
        }
        
        var isLessonOverlapping = lessonsIntimetable
            .Where(l => l.Weekday == lesson.Weekday)
            .Any(l => _timeslotConverter.IsLessonInTimeSlot(l.TimeSlotBegin, l.TimeSlotDuration, lesson.TimeSlotBegin, lesson.TimeSlotDuration));
        
        if (isLessonOverlapping)
        {
            return;
            throw new ArgumentException($"Lesson with id {lessonId} is overlapping with another lesson in timetable with id {timetableId}");
        }
        
        timetable.Lessons.Add(lesson);

        await _db.SaveChangesAsync();

        _logger.LogInformation($"Added lesson {lessonId} to timetable {timetableId}");
    }
}