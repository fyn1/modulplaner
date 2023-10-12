namespace FhModulplaner.Data
{
    public class User
    {
        public Guid Id { get; set; }
        
        public string ADId { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Surname { get; set; } = string.Empty;
        
        public ICollection<Timetable> Timetables { get; set; } = new List<Timetable>();
    }
}
