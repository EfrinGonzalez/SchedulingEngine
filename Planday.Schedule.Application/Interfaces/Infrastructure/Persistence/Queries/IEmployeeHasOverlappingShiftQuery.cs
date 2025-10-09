namespace Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Queries
{
    public interface IEmployeeHasOverlappingShiftQuery
    {
        Task<bool> HasOverlappingShiftAsync(long employeeId, DateTime start, DateTime end, long excludeShiftId, CancellationToken cancellationToken = default);
    }
}
