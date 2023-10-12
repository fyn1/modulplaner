using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Data
{
    public class AppDbContext : DbContext
    {   
        public AppDbContext()
        {

        }

        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseOfStudy> CoursesOfStudies { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<User> Students { get; set; }
        public DbSet<Timetable> Timetables { get; set; }
        public DbSet<OpenTimetableSemester> OpenTimetableSemesters { get; set; }
        public DbSet<OpenTimetable> OpenTimetables { get; set; }
    }
}
