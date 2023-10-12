using FhModulplaner.Data;
using FhModulplaner.Services.Client;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Globalization;

namespace FhModulplaner.Services
{
    public class SyncFhDataBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SyncFhDataBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromDays(1);

        public SyncFhDataBackgroundService(IServiceScopeFactory serviceScopeFactory, ILogger<SyncFhDataBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_period);

            do
            {
                _logger.LogInformation("Start Fh syncing ...");

                await StartSync();

                _logger.LogInformation("Fh sync was successful");
            } while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task StartSync()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var fhClient = scope.ServiceProvider.GetRequiredService<IFhClient>();

            await SyncCoursesOfStudy(fhClient, dbContext);

            await SyncLessonsAndCourses(fhClient, dbContext);
            
        }

        private async Task SyncCoursesOfStudy(IFhClient fhClient,AppDbContext dbContext)
        {
            var coursesOfStudy = await fhClient.GetCoursesOfStudy();
            var coursesOfStudyFromDb = await dbContext.CoursesOfStudies.ToListAsync();

            var coursesOfStudyNotInDatabase = coursesOfStudy
                .Select(course => course.Value)
                .Where(course => course.Name is not null)
                .Where(course => !coursesOfStudyFromDb.Exists(c => c.ShortName.Equals(course.Sname)))
                .Select(course => new Data.CourseOfStudy
                {
                    Id = Guid.NewGuid(),
                    ShortName = course.Sname,
                    Name = course.Name,
                    Po = course.Po,

                })
                .ToList();

            await dbContext.CoursesOfStudies.AddRangeAsync(coursesOfStudyNotInDatabase);

            await dbContext.SaveChangesAsync();
            _logger.LogInformation($"Added {coursesOfStudyNotInDatabase.Count()} new coursesOfStudy");
        }

        public async Task SyncLessonsAndCourses(IFhClient fhClient, AppDbContext dbContext)
        {
            var courseOfStudyDict = await dbContext.CoursesOfStudies.ToDictionaryAsync(course => course.ShortName, course => course.Id);

            var allClientLessons = await fhClient.GetTimetable();

            var allCourses = new List<Course>();
            var coursesFromDb = await dbContext.Courses.ToListAsync();

            var allDataLessons = new List<Data.Lesson>();
            var dataLessonsFromDb = await dbContext.Lessons.ToListAsync();

            foreach (var clientLesson in allClientLessons)
            {
                var course = MapLessonToCourse(clientLesson, courseOfStudyDict[clientLesson.CourseOfStudy]);
                var dataLesson = MapClientLessonToDataLesson(clientLesson, course.Id);

                allCourses.Add(course);
                allDataLessons.Add(dataLesson);
            }

            allCourses = allCourses.Where(course => !coursesFromDb.Exists(c => c.FhCourseId.Equals(course.FhCourseId))).ToList();

            allDataLessons = allDataLessons.Where(dataLesson => !dataLessonsFromDb.Exists(dbLesson => 
            dbLesson.Room == dataLesson.Room &&
            dbLesson.TimeBegin == dataLesson.TimeBegin &&
            dbLesson.Weekday == dataLesson.Weekday
            )).ToList();

            await dbContext.Courses.AddRangeAsync(allCourses);
            await dbContext.Lessons.AddRangeAsync(allDataLessons);

            await dbContext.SaveChangesAsync();
            _logger.LogInformation($"Added {allCourses.Count()} new courses");
            _logger.LogInformation($"Added {allDataLessons.Count()} new lessons");
        }

        private Data.Course MapLessonToCourse(Client.Lesson lesson, Guid courseOfStudyId)
        {
            Data.Course course= new Data.Course
            {
                Id = Guid.NewGuid(),
                FhCourseId = lesson.CourseId,
                Name = lesson.Name,
                Semester = lesson.Grade,
                CourseOfStudyId = courseOfStudyId,
            };

            return course;
        }

        private Data.Lesson MapClientLessonToDataLesson(Client.Lesson lesson, Guid courseId)
        {
            Data.Lesson result = new Data.Lesson
            {
                LessonType = MapCourseTypeToLessenType(lesson.CourseType),
                StudentSet= lesson.StudentSet,
                Room = lesson.RoomId,
                TimeBegin = ParseStringToDateTime(lesson.TimeBegin),
                TimeEnd = ParseStringToDateTime(lesson.TimeEnd),
                TimeSlotBegin = lesson.TimeSlotBegin,
                TimeSlotDuration = lesson.TimeSlotDuration,
                Weekday = MapWeekdayToDayOfWeek(lesson.Weekday),
                CourseId = courseId
            };

            return result;
        }

        private DateTime ParseStringToDateTime(string time)
        {
            if(time.Length == 3)
                time = "0" + time;

            return DateTime.ParseExact(time, "HHmm", CultureInfo.InvariantCulture, DateTimeStyles.None);

        }

        private DayOfWeek MapWeekdayToDayOfWeek(string weekday)
        {
            return weekday switch
            {
                "Mon" => DayOfWeek.Monday,
                "Tue" => DayOfWeek.Tuesday,
                "Wed" => DayOfWeek.Wednesday,
                "Thu" => DayOfWeek.Thursday,
                "Fri" => DayOfWeek.Friday,
                _ => throw new Exception($"weekday '{weekday}' not supportet "),
            };
        }

        private LessonType MapCourseTypeToLessenType(string courseType)
        {
            return courseType switch
            {
                "V" => LessonType.Lecture,
                "P" => LessonType.Practise,
                "Ü" => LessonType.Exercise,
                "SV" => LessonType.SeminarLecture,
                "S" => LessonType.Seminar,
                "PR" => LessonType.ProjectWeek,
                "ÜP" => LessonType.QualityOfLearning,
                "ÜPP" => LessonType.QualityOfLearning,
                "T" => LessonType.Tutorium,
                "Org" => LessonType.Organisation,
                _ => throw new Exception($"courseType '{courseType}' not supportet"),
            };
        }

    }
}
