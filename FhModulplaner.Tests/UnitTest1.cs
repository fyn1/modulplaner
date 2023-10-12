using FhModulplaner.Models.Timetable;
using FhModulplaner.Services.Planning;
using Microsoft.AspNetCore.Builder;

namespace FhModulplaner.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData(0, 2, 1, 3, true)]
    [InlineData(1, 3, 0, 2, true)]
    [InlineData(0, 2, 0, 1, true)]
    [InlineData(0, 2, 3, 1, false)]
    [InlineData(6, 2, 6, 2, true)]
    public void IsLessonOverlappingWithLesson(int start1, int duration1, int start2, int duration2, bool result)
    {
        var timeslotConverter = new TimeslotConverter();
        
        var Lesson1 = new LessonDto
        {
            TimeSlotBegin = start1,
            TimeSlotDuration = duration1
        };

        var Lesson2 = new LessonDto
        {
            TimeSlotBegin = start2,
            TimeSlotDuration = duration2
        };

        var res = timeslotConverter.IsLessonInTimeSlot(Lesson1, Lesson2);

        Assert.Equal(result, res);
    }

    [Fact]
    public void FillAllTimeSlotsCorrectly()
    {
        var timeslotConverter = new TimeslotConverter();
        
        List<LessonDto> lessons = new List<LessonDto>
        {
            new LessonDto()
            {
                CourseName = "Mathe",
                TimeSlotBegin = 0,
                TimeSlotDuration = 2
            },
            new LessonDto()
            {
                CourseName = "Qdl 1",
                TimeSlotBegin = 4,
                TimeSlotDuration = 2
            },
            new LessonDto()
            {
                CourseName = "Qdl 2",
                TimeSlotBegin = 6,
                TimeSlotDuration = 2
            },
            new LessonDto()
            {
                CourseName = "Englisch",
                TimeSlotBegin = 8,
                TimeSlotDuration = 2
            },
        };
        
        timeslotConverter.FillAllTimeSlots(lessons);
        
        
        Assert.Equal(14, lessons.Count);
        Assert.Equal("Mathe",lessons[0].CourseName);
        Assert.Equal("Mathe",lessons[1].CourseName);
        Assert.Equal(LessonDto.Empty,lessons[2]);
        Assert.Equal(LessonDto.Empty,lessons[3]);
        Assert.Equal("Qdl 1",lessons[4].CourseName);
        Assert.Equal("Qdl 1",lessons[5].CourseName);
        Assert.Equal("Qdl 2",lessons[6].CourseName);
        Assert.Equal("Qdl 2",lessons[7].CourseName);
        Assert.Equal("Englisch",lessons[8].CourseName);
        Assert.Equal("Englisch",lessons[9].CourseName);
        Assert.Equal(LessonDto.Empty,lessons[10]);
        Assert.Equal(LessonDto.Empty,lessons[11]);
        Assert.Equal(LessonDto.Empty,lessons[12]);
        Assert.Equal(LessonDto.Empty,lessons[13]);
    }
    
    [Fact]
    public void FillAllTimeSlotsCorrectlyWithChanges()
    {
        var timeslotConverter = new TimeslotConverter();
        
        List<LessonDto> lessons = new List<LessonDto>
        {
            new LessonDto()
            {
                CourseName = "Jahresabschluss 2",
                TimeSlotBegin = 8,
                TimeSlotDuration = 4
            },
        };
        
        //timeslotConverter.FillAllTimeSlots(lessons);

        var lesson = new LessonDto()
        {
            CourseName = "Qdl Intensiv",
            TimeSlotBegin = 6,
            TimeSlotDuration = 2
        };
        
        lessons.Add(lesson);
        
        timeslotConverter.FillAllTimeSlots(lessons);
        
        
        Assert.Equal(14, lessons.Count);
        Assert.Equal(LessonDto.Empty,lessons[0]);
        Assert.Equal(LessonDto.Empty,lessons[1]);
        Assert.Equal(LessonDto.Empty,lessons[2]);
        Assert.Equal(LessonDto.Empty,lessons[3]);
        Assert.Equal(LessonDto.Empty,lessons[4]);
        Assert.Equal(LessonDto.Empty,lessons[5]);
        Assert.Equal("Qdl Intensiv",lessons[6].CourseName);
        Assert.Equal("Qdl Intensiv",lessons[7].CourseName);
        Assert.Equal("Jahresabschluss 2",lessons[8].CourseName);
        Assert.Equal("Jahresabschluss 2",lessons[9].CourseName);
        Assert.Equal("Jahresabschluss 2", lessons[10].CourseName);
        Assert.Equal("Jahresabschluss 2", lessons[11].CourseName);
        Assert.Equal(LessonDto.Empty,lessons[12]);
        Assert.Equal(LessonDto.Empty,lessons[13]);
    }
}