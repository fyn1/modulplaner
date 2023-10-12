using System.Security.Claims;
using FhModulplaner.Data;
using FhModulplaner.Models.Timetable;
using FhModulplaner.Services;
using FhModulplaner.Services.Auth;
using FhModulplaner.Services.Client;
using FhModulplaner.Services.Planning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Controllers
{
    [Authorize]
    public class TimetableController : Controller
    {
        private readonly IFhClient _fhClient;
        private readonly AppDbContext _db;


        public TimetableController(IFhClient fhClient, AppDbContext db)
        {
            _fhClient = fhClient;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var model = new IndexViewModel
            {
                Name = "Index",
            };

            var res = await _fhClient.GetTimetable("STDBSY", "4");
            var res2 = await _fhClient.GetCoursesOfStudy();

            return View(model);
        }

        public async Task<IActionResult> Planning(
            [FromServices] ITimetablePlanningQuery timetablePlanningQuery,
            [FromServices] ICreateTimetableCommand createTimetableCommand,
            Guid? timetableId,
            DayOfWeek openDayOfWeek = DayOfWeek.Monday)
        {
            if (timetableId is null)
            {
                var userId = Guid.Parse(User.FindFirstValue(AppClaimTypes.UserId));
                timetableId = await createTimetableCommand.CreateTimetable(userId);
                return RedirectToAction(nameof(Planning), new
                {
                    timetableId = timetableId.Value,
                    openDayOfWeek = openDayOfWeek
                });
            }

            var model = await timetablePlanningQuery.GetPlanningViewModel(timetableId.Value, openDayOfWeek);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> NewSemester([FromServices]IAddSemesterToPlanningCommand command, Guid courseOfStudyId, Guid timetableId, int semester)
        {
            await command.AddOpenTimeTableSemester(courseOfStudyId, timetableId, semester);
            
            return RedirectToAction(nameof(Planning), new {timetableId = timetableId});
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSemester([FromServices] IRemoveSemesterFromPlanningCommand command, Guid openSemesterId,
            Guid timetableId)
        {
            await command.RemoveOpenTimeTableSemester(openSemesterId);

            return RedirectToAction(nameof(Planning), new { timetableId = timetableId });

        }
        
        [HttpPost]
        public async Task<IActionResult> AddLesson([FromServices] IAddLessonToTimetableCommand command, Guid timetableId, Guid lessonId, DayOfWeek openDayOfWeek)
        {
            await command.AddLessonToTimetable(timetableId, lessonId);

            return RedirectToAction(nameof(Planning), new { timetableId = timetableId, openDayOfWeek = openDayOfWeek });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveLesson([FromServices] IRemoveLessonFromTimetableCommand command, Guid timetableId, Guid lessonId, DayOfWeek openDayOfWeek)
        {
            await command.RemoveLessonFromTimetable(timetableId, lessonId);

            return RedirectToAction(nameof(Planning), new { timetableId = timetableId, openDayOfWeek = openDayOfWeek });
        }

        [HttpPost]
        public async Task<IActionResult> ResetTimetable([FromServices] IResetTimetableCommand command, Guid timetableId)
        {
            await command.ResetTimetable(timetableId);

            return RedirectToAction(nameof(Planning), new { timetableId = timetableId });
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveTimetable([FromServices] ISaveTimetableInDbCommand command, Guid timetableId, string timetableName)
        {
            await command.SaveTimetableInDb(timetableId, timetableName);

            return RedirectToAction(nameof(Planning), new { timetableId = timetableId });
        }
        
        [HttpPost]
        public async Task<IActionResult> AddOpenTimetable([FromServices] IAddTimetableToPlanningCommand command, Guid timetableId, Guid openTimetableId)
        {
            await command.AddTimetableToPlanning(timetableId, openTimetableId);

            return RedirectToAction(nameof(Planning), new { timetableId = timetableId });
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveOpenTimetable([FromServices] IRemoveTimetableFromPlanningCommand command, Guid timetableId, Guid openTimetableId)
        {
            await command.RemoveTimetableFromPlanning(openTimetableId);

            return RedirectToAction(nameof(Planning), new { timetableId = timetableId });
        }

        [HttpGet]
        public IActionResult GetSemester(Guid courseofStudyId)
        {
            var semesters = _db.Courses
                .Where(c => c.CourseOfStudyId == courseofStudyId)
                .Select(c => c.Semester)
                .ToHashSet()
                .ToList()
                .OrderBy(s => s);

            return Json(semesters);
        }
    }
}