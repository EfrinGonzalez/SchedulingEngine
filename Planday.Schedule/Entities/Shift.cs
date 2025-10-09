namespace Planday.Schedule.Domain.Entities
{
    public class Shift(long id, long? employeeId, DateTime start, DateTime end)
    {
        public long Id { get; } = id;
        public long? EmployeeId { get; } = employeeId;
        public DateTime Start { get; } = start;
        public DateTime End { get; } = end;
    }    
}

