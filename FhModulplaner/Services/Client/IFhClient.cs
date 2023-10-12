namespace FhModulplaner.Services.Client
{
    public interface IFhClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseOfStudy">Defauölt gets all courses of study</param>
        /// <param name="semester">Default gets all semester</param>
        /// <param name="group">Default gets all groups</param>
        /// <returns></returns>
        Task<IEnumerable<Lesson>> GetTimetable(string courseOfStudy = "*", string semester = "*", string group = "*");
        Task<IDictionary<string, CourseOfStudy>> GetCoursesOfStudy();
    }
}
