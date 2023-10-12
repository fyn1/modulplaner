using FhModulplaner.Data;

namespace FhModulplaner.Services.Planning;

public interface ICreateTimetableCommand
{
    Task<Guid> CreateTimetable(Guid userId);
}

public class CreateTimetableCommand : ICreateTimetableCommand
{
    private readonly AppDbContext _db;
    private readonly ILogger<CreateTimetableCommand> _logger;

    public CreateTimetableCommand(AppDbContext db, ILogger<CreateTimetableCommand> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task<Guid> CreateTimetable(Guid userId)
    {
        var timetable = new Timetable
        {
            Id = Guid.NewGuid(),
            isInUse = true,
            OpendDayOfWeek = DayOfWeek.Monday,
            UserId = userId
        };
        
        _db.Timetables.Add(timetable);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation($"Created timetable {timetable.Id} for user {userId}");
        
        return timetable.Id;
    }
}