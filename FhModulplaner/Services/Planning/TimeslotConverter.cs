using FhModulplaner.Models.Timetable;

namespace FhModulplaner.Services.Planning;

public interface ITimeslotConverter
{
    void FillAllTimeSlots(List<LessonDto> lessons);
    bool IsLessonInTimeSlot(LessonDto lesson1, LessonDto lesson2);
    bool IsLessonInTimeSlot(int startSlot1, int slotDuration1, int startSlot2, int slotDuration2);
}

public class TimeslotConverter : ITimeslotConverter
{
    public void FillAllTimeSlots(List<LessonDto> lessons)
    {
        lessons.Sort((lesson1, lesson2) => lesson1.TimeSlotBegin.CompareTo(lesson2.TimeSlotBegin));
        
        for (int i = 0; i < 14; i++)
        {
            if (i >= lessons.Count)
            {
                lessons.Add(LessonDto.Empty);
                continue;
            }

            var temp = lessons[i];

            if (temp.TimeSlotBegin > i)
            {
                lessons.Insert(i, LessonDto.Empty);
            }
            else if (temp.TimeSlotDuration > 1)
            {
                for (int j = 1; j < temp.TimeSlotDuration; j++)
                {
                    lessons.Insert(j + i, temp);
                }

                i += temp.TimeSlotDuration - 1;
            }
        }
    }

    public bool IsLessonInTimeSlot(LessonDto lesson1, LessonDto lesson2)
    {
        var begin1 = lesson1.TimeSlotBegin;
        var end1 = lesson1.TimeSlotBegin + lesson1.TimeSlotDuration - 1;

        var begin2 = lesson2.TimeSlotBegin;
        var end2 = lesson2.TimeSlotBegin + lesson2.TimeSlotDuration - 1;

        return end1 >= begin2 && begin1 <= end2 || end2 >= begin1 && begin2 <= end1;
    }
    
    public bool IsLessonInTimeSlot(int startSlot1, int slotDuration1, int startSlot2, int slotDuration2)
    {
        var end1 = startSlot1 + slotDuration1 - 1;
        var end2 = startSlot2 + slotDuration2 - 1;

        return end1 >= startSlot2 && startSlot1 <= end2 || end2 >= startSlot1 && startSlot2 <= end1;
    }
}