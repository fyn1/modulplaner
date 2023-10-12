using FhModulplaner.Data;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Planning;

public interface IAddTimetableToPlanningCommand
{
    Task AddTimetableToPlanning(Guid timetableId, Guid openTimetableId);
}

public class AddTimetableToPlanningCommand : IAddTimetableToPlanningCommand
{
    private readonly AppDbContext _db;
    private readonly ILogger<AddSemesterToPlanningCommand> _logger;
    
    public AddTimetableToPlanningCommand(AppDbContext db, ILogger<AddSemesterToPlanningCommand> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task AddTimetableToPlanning(Guid timetableId, Guid openTimetableId)
    {
        if (!await _db.Timetables.AnyAsync(t => t.Id == timetableId))
        {
            throw new ArgumentException($"Timetable with id {timetableId} not found");
        }
        
        if (!await _db.Timetables.AnyAsync(t => t.Id == openTimetableId))
        {
            throw new ArgumentException($"Timetable with id {openTimetableId} not found");
        }

        var openTimetable = new OpenTimetable
        {
            Id = Guid.NewGuid(),
            TimetableId = timetableId,
            OpendTimetableId = openTimetableId
        };
        
        _db.OpenTimetables.Add(openTimetable);
        await _db.SaveChangesAsync();

        _logger.LogInformation($"Added timetable {openTimetableId} to timetable {timetableId}");
    }
}