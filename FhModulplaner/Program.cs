using FhModulplaner.Data;
using FhModulplaner.Services;
using FhModulplaner.Services.Auth;
using FhModulplaner.Services.Client;
using FhModulplaner.Services.Planning;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IFhClient, FhClient>();
builder.Services.AddHostedService<SyncFhDataBackgroundService>();
builder.Services.AddTransient<ITimetablePlanningQuery, TimetablePlanningQuery>();
builder.Services.AddTransient<ICreateTimetableCommand, CreateTimetableCommand>();
builder.Services.AddTransient<IAddSemesterToPlanningCommand, AddSemesterToPlanningCommand>();
builder.Services.AddTransient<IRemoveSemesterFromPlanningCommand, RemoveSemesterFromPlanningCommand>();
builder.Services.AddTransient<IOpenTimetableSemesterMapper, OpenTimetableSemesterMapper>();
builder.Services.AddTransient<IAddLessonToTimetableCommand, AddLessonToTimetableCommand>();
builder.Services.AddTransient<ITimeslotConverter, TimeslotConverter>();
builder.Services.AddTransient<IRemoveLessonFromTimetableCommand, RemoveLessonFromTimetableCommand>();
builder.Services.AddTransient<IResetTimetableCommand, ResetTimetableCommand>();
builder.Services.AddTransient<ISaveTimetableInDbCommand, SaveTimetableInDbCommand>();
builder.Services.AddTransient<IAddTimetableToPlanningCommand, AddTimetableToPlanningCommand>();
builder.Services.AddTransient<IOpenTimetableMapper, OpenTimetableMapper>();
builder.Services.AddTransient<IRemoveTimetableFromPlanningCommand, RemoveTimetableFromPlanningCommand>();
builder.Services.AddDbContext<AppDbContext>(options => 
{
    options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContext"));
});
//builder.Services.AddHostedService<SyncFhDataBackgroundService>();

builder.Services.AddAuth(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await db.SaveChangesAsync();

    // if (!(await db.Students.AnyAsync()))
    // {
    //     await db.Students.AddAsync(new User()
    //     {
    //         Id = Guid.NewGuid(),
    //         Name = "Fyn",
    //         Surname = "Hoffmann",
    //     });
    //     await db.SaveChangesAsync();
    // }
}


app.Run();
