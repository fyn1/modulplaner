using FhModulplaner.Models.Timetable;

namespace FhModulplaner.Services.Planning;

public interface IOpenTimetableSemesterMapper
{
    OpenTimetableSemesterDto Map(OpenTimetableSemesterData semesterData);
}

public class OpenTimetableSemesterMapper : IOpenTimetableSemesterMapper
{
    private readonly ITimeslotConverter _timeslotConverter;
    
    public OpenTimetableSemesterMapper(ITimeslotConverter timeslotConverter)
    {
        _timeslotConverter = timeslotConverter;
    }
    
    public OpenTimetableSemesterDto Map(OpenTimetableSemesterData semesterData)
    {
        var lessonsByTimeSlot = new List<List<LessonDto>>();
        var currentLessons = semesterData.Lessons.OrderBy(l => l.TimeSlotBegin).ToList();
        var lessonsInNextTimeSlot = new List<LessonDto>();

        do
        {
            foreach (var currentLesson in currentLessons)
            {
                if (lessonsInNextTimeSlot.Any(lesson => lesson.Id == currentLesson.Id))
                {
                    continue;
                }

                var overlappingLessons = currentLessons
                    .Where(lesson => lesson.Id != currentLesson.Id)
                    .Where(lesson => lesson.Weekday == currentLesson.Weekday)
                    .ToList()
                    .Where(lesson => _timeslotConverter.IsLessonInTimeSlot(currentLesson, lesson));

                lessonsInNextTimeSlot.AddRange(overlappingLessons);
            }

            foreach (var lesson in lessonsInNextTimeSlot)
            {
                currentLessons.Remove(lesson);
            }

            lessonsByTimeSlot.Add(currentLessons);

            currentLessons = lessonsInNextTimeSlot;
            lessonsInNextTimeSlot = new List<LessonDto>();
            
        } while (currentLessons.Any());

        foreach (var lessonsList in lessonsByTimeSlot)
        {
            _timeslotConverter.FillAllTimeSlots(lessonsList);
        }
        
        var model = new OpenTimetableSemesterDto
        {
            Id = semesterData.Id,
            CourseOfStudyId = semesterData.CourseOfStudyId,
            CourseOfStudyShortName = semesterData.CourseOfStudyShortName,
            CourseOfStudyName = semesterData.CourseOfStudyName,
            Semester = semesterData.Semester,
            Lessons = lessonsByTimeSlot,
        };

        return model;
    }
}