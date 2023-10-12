using FhModulplaner.Data;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Planning;

public interface IRemoveLessonFromTimetableCommand
{
    Task RemoveLessonFromTimetable(Guid timetableId, Guid lessonId);
}

public class RemoveLessonFromTimetableCommand : IRemoveLessonFromTimetableCommand
{
    private readonly AppDbContext _db;
    private readonly ILogger<AddSemesterToPlanningCommand> _logger;
    
    public RemoveLessonFromTimetableCommand(AppDbContext db, ILogger<AddSemesterToPlanningCommand> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task RemoveLessonFromTimetable(Guid timetableId, Guid lessonId)
    {
        var timetable = await _db.Timetables
            .Include(t => t.Lessons)
            .SingleOrDefaultAsync(t => t.Id == timetableId);

        if (timetable is null)
        {
            throw new ArgumentException($"Timetable with id {timetableId} not found");
        }
        
        var lesson = timetable.Lessons.SingleOrDefault(l => l.Id == lessonId);
        
        if (lesson is null)
        {
            throw new ArgumentException($"Lesson with id {lessonId} not found");
        }

        timetable.Lessons.Remove(lesson);

        await _db.SaveChangesAsync();

        _logger.LogInformation($"Removed lesson {lessonId} from timetable {timetableId}");
    }
    
}