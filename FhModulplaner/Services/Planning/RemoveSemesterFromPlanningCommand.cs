using FhModulplaner.Data;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Planning;

public interface IRemoveSemesterFromPlanningCommand
{
    Task RemoveOpenTimeTableSemester(Guid openTimetableSemesterId);
}

public class RemoveSemesterFromPlanningCommand : IRemoveSemesterFromPlanningCommand
{
    private readonly AppDbContext _db;
    private readonly ILogger<RemoveSemesterFromPlanningCommand> _logger;

    public RemoveSemesterFromPlanningCommand(AppDbContext db, ILogger<RemoveSemesterFromPlanningCommand> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task RemoveOpenTimeTableSemester(Guid openTimetableSemesterId)
    {
        var openTimetableSemester = await _db.OpenTimetableSemesters.SingleOrDefaultAsync(s => s.Id == openTimetableSemesterId);

        if (openTimetableSemester is null)
        {
            _logger.LogWarning($"Semester with id {openTimetableSemesterId} not found");
        }

        _db.OpenTimetableSemesters.Remove(openTimetableSemester);
        await _db.SaveChangesAsync();

        _logger.LogInformation($"Removed semester {openTimetableSemester.Semester} of course of study {openTimetableSemester.CourseOfStudyId} from timetable {openTimetableSemester.TimetableId}");
    }

}