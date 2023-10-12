namespace FhModulplaner.Services.Client
{
    public class Lesson
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EventType { get; set; }
        public string CourseId { get; set; }
        public string CourseOfStudy { get; set; }
        public string ExaminationReg { get; set; }
        public string TermId { get; set; }
        public int Grade { get; set; }
        public object Description { get; set; }
        public object Note { get; set; }
        public string CourseType { get; set; }
        public string LecturerId { get; set; }
        public string LecturerName { get; set; }
        public string LecturerSurname { get; set; }
        public string StudentSet { get; set; }
        public string RoomId { get; set; }
        public int DateBegin { get; set; }
        public int DateEnd { get; set; }
        public string TimeBegin { get; set; }
        public string TimeEnd { get; set; }
        public int TimestampBegin { get; set; }
        public int TimestampEnd { get; set; }
        public int TimeSlotBegin { get; set; }
        public int TimeSlotDuration { get; set; }
        public int TimeSlotColum { get; set; }
        public string Weekday { get; set; }
        public string Interval { get; set; }
        public string Flags { get; set; }
        public object Creator { get; set; }
        public int Created { get; set; }
        public int Modified { get; set; }
    }
}
