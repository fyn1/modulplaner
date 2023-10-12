using FhModulplaner.Data;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Planning;

public interface IRemoveTimetableFromPlanningCommand
{
    Task RemoveTimetableFromPlanning(Guid openTimetableId);
}

public class RemoveTimetableFromPlanningCommand : IRemoveTimetableFromPlanningCommand
{
    private readonly AppDbContext _db;
    private readonly ILogger<RemoveSemesterFromPlanningCommand> _logger;
    
    public RemoveTimetableFromPlanningCommand(AppDbContext db, ILogger<RemoveSemesterFromPlanningCommand> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task RemoveTimetableFromPlanning(Guid openTimetableId)
    {
        var openTimetable = await _db.OpenTimetables.SingleOrDefaultAsync(s => s.Id == openTimetableId);
        
        if (openTimetable is null)
        {
            _logger.LogWarning($"Timetable with id {openTimetableId} not found");
        }
        
        _db.OpenTimetables.Remove(openTimetable);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation($"Removed timetable {openTimetableId} from timetable {openTimetable.TimetableId}");
    }
}