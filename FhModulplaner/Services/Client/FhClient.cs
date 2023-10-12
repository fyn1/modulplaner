namespace FhModulplaner.Services.Client
{
    public class FhClient : IFhClient
    {


        public async Task<IEnumerable<Lesson>> GetTimetable(string courseOfStudy = "*", string semester = "*", string group = "*")
        {
            var uri = $"https://ws.inf.fh-dortmund.de/fbws/current/rest/CourseOfStudy/{courseOfStudy}/{semester}/Events?Accept=application/json&studentSet={group}";

            var client = new HttpClient();
            var response = client.GetAsync(uri);

            var timetable2 = await response.Result.Content.ReadAsStringAsync();
            var timetable = await response.Result.Content.ReadFromJsonAsync<List<Lesson>>();


            return timetable;
        }

        public async Task<IDictionary<string, CourseOfStudy>> GetCoursesOfStudy()
        {
            string uri = "https://ws.inf.fh-dortmund.de/fbws/current/rest/CourseOfStudy/Events?Accept=application/json";

            var client = new HttpClient();
            var response = client.GetAsync(uri);

            var coursesOfStudyResponse = await response.Result.Content.ReadFromJsonAsync<IDictionary<string, CourseOfStudy>>();

            return coursesOfStudyResponse;
        }

    }
}
