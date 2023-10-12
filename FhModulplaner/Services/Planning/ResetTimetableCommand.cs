using FhModulplaner.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Planning;

public interface IResetTimetableCommand
{
    Task ResetTimetable(Guid timetableId);
}

public class ResetTimetableCommand : IResetTimetableCommand
{
    private readonly AppDbContext _db;
    private readonly ILogger<CreateTimetableCommand> _logger;
    
    public ResetTimetableCommand(AppDbContext db, ILogger<CreateTimetableCommand> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task ResetTimetable(Guid timetableId)
    {
        var timetable = await _db.Timetables
            .Include(t => t.Lessons)
            .SingleOrDefaultAsync(t => t.Id == timetableId);

        if (timetable is null)
        {
            throw new ArgumentException($"Timetable with id {timetableId} not found");
        }
        
        timetable.Lessons.Clear();
        
        await _db.SaveChangesAsync();
        
        _logger.LogInformation($"Reset timetable {timetableId}");
    }


}