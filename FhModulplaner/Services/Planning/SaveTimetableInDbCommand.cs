using FhModulplaner.Data;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Planning;

public interface ISaveTimetableInDbCommand
{
    Task SaveTimetableInDb(Guid timetableId, string timetableName);
}

public class SaveTimetableInDbCommand : ISaveTimetableInDbCommand
{
    private readonly AppDbContext _db;
    private readonly ILogger<SaveTimetableInDbCommand> _logger;
    
    public SaveTimetableInDbCommand(AppDbContext db, ILogger<SaveTimetableInDbCommand> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task SaveTimetableInDb(Guid timetableId, string timetableName)
    {
        var timetable = await _db.Timetables.SingleOrDefaultAsync(t => t.Id == timetableId);
        
        if (timetable is null)
        {
            throw new ArgumentException($"Timetable with id {timetableId} not found");
        }
        
        timetable.Name = timetableName;
        timetable.isInUse = false;

        await _db.SaveChangesAsync();
    }
}