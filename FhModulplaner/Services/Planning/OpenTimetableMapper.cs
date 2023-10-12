using FhModulplaner.Models.Timetable;

namespace FhModulplaner.Services.Planning;

public interface IOpenTimetableMapper
{
    TimetableDto Map(TimetableDto timetableDto);
}

public class OpenTimetableMapper : IOpenTimetableMapper
{
    private readonly ITimeslotConverter _timeslotConverter;
    
    public OpenTimetableMapper(ITimeslotConverter timeslotConverter)
    {
        _timeslotConverter = timeslotConverter;
    }

    public TimetableDto Map(TimetableDto timetableDto)
    {
        _timeslotConverter.FillAllTimeSlots(timetableDto.Lessons);
        
        return timetableDto;
    }
}