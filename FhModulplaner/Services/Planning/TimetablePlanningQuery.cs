using FhModulplaner.Data;
using FhModulplaner.Models.Timetable;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Planning;

public interface ITimetablePlanningQuery
{
    Task<PlanningViewModel> GetPlanningViewModel(Guid timetableId, DayOfWeek openDayOfWeek);
}

public class TimetablePlanningQuery : ITimetablePlanningQuery
{
    private readonly AppDbContext _db;
    private readonly IOpenTimetableSemesterMapper _openTimetableSemesterMapper;
    private readonly IOpenTimetableMapper _openTimetableMapper;
    private readonly ITimeslotConverter _timeslotConverter;
    private const int TrimCourseNameLength = 18;

    public TimetablePlanningQuery(AppDbContext db, IOpenTimetableSemesterMapper openTimetableSemesterMapper, ITimeslotConverter timeslotConverter, IOpenTimetableMapper openTimetableMapper)
    {
        _db = db;
        _openTimetableSemesterMapper = openTimetableSemesterMapper;
        _timeslotConverter = timeslotConverter;
        _openTimetableMapper = openTimetableMapper;
    }

    public async Task<PlanningViewModel> GetPlanningViewModel(Guid timetableId, DayOfWeek openDayOfWeek)
    {
        var coursesOfStudy = await _db.CoursesOfStudies
            .Select(courseOfStudy => new CourseOfStudyDto
            {
                Id = courseOfStudy.Id,
                ShortName = courseOfStudy.ShortName,
                Name = courseOfStudy.Name,
            })
            .ToListAsync();

        var plannedLessons = await _db.Timetables
            .Where(t => t.Id == timetableId)
            .Select(t => t.Lessons
                .Where(l => l.Weekday == openDayOfWeek)
                .Select(lesson => new LessonDto
            {
                Id = lesson.Id,
                LessonType = lesson.LessonType,
                TimeSlotBegin = lesson.TimeSlotBegin,
                TimeSlotDuration = lesson.TimeSlotDuration,
                CourseName = lesson.Course!.Name,
                TrimmedCourseName = lesson.Course.Name.Substring(0, TrimCourseNameLength),
            })
                .ToList())
            .SingleAsync();
        
        var openTimetableSemestersData = await _db.OpenTimetableSemesters
            .Where(s => s.TimetableId == timetableId)
            .Select(semester => new OpenTimetableSemesterData
            {
                Id = semester.Id,
                CourseOfStudyId = semester.CourseOfStudyId,
                CourseOfStudyShortName = semester.CourseOfStudy!.ShortName,
                CourseOfStudyName = semester.CourseOfStudy.Name,
                Semester = semester.Semester,
                Lessons = semester.CourseOfStudy.Courses
                    .SelectMany(c => c.Lessons)
                    .Where(l => l.Weekday == openDayOfWeek)
                    .Select(lesson => new LessonDto
                    {
                        Id = lesson.Id,
                        LessonType = lesson.LessonType,
                        TimeSlotBegin = lesson.TimeSlotBegin,
                        TimeSlotDuration = lesson.TimeSlotDuration,
                        CourseName = lesson.Course!.Name,
                        TrimmedCourseName = lesson.Course.Name.Substring(0, TrimCourseNameLength),
                    })
                    .ToList(),
            })
            .ToListAsync();

        var openTimetables = await _db.OpenTimetables
            .Where(oT => oT.TimetableId == timetableId)
            .Select(ot => new TimetableDto
            {
                Id = ot.Id,
                Name = ot.OpendTimetable.Name,
                Lessons = ot.OpendTimetable.Lessons
                    .Where(l => l.Weekday == openDayOfWeek)
                    .Select(lesson => new LessonDto
                    {
                        Id = lesson.Id,
                        LessonType = lesson.LessonType,
                        TimeSlotBegin = lesson.TimeSlotBegin,
                        TimeSlotDuration = lesson.TimeSlotDuration,
                        CourseName = lesson.Course!.Name,
                        TrimmedCourseName = lesson.Course.Name.Substring(0, TrimCourseNameLength),
                    })
                    .ToList(),
            })
            .ToListAsync();
        
        var timetablesFromUsers = await _db.Timetables
            .Where(t => t.Name != String.Empty)
            .Select(t => new TimetableDto
            {
                Id = t.Id,
                Name = t.Name,
            })
            .ToListAsync();

        _timeslotConverter.FillAllTimeSlots(plannedLessons);
        
        var model = new PlanningViewModel
        {
            TimetableId = timetableId,
            OpenDayOfWeek = openDayOfWeek,
            CoursesOfStudy = coursesOfStudy,
            TimetablesFromUsers = timetablesFromUsers,
            PlannedLessons = plannedLessons,
            OpenSemesters = openTimetableSemestersData.Select(_openTimetableSemesterMapper.Map).ToList(),
            OpenTimetables = openTimetables.Select(_openTimetableMapper.Map).ToList(),
        };

        return model;
    }
}