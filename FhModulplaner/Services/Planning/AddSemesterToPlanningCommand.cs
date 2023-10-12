using FhModulplaner.Data;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Planning;

public interface IAddSemesterToPlanningCommand
{
    Task AddOpenTimeTableSemester(Guid courseOfStudyId, Guid timeTableId, int semester);
}

public class AddSemesterToPlanningCommand : IAddSemesterToPlanningCommand
{
    private readonly AppDbContext _db;
    private readonly ILogger<AddSemesterToPlanningCommand> _logger;

    public AddSemesterToPlanningCommand(AppDbContext db, ILogger<AddSemesterToPlanningCommand> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task AddOpenTimeTableSemester(Guid courseOfStudyId, Guid timeTableId, int semester)
    {
        var courseOfStudy = await _db.CoursesOfStudies.SingleOrDefaultAsync(c => c.Id == courseOfStudyId);

        if (!await _db.Timetables.AnyAsync(t => t.Id == timeTableId))
        {
            throw new ArgumentException($"Timetable with id {timeTableId} not found");
        }
        
        if (courseOfStudy is null)
        {
            throw new ArgumentException($"Course of study with id {courseOfStudyId} not found");
        }

        var openTimetableSemester = new OpenTimetableSemester
        {
            Id = Guid.NewGuid(),
            CourseOfStudyId = courseOfStudyId,
            TimetableId = timeTableId,
            Semester = semester
        };

        _db.OpenTimetableSemesters.Add(openTimetableSemester);
        await _db.SaveChangesAsync();

        _logger.LogInformation($"Added semester {semester} of course of study {courseOfStudy.Name} to timetable {timeTableId}");
    }
}