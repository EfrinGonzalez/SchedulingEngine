namespace Planday.Schedule.Application.Interfaces.Infrastructure.Persistence.Commands
{
    public interface IAssignShiftCommand
    {
        Task AssignShiftAsync(long shiftId, long employeeId, CancellationToken cancellationToken = default);
    }
}
