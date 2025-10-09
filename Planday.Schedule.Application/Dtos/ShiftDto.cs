namespace Planday.Schedule.Application.Dtos
{
    public record ShiftDto(long Id, long? EmployeeId, DateTime Start, DateTime End, string? EmployeeEmail = null);
}
